using Beursfuif.BL;
using Beursfuif.Server.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.ViewModel
{
    public class ClientsViewModel : BeursfuifViewModelBase
    {
        private BeursfuifServer _server;

        /// <summary>
        /// The <see cref="MyProperty" /> property's name.
        /// </summary>
        public const string ClientsPropertyName = "MyProperty";

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

        public ClientsViewModel(BeursfuifServer server)
        {
            _server = server;

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
            }

            InitServer();
        }

        private void InitServer()
        {
            _server.NewClientEvent += Server_NewClientEventHandler;
        }

        void Server_NewClientEventHandler(object sender, BL.Event.NewClientEventArgs e)
        {
            Action action = delegate(){
                Clients.Add(new Client()
                {
                    Id = e.Id,
                    Ip = e.Ip,
                    LastActivity = DateTime.Now,
                    Name = e.Name,
                    OrderCount = 0
                });
            };

            App.Current.Dispatcher.BeginInvoke(action, System.Windows.Threading.DispatcherPriority.Normal);
            MessengerInstance.Send<ToastMessage>(new ToastMessage("New client connected",e.Name + " heeft zich aangemeld."));
        }
    }
}
