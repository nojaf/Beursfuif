using Beursfuif.BL;
using Beursfuif.BL.Event;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Beursfuif.Server.DataAccess;
using Beursfuif.BL.Extensions;
using Beursfuif.Server.Entity;

namespace Beursfuif.Server.Services
{
    public class BeursfuifServer:IBeursfuifServer
    {

        #region Fields and properties
        private IDisposable _webApp;
        private IHubContext _hub;
        private readonly IBeursfuifData _beursfuifData;

        public ObservableCollection<Client> Clients
        {
            get
            {
                return _beursfuifData.Clients;
            }
            set
            {
                _beursfuifData.Clients = value;
            }
        }

        public int Port => _beursfuifData.Port;

        public bool Active
        {
            get;
            set;
        }
        #endregion

        public BeursfuifServer(IBeursfuifData beursfuifData)
        {
            this._beursfuifData = beursfuifData;
        }

        private bool InitServer()
        {
            try
            {
                _webApp = WebApp.Start<Startup>($"http://+:{Port}");
                _hub = Startup.DependencyResolver.Resolve<IConnectionManager>().GetHubContext<BeursfuifHub>();
                BeursfuifHub.NewPackageReceived += BeursfuifHub_NewPackageReceived;
                return true;
            }
            catch (Exception ex)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<LogMessage>(new LogMessage() { 
                    Message = ex.Message,
                    Time = DateTime.Now,
                    Type = LogType.ERROR
                });
                return false;
            }

        }

        #region Events
        public event EventHandler<NewClientEventArgs> NewClientEvent;
        protected void OnNewClient(object sender, NewClientEventArgs e)
        {
            NewClientEvent?.Invoke(sender, e);
        }

        public event EventHandler<BL.Event.NewOrderEventArgs> NewOrderEvent;
        protected void OnNewOrderEvent(object sender, NewOrderEventArgs e)
        {
            NewOrderEvent?.Invoke(this, e);
        }

        public event EventHandler<BL.Event.ClientLeftEventArgs> ClientLeftEvent;
        protected void OnClientLeftEvent(object sender, ClientLeftEventArgs e)
        {
            ClientLeftEvent?.Invoke(sender, e);
        }

        public event EventHandler<BL.Event.BasicAuthAckEventArgs> CurrentTimeAckEvent;
        protected void OnCurrentTimeAckEvent(object sender, BasicAuthAckEventArgs e)
        {
            CurrentTimeAckEvent?.Invoke(sender, e);
        }

        public event EventHandler<BL.Event.BasicAuthAckEventArgs> IntervalUpdateAckEvent;
        protected void OnIntervalUpdateAckEvent(object sender, BasicAuthAckEventArgs e)
        {
            IntervalUpdateAckEvent?.Invoke(sender, e);
        }
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
                case ProtocolKind.NEW_ORDER:
                    NewOrder(e);
                    break;
                case ProtocolKind.ACK_UPDATE_CLIENT_INTERVAL:
                    AckIntervalUpdate(e);
                    break;
            }
        }

        private void AckIntervalUpdate(Package e)
        {
            Client client = GetClientByConnectionContext(e.ClientContext);
            if (client != null)
            {
                OnIntervalUpdateAckEvent(this, new BasicAuthAckEventArgs(client.Id, e.AuthenticationCode));
            }
        }

        private void NewOrder(Package e)
        {
            Client client = GetClientByConnectionContext(e.ClientContext);
            if (client != null)
            {
                OnNewOrderEvent(this, new NewOrderEventArgs(client.Id, e.AuthenticationCode, e.NewOrder));
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
                Action action = () =>
                {
                    if (Clients.Contains(client))
                    {
                        Clients.Remove(client);
                    }
                };

                if (App.Current != null && App.Current.Dispatcher != null)
                {
                    App.Current.Dispatcher.BeginInvoke(action);
                }
            }
        }

        private void AddNewClient(BL.Package e)
        {
            DateTime currentBFTime = _beursfuifData.BeursfuifCurrentTime;

            Client newClient = new Client(Guid.NewGuid())
            {
                Name = e.Name,
                ConnectionContext  = e.ClientContext,
                Ip = e.Ip,
                LastActivity = currentBFTime,
            };

            Action action = () =>
            {
                Clients.Add(newClient);
            };

            if(App.Current != null && App.Current.Dispatcher != null){
                App.Current.Dispatcher.BeginInvoke(action);
            }
        
 
            Interval currentInterval = _beursfuifData.CurrentInterval;
            ClientInterval clientInterval = currentInterval.ToClientInterval(currentBFTime, BeursfuifPaths.AssetsPath);
            SendAckInitialClientConnect(clientInterval, newClient.Id, currentBFTime);
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
            var client = Clients.FirstOrDefault(x => x.Id == clientId);
            if (client != null)
            {
                string context = client.ConnectionContext;
                //SignalR method
                var signalrClient = _hub.Clients.Client(context);
                _hub.Clients.Client(context).sendInitialData(currentBeursfuifTime, currentInterval);
            }
            else
            {
                System.Threading.Thread.Sleep(200);
                SendAckInitialClientConnect(currentInterval, clientId, currentBeursfuifTime);
            }
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
            if (_hub != null)
            {
                _hub.Clients.All.UpdateTime(currentTime, authenticationCode);
            }
        }

        public void UpdateInterval(BL.ClientInterval clientInterval, DateTime currentBFTime)
        {
           //SignalR Method
            _hub.Clients.All.UpdateInterval(clientInterval, currentBFTime);
        }

        public void SendDrinkAvailableChanged(ClientInterval currentInterval)
        {
            //SignalR Method
            _hub.Clients.All.DrinkAvailableChanged(currentInterval);
        }

        public void SendAckNewOrder(Guid clientId, string authCode, DateTime currentBeursfuifTime)
        {
            //SignalR Method
            Client client = Clients.FirstOrDefault(x => x.Id == clientId);
            if(client != null)
            {
                _hub.Clients.Client(client.ConnectionContext).ackNewOrder();
            }
        }
        #endregion

        public void DisposeConnection()
        {
            if(_webApp != null)
            {
                Beursfuif.Server.Services.Startup.DependencyResolver.Dispose();
                _webApp.Dispose();
            }
            BeursfuifHub.NewPackageReceived -= BeursfuifHub_NewPackageReceived;
        }

        public async Task<bool> Start()
        {
            return await Task.Factory.StartNew<bool>(InitServer);
        }



    }
}
