using Beursfuif.BL;
using Beursfuif.BL.Event;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beursfuif.Server.Services
{
    public class BeursfuifServer:IBeursfuifServer
    {

        #region Fields and properties
        private IDisposable _webApp;
        private IHubContext _hub;

        public List<BL.Client> Clients
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public string Ip
        {
            get;
            set;
        }

        public bool Active
        {
            get;
            set;
        }
        #endregion

        public BeursfuifServer()
        {
            Clients = new List<BL.Client>();
        }

        private bool InitServer(object state)
        {
            try
            {
                _webApp = WebApp.Start<Startup>("http://" + Ip + ":" + Port);
                _hub = GlobalHost.ConnectionManager.GetHubContext<BeursfuifHub>();
                BeursfuifHub.NewPackageReceived += BeursfuifHub_NewPackageReceived;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        #region Events
        public event EventHandler<NewClientEventArgs> NewClientEvent;
        protected void OnNewClient(object sender, NewClientEventArgs e)
        {
            if (NewClientEvent != null)
            {
                NewClientEvent(sender, e);
            }
        }

        public event EventHandler<BL.Event.NewOrderEventArgs> NewOrderEvent;

        public event EventHandler<BL.Event.ClientLeftEventArgs> ClientLeftEvent;
        protected void OnClientLeftEvent(object sender, ClientLeftEventArgs e)
        {
            if (ClientLeftEvent != null)
            {
                ClientLeftEvent(sender, e);
            }
        }

        public event EventHandler<BL.Event.BasicAuthAckEventArgs> CurrentTimeAckEvent;
        protected void OnCurrentTimeAckEvent(object sender, BasicAuthAckEventArgs e)
        {
            if (CurrentTimeAckEvent != null)
            {
                CurrentTimeAckEvent(sender, e);
            }
        }

        public event EventHandler<BL.Event.BasicAuthAckEventArgs> IntervalUpdateAckEvent;
        #endregion

        #region Receive from client
        void BeursfuifHub_NewPackageReceived(object sender, BL.Package e)
        {
            if (!Active)
            {
                //TODO: send message to client that server isn't running
                return;
            }

            switch(e.Kind)
            {
                case BL.ProtocolKind.NEW_CLIENT_CONNECTS:
                    AddNewClient(e);
                    break;
                case ProtocolKind.CLIENT_LEAVES_SERVER:
                    RemoveClient(e);
                    break;
                case ProtocolKind.ACK_TIME_UPDATE:
                    AckTimeUpdate(e);
                    break;
            }
        }

        private void AckTimeUpdate(Package e)
        {
            Client client = GetClientByConnectionContext(e.ClientContext);
            if(client != null)
            {
                OnCurrentTimeAckEvent(this,new BasicAuthAckEventArgs(client.Id,e.AuthenticationCode));
            }
        }

        private void RemoveClient(Package e)
        {
            Client client = GetClientByConnectionContext(e.ClientContext);
            if(client != null)
            {
                OnClientLeftEvent(this, new ClientLeftEventArgs(client.Id));
                Clients.Remove(client);
            }
        }

        private void AddNewClient(BL.Package e)
        {
            Client newClient = new Client(Guid.NewGuid())
            {
                Name = e.Name,
                ConnectionContext  = e.ClientContext,
                Ip = e.Ip
            };

            Clients.Add(newClient);

            OnNewClient(this, new NewClientEventArgs(newClient.Name, newClient.Ip, newClient.Id));
        }

        //Helper method
        private Client GetClientByConnectionContext(string context)
        {
            return Clients.FirstOrDefault(x => x.ConnectionContext == context);
        }
        #endregion

        #region Send to client
        public void SendAckInitialClientConnect(BL.ClientInterval currentInterval, Guid clientId, DateTime currentBeursfuifTime)
        {
            string context = Clients.FirstOrDefault(x => x.Id == clientId).ConnectionContext;
            //SignalR method
            _hub.Clients.Client(context).sendInitialData(currentBeursfuifTime, currentInterval);
        }

        public void KickClient(Guid id)
        {
            //SignalR Method
            Client toKick = Clients.FirstOrDefault(x => x.Id == id);
            if(toKick != null)
            {
                _hub.Clients.Client(toKick.ConnectionContext).youGotKicked();
            }
        }

        public void UpdateTime(DateTime currentTime, string authenticationCode)
        {
            //SignalR Method
            _hub.Clients.All.UpdateTime(currentTime, authenticationCode);
        }

        public void UpdateInterval(BL.ClientInterval clientInterval, DateTime currentBFTime)
        {
           // throw new NotImplementedException();
        }

        public void SendDrinkAvailableChanged(int drinkId, BL.Drink drink, int intervalId)
        {
            throw new NotImplementedException();
        }

        public void SendAckNewOrder(Guid clientId, string authCode, DateTime currentBeursfuifTime)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void DisposeConnection()
        {
            if(_webApp != null)
            {
                _webApp.Dispose();
            }
        }

        public async Task<bool> Start(string ip, int port)
        {

                if (string.IsNullOrEmpty(Ip))
                {
                    Ip = ip;
                    Port = port;
                    return await Task.Factory.StartNew<bool>(InitServer,null);
                }
                return false;
        }
    }
}
