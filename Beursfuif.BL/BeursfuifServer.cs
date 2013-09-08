using Alchemy;
using Alchemy.Classes;
using Beursfuif.BL.Event;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;

namespace Beursfuif.BL
{
    public class BeursfuifServer
    {
        public event EventHandler<NewClientEventArgs> NewClientEvent;
        public event EventHandler<NewOrderEventArgs> NewOrderEvent;
        public event EventHandler<ClientLeftEventArgs> ClientLeftEvent;

        //ID , UserContext
        public Dictionary<int,UserContext> Clients { get; set; }
        private WebSocketServer _server;
        //TODO: make port dynamic
        public const int Port = 5678;

        public BeursfuifServer()
        {
            Clients = new Dictionary<int, UserContext>();
            _server = new Alchemy.WebSocketServer(Port, IPAddress.Any)
                        {
                            OnReceive = OnWebSocketMessageReceive,
                            OnSend = OnWebSocketMessageSend,
                            OnConnected = OnWebSocketClientConnect,
                            OnDisconnect = OnWebSocketClientDisconnect,
                            TimeOut = new TimeSpan(0, 5, 0)
                        };
        }

        private void OnWebSocketClientDisconnect(Alchemy.Classes.UserContext context)
        {
            var kv = Clients.FirstOrDefault(x => x.Value == context);
            if (kv.Value != null)
            {
                Console.WriteLine("Client ID:" + kv.Key + " , " + kv.Value.ClientAddress.ToString() + " left.");
                OnClientLeftEvent(this, new ClientLeftEventArgs(kv.Key));
                Clients.Remove(kv.Key);
            }
           
        }

        private void OnWebSocketClientConnect(Alchemy.Classes.UserContext context)
        {
            Console.WriteLine("Client " + context.ClientAddress.ToString() + " connected");
            int nextId = GetNextId();
            Clients.Add(nextId, context);
        }

        private void OnWebSocketMessageSend(Alchemy.Classes.UserContext context)
        {

        }

        private void OnWebSocketMessageReceive(Alchemy.Classes.UserContext context)
        {
            if (context.DataFrame != null)
            {
                Console.WriteLine("Message received from " + context.ClientAddress.ToString() + ": " + context.DataFrame);

                Package p = JsonConvert.DeserializeObject<Package>(context.DataFrame.ToString());
                switch (p.MessageId)
                {
                    case ProtocolKind.NEW_CLIENT_CONNECTS:
                        ReceivedNewClientConnect(context, p);
                        break;
                    case ProtocolKind.NEW_ORDER:
                        OnNewOrderEvent(this, new NewOrderEventArgs(p.ClientId, p.AuthenticationCode, p.NewOrder));
                        break;
                }
            }
        }



        public void StartServer()
        {
            Console.WriteLine("Server started");
            _server.Start();
        }

        public void StopServer()
        {
            _server.Stop();
        }

        public void RestartServer()
        {
            _server.Restart();
        }

        #region Events
        public void OnNewClientEvent(object sender, NewClientEventArgs e)
        {
            if (NewClientEvent != null)
            {
                NewClientEvent(sender, e);
            }
        }

        public void OnNewOrderEvent(object sender, NewOrderEventArgs e)
        {
            if(NewOrderEvent != null){
                NewOrderEvent(sender, e);
            }
        }

        public void OnClientLeftEvent(object sender, ClientLeftEventArgs e)
        {
            if (ClientLeftEvent != null)
            {
                ClientLeftEvent(sender, e);
            }
        }
        #endregion

        #region Send to client(s)
        public int GetNextId()
        {
            return (Clients == null || Clients.Count == 0 ? 1 : Clients.Max(x => x.Key) + 1);
        }

        public void SendAckInitialClientConnect(ClientInterval currentInterval, int clientId, DateTime currentBeursfuifTime)
        {
            Package p = new Package()
            {
                MessageId = ProtocolKind.ACK_NEW_CLIENT_CONNECTS,
                ClientId = clientId,
                CurrentInterval = currentInterval,
                CurrentBeursfuifTime = currentBeursfuifTime
            };

            string data = p.ToJSON();
            Clients.FirstOrDefault(x => x.Key == clientId).Value.Send(data);
        }

        public void KickClient(int id)
        {
            Package package = new Package()
            {
                MessageId = ProtocolKind.KICK_CLIENT
            };
            var context = Clients.FirstOrDefault(x => x.Key == id);
            context.Value.Send(package.ToJSON());
        }

        //This methode will kick all the clients because the connection will be closed
        public void Pause()
        {
            Package package = new Package()
            {
                MessageId = ProtocolKind.KICK_CLIENT
            };

            foreach (var client in Clients)
            {
                client.Value.Send(package.ToJSON());
            }
        }

        public void UpdateTime(DateTime currentTime)
        {
            Package package = new Package()
            {
                MessageId = ProtocolKind.TIME_UPDATE,
                CurrentBeursfuifTime = currentTime
            };

            Broadcast(package);
        }

        public void UpdateInterval(ClientInterval clientInterval, DateTime currentBFTime)
        {
            Package package = new Package()
            {
                MessageId = ProtocolKind.UPDATE_CLIENT_INTERVAL,
                CurrentInterval = clientInterval,
                CurrentBeursfuifTime = currentBFTime
            };
            Broadcast(package);

        }

        private void Broadcast(Package package)
        {
            foreach (var client in Clients)
            {
                //TODO: add some sort of timer to check if the connection is still healthy
                client.Value.Send(package.ToJSON());
            }
        }
        #endregion

        #region Received from client
        private void ReceivedNewClientConnect(Alchemy.Classes.UserContext context, Package p)
        {
            if (Clients.ContainsValue(context))
            {
                int id = Clients.First(x => x.Value == context).Key;
                OnNewClientEvent(this, new NewClientEventArgs(p.ClientName, context.ClientAddress.ToString(), id));
            }

            //TODO: ack ID + currentInterval, methode here but called from VM after event
        }

        //"zeikt in je eigen kulten"
        private void ReceivedTimeAck(Alchemy.Classes.UserContext context, Package p)
        {
            if (p.AuthenticationCode != "zeikt in je eigen kulten")
            {
                Console.WriteLine("Foute auth code");
            }

            //TODO: stop the timer that checks for the clients response 
        }
        #endregion


    }
}
