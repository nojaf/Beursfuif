using Beursfuif.BL;
using Beursfuif.BL.Event;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Beursfuif.Server.Services
{
    public class BeursfuifServer:IBeursfuifServer
    {
        private IDisposable _webApp;
        private IHubContext _hub;

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

        public event EventHandler<BL.Event.BasicAuthAckEventArgs> CurrentTimeAckEvent;

        public event EventHandler<BL.Event.BasicAuthAckEventArgs> IntervalUpdateAckEvent;
        #endregion

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

        public BeursfuifServer()
        {
            Clients = new List<BL.Client>();
        }

        private void InitServer(object state)
        {
            _webApp = WebApp.Start<Startup>("http://"+Ip + ":" + Port);
            _hub = GlobalHost.ConnectionManager.GetHubContext<BeursfuifHub>();
            BeursfuifHub.NewPackageReceived += BeursfuifHub_NewPackageReceived;
        }

        #region Receive from client
        void BeursfuifHub_NewPackageReceived(object sender, BL.Package e)
        {
            if (!Active)
            {

                return;
            }

            switch(e.Kind)
            {
                case BL.ProtocolKind.NEW_CLIENT_CONNECTS:
                    AddNewClient(e);
                    break;
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
            throw new NotImplementedException();
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

        public void Start(string ip, int port)
        {
            if (string.IsNullOrEmpty(Ip))
            {
                Ip = ip;
                Port = port;
                ThreadPool.QueueUserWorkItem(new WaitCallback(InitServer));
            }
        }
    }
}
