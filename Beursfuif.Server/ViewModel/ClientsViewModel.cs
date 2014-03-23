using Beursfuif.BL;
using Beursfuif.Server.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Beursfuif.BL.Extensions;
using GalaSoft.MvvmLight.Command;
using Beursfuif.Server.Services;
using Beursfuif.Server.DataAccess;

namespace Beursfuif.Server.ViewModel
{
    public class ClientsViewModel : BeursfuifViewModelBase
    {
        #region Fields and props
        private IBeursfuifServer _server;

        /// <summary>
        /// The <see cref="MyProperty" /> property's name.
        /// </summary>
        public const string ClientsPropertyName = "Clients";

        private ObservableCollection<Client> _clients = null;

        /// <summary>
        /// Sets and gets the MyProperty property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Client> Clients
        {
            get
            {
                return _clients;
            }

            set
            {
                if (_clients == value)
                {
                    return;
                }

                RaisePropertyChanging(ClientsPropertyName);
                _clients = value;
                RaisePropertyChanged(ClientsPropertyName);
            }
        }

        public RelayCommand<Guid> KickClientCommand { get; set; }
        #endregion

        public ClientsViewModel(IBeursfuifServer server)
        {
            if (IsInDesignMode)
            {
                #region DummyData
                Clients = new ObservableCollection<Client>()
                {

                    new Client(){
                        Id = Guid.NewGuid(),
                        Ip = "192.168.1.101",
                        LastActivity = DateTime.Now,
                        Name = "Gilles",
                        OrderCount = 5
                    },
                    new Client(){
                        Id = Guid.NewGuid(),
                        Ip = "192.168.1.102",
                        LastActivity = DateTime.Now.Subtract(new TimeSpan(0,12,0)),
                        Name = "Jurgen",
                        OrderCount = 2
                    },
                    new Client(){
                        Id = Guid.NewGuid(),
                        Ip = "192.168.1.103",
                        LastActivity = DateTime.Now.Subtract(new TimeSpan(0,6,0)),
                        Name = "Florejan",
                        OrderCount = 14
                    },
                    new Client(){
                        Id = Guid.NewGuid(),
                        Ip = "192.168.1.104",
                        LastActivity = DateTime.Now.Subtract(new TimeSpan(0,2,0)),
                        Name = "Wouter",
                        OrderCount = 4
                    }

                };
                #endregion
            }
            else
            {
                PointInCode("ClientsViewModel: Ctor");
                Clients = new ObservableCollection<Client>();
                _server = server;
                InitServer();
                InitCommands();
                InitMessages();
            }

        }

        #region Commands
        private void InitCommands()
        {
            PointInCode("ClientsViewModel: InitCommands");
            KickClientCommand = new RelayCommand<Guid>(KickClientHandler);
        }

        private void KickClientHandler(Guid id)
        {
            PointInCode("ClientsViewModel: KickClientHandler");
            Client client = Clients.FirstOrDefault(x => x.Id == id);
            if (client != null)
            {
                _server.KickClient(id);
                SendLogMessage("Client kicked: " + client.Name, LogType.CLIENT_VM);
            }
        }
        #endregion

        #region Messages
        private void InitMessages()
        {
            PointInCode("ClientsViewModel: InitMessages");
            MessengerInstance.Register<KickClientMessage>(this, KickClient);
        }

        private void KickClient(KickClientMessage msg)
        {
            PointInCode("ClientsViewModel: KickClient");
            Client client = Clients.FirstOrDefault(x => x.Id == msg.ClientId);
            if (client != null)
            {
                _server.KickClient(client.Id);
                SendToastMessage("Client was kicked",client.Name + " was kicked because " + msg.Reason);
            }
        }

        public void KickAll(KickWasKickedReason kickWasKickedReason)
        {
            PointInCode("ClientsViewModel: KickAll");
            foreach (var client in Clients)
            {
                KickClient(new KickClientMessage()
                {
                    ClientId = client.Id,
                    Reason = kickWasKickedReason
                });
            }
        }
        #endregion

        #region Server
        private void InitServer()
        {
            PointInCode("ClientsViewModel: InitServer");
            _server.NewClientEvent += Server_NewClientEventHandler;
            _server.NewOrderEvent += Server_NewOrderEvent;
            _server.ClientLeftEvent += Server_ClientLeftEvent;
            _server.CurrentTimeAckEvent += Server_CurrentTimeAckEvent;
            _server.IntervalUpdateAckEvent += Server_IntervalUpdateAckEvent;
        }

