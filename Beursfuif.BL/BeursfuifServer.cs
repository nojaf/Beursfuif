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
                Clients.Remove(kv.Key);
            }
        }

        private void OnWebSocketClientConnect(Alchemy.Classes.UserContext context)
        {
            Console.WriteLine("Client " + context.ClientAddress.ToString() + " connected");
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
                        int nextId = GetNextId();
                        Clients.Add(nextId, context);
                        OnNewClientEvent(this,new NewClientEventArgs(p.ClientName, context.ClientAddress.ToString(),nextId));
                        
                        //TODO: ack ID + currentInterval, methode here but called from VM after event
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
        #endregion

        public int GetNextId()
        {
            return (Clients == null || Clients.Count == 0 ? 1 : Clients.Max(x => x.Key) + 1);
        }

        public void SendAckInitialClientConnect(ClientInterval currentInterval, int clientId)
        {
            Package p = new Package()
            {
                MessageId = 2,
                ClientId = clientId,
                CurrentInterval = currentInterval
            };

            string data = p.ToJSON();
            Clients.FirstOrDefault(x => x.Key == clientId).Value.Send(data);
        }
    }
}
