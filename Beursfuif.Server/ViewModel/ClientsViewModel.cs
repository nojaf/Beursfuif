using Beursfuif.BL;
using Beursfuif.Server.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Beursfuif.BL.Extensions;
using GalaSoft.MvvmLight.Command;

namespace Beursfuif.Server.ViewModel
{
    public class ClientsViewModel : BeursfuifViewModelBase
    {
        private BeursfuifServer _server;

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

        public RelayCommand<int> KickClientCommand { get; set; }

        public ClientsViewModel(BeursfuifServer server)
        {


            if (IsInDesignMode)
            {
                #region DummyData
                Clients = new ObservableCollection<Client>()
                {

                    new Client(){
                        Id = 1,
                        Ip = "192.168.1.101",
                        LastActivity = DateTime.Now,
                        Name = "Gilles",
                        OrderCount = 5
                    },
                    new Client(){
                        Id = 2,
                        Ip = "192.168.1.102",
                        LastActivity = DateTime.Now.Subtract(new TimeSpan(0,12,0)),
                        Name = "Jurgen",
                        OrderCount = 2
                    },
                    new Client(){
                        Id = 3,
                        Ip = "192.168.1.103",
                        LastActivity = DateTime.Now.Subtract(new TimeSpan(0,6,0)),
                        Name = "Florejan",
                        OrderCount = 14
                    },
                    new Client(){
                        Id = 4,
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
                Clients = new ObservableCollection<Client>();
                _server = server;
                InitServer();
                InitCommands();
            }

        }

        private void InitCommands()
        {
            KickClientCommand = new RelayCommand<int>(KickClientHandler);
        }

        private void KickClientHandler(int id)
        {
            Client client = Clients.FirstOrDefault(x => x.Id == id);
            if (client != null)
            {
                _server.KickClient(id);
                SendLogMessage("Client kicked: " + client.Name, LogType.CLIENT_VM);
            }
        }

        private void InitServer()
        {
            _server.NewClientEvent += Server_NewClientEventHandler;
            _server.NewOrderEvent += Server_NewOrderEvent;
            _server.ClientLeftEvent += Server_ClientLeftEvent;
        }

        void Server_ClientLeftEvent(object sender, BL.Event.ClientLeftEventArgs e)
        {
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
            Client c = Clients.FirstOrDefault(x => x.Id == e.ClientId);
            if (c != null)
            {
                string msg = c.Name + ": " + e.Order.TotalPrice(GetCurrentInterval()) + " bongs.";
                SendToastMessage("Nieuwe bestelling ontvangen",msg);
                SendLogMessage("New order: " + msg, LogType.FROM_CLIENT | LogType.CLIENT_VM);
                c.LastActivity = base.GetLocator().Settings.BeursfuifCurrentTime;
                c.OrderCount++;
            }
        }

        void Server_NewClientEventHandler(object sender, BL.Event.NewClientEventArgs e)
        {
           
            Action action = delegate()
            {
                Clients.Add(new Client()
                {
                    Id = e.Id,
                    Ip = e.Ip,
                    LastActivity = base.GetLocator().Settings.BeursfuifCurrentTime,
                    Name = e.Name,
                    OrderCount = 0
                });
            };

            App.Current.Dispatcher.BeginInvoke(action, System.Windows.Threading.DispatcherPriority.Normal);
            SendToastMessage("New client connected", e.Name + " heeft zich aangemeld.");
            SendLogMessage("New client connected: "+e.Name + " heeft zich aangemeld.", LogType.CLIENT_VM | LogType.FROM_CLIENT);
        }

        private Interval GetCurrentInterval()
        {
            var locator = base.GetLocator();
            return locator.Settings.CurrentInterval;
        }

       
    }
}