        void Server_CurrentTimeAckEvent(object sender, BL.Event.BasicAuthAckEventArgs e)
        {
            PointInCode("ClientsViewModel: Server_CurrentTimeAckEvent");
            if (GetCurrentInterval().AuthenticationString() != e.AuthCode)
            {
                KickClient(new KickClientMessage()
                {
                    ClientId = e.ClientId,
                    Reason = KickWasKickedReason.WRONG_AUTH_CODE
                });
                SendLogMessage("Client " + GetClientName(e.ClientId) +
                    " kicked because wrong auth code (" + e.AuthCode + ") at time ack", LogType.CLIENT_SERVER_ERROR | LogType.CLIENT_VM);
                return;
            }
            //else
            ClientResponded(e.ClientId);
            SendLogMessage(GetClientName(e.ClientId) + " has replied to the current time update", LogType.CLIENT_VM | LogType.GOOD_NEWS | LogType.FROM_CLIENT);
        }

        void Server_ClientLeftEvent(object sender, BL.Event.ClientLeftEventArgs e)
        {
            PointInCode("ClientsViewModel: Server_ClientLeftEvent");
            Client c = Clients.FirstOrDefault(x => x.Id == e.ClientId);
            if (c != null)
            {
                SendToastMessage("Client left", c.Name + " heeft de server verlaten.");
                SendLogMessage("Client left: "+ c.Name, LogType.FROM_CLIENT | LogType.CLIENT_VM);
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Clients.Remove(c);
                }));
            }
        }

        void Server_NewOrderEvent(object sender, BL.Event.NewOrderEventArgs e)
        {
            PointInCode("ClientsViewModel: Server_NewOrderEvent");
            Client c = Clients.FirstOrDefault(x => x.Id == e.ClientId);
            string authString = GetCurrentInterval().AuthenticationString();
            if (c != null && e.AuthenticationCode == authString)
            {
                string msg = c.Name + ": " + e.Order.TotalPrice(GetCurrentInterval()) + " bons.";
                SendToastMessage("Nieuwe bestelling ontvangen",msg);
                SendLogMessage("New order: " + msg, LogType.FROM_CLIENT | LogType.CLIENT_VM);
                c.OrderCount++;
                _server.SendAckNewOrder(e.ClientId, authString, GetCurrentBeursfuifTime());
                ClientResponded(e.ClientId);
            }
        }

        void Server_NewClientEventHandler(object sender, BL.Event.NewClientEventArgs e)
        {
            PointInCode("ClientsViewModel: Server_NewClientEventHandler");
            DateTime currentBFTime = GetCurrentBeursfuifTime();

            Action action = delegate()
            {
                Clients.Add(new Client()
                {
                    Id = e.Id,
                    Ip = e.Ip,
                    LastActivity = currentBFTime,
                    Name = e.Name,
                    OrderCount = 0
                });
            };

            App.Current.Dispatcher.BeginInvoke(action, System.Windows.Threading.DispatcherPriority.Normal);
            SendToastMessage("New client connected", e.Name + " heeft zich aangemeld.");
            SendLogMessage("New client connected: "+e.Name + " heeft zich aangemeld.", LogType.CLIENT_VM | LogType.FROM_CLIENT);

            Interval currentInterval = GetCurrentInterval();

            _server.SendAckInitialClientConnect(currentInterval.ToClientInterval(currentBFTime, PathManager.ASSETS_PATH), e.Id, currentBFTime);
            SendLogMessage("Repley on " + e.Name + "'s connection request", LogType.SETTINGS_VM);
        }

        void Server_IntervalUpdateAckEvent(object sender, BL.Event.BasicAuthAckEventArgs e)
        {
            PointInCode("ClientsViewModel: Server_IntervalUpdateAckEvent");           
            if (GetCurrentInterval().AuthenticationString() != e.AuthCode)
            {
                KickClient(new KickClientMessage()
                {
                    ClientId = e.ClientId,
                    Reason = KickWasKickedReason.WRONG_AUTH_CODE
                });
                SendLogMessage("Client " + GetClientName(e.ClientId) +
                    " kicked because wrong auth code ("+e.AuthCode+") after receiving interval update", LogType.CLIENT_SERVER_ERROR | LogType.CLIENT_VM);
                return;
            }
            //else
            ClientResponded(e.ClientId);
            SendLogMessage(GetClientName(e.ClientId) + " has replied to the current interval update", LogType.CLIENT_VM | LogType.GOOD_NEWS | LogType.FROM_CLIENT);
        }
        #endregion

        #region Helper methodes
        private Interval GetCurrentInterval()
        {
            var locator = base.GetLocator();
            return locator.Settings.CurrentInterval;
        }

        private string GetClientName(Guid id)
        {
            Client client = Clients.FirstOrDefault(x => x.Id == id);
            if(client != null) return client.Name;
            return null;
        }

        private DateTime GetCurrentBeursfuifTime()
        {
            return base.GetLocator().Settings.BeursfuifCurrentTime;
        }

        private void ClientResponded(Guid id)
        {
            Client responder = Clients.FirstOrDefault(x => x.Id == id);
            if (responder != null) responder.LastActivity = GetCurrentBeursfuifTime();
        }
        #endregion

    }
}
