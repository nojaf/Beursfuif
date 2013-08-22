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

        //ID , UserContext
        public Dictionary<int,UserContext> Clients { get; set; }
        private WebSocketServer _server;

        public BeursfuifServer(int port)
        {
            Clients = new Dictionary<int, UserContext>();
            _server = new Alchemy.WebSocketServer(port, IPAddress.Any)
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
                    case 1:
                        OnNewClientEvent(this,new NewClientEventArgs(p.ClientName, context.ClientAddress.ToString()));
                        Clients.Add(GetNextId(), context);
                        //TODO: ack ID + currentInterval, methode here but called from VM after event
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
        #endregion

        public int GetNextId()
        {
            return (Clients == null || Clients.Count == 0 ? 1 : Clients.Max(x => x.Key) + 1);
        }
    }
}
