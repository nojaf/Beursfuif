﻿using Beursfuif.BL;
using Beursfuif.Server.Messages;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Beursfuif.BL.Extensions;
using Beursfuif.Server.Entity;
using GalaSoft.MvvmLight.Command;
using Beursfuif.Server.Services;

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

        /// <summary>
        /// Sets and gets the MyProperty property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Client> Clients
        {
            get
            {
                return _beursfuifData.Clients;
            }

            set
            {
                if (_beursfuifData.Clients == value)
                {
                    return;
                }

                RaisePropertyChanging(ClientsPropertyName);
                _beursfuifData.Clients = value;
                RaisePropertyChanged(ClientsPropertyName);
            }
        }

        public RelayCommand<Guid> KickClientCommand { get; set; }
        #endregion

        public ClientsViewModel(IBeursfuifServer server, IBeursfuifData data):base(data)
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
                _server = server;
                InitServer();
                InitCommands();
                InitMessages();
                _beursfuifData.BeursfuifBusyChanged += BeursfuifData_BeursfuifBusyChanged;
            }

        }

        void BeursfuifData_BeursfuifBusyChanged(object sender, bool e)
        {
            if (!e && Clients.Any())
            {
                //party isn't on going and there are connected clients,
                //kick them
                KickAll(KickWasKickedReason.PARTY_PAUSED);            
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

                //Remove client anyway
                //If he fails to respond 
                _beursfuifData.Clients.Remove(client);
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
            Action action = () =>
            {
                foreach (var client in Clients)
                {
                    KickClient(new KickClientMessage()
                    {
                        ClientId = client.Id,
                        Reason = kickWasKickedReason
                    });
                }
                Clients.Clear();
            };

            if (App.Current != null && App.Current.Dispatcher != null)
            {
                App.Current.Dispatcher.BeginInvoke(action);
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
            if (_beursfuifData.AuthenticationString() != e.AuthCode)
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
                    RaisePropertyChanged(ClientsPropertyName);
                }));
            }
        }

        void Server_NewOrderEvent(object sender, BL.Event.NewOrderEventArgs e)
        {
            PointInCode("ClientsViewModel: Server_NewOrderEvent");
            Client c = Clients.FirstOrDefault(x => x.Id == e.ClientId);
            string authString = _beursfuifData.AuthenticationString();
            if (c != null && e.AuthenticationCode == authString)
            {
                string msg = $"{c.Name}: {e.Order.TotalPrice(_beursfuifData.CurrentInterval)} bons";
                SendToastMessage("Nieuwe bestelling ontvangen",msg);
                SendLogMessage("New order: " + msg, LogType.FROM_CLIENT | LogType.CLIENT_VM);
                c.OrderCount++;
                _server.SendAckNewOrder(e.ClientId, authString, _beursfuifData.BeursfuifCurrentTime);
                ClientResponded(e.ClientId);
            }
        }

        void Server_NewClientEventHandler(object sender, BL.Event.NewClientEventArgs e)
        {
            PointInCode("ClientsViewModel: Server_NewClientEventHandler");
            RaisePropertyChanged(ClientsPropertyName);

            SendToastMessage("New client connected", e.Name + " heeft zich aangemeld.");
            SendLogMessage("New client connected: " + e.Name + " heeft zich aangemeld.", LogType.CLIENT_VM | LogType.FROM_CLIENT);

        }

        void Server_IntervalUpdateAckEvent(object sender, BL.Event.BasicAuthAckEventArgs e)
        {
            PointInCode("ClientsViewModel: Server_IntervalUpdateAckEvent");           
            if (_beursfuifData.AuthenticationString() != e.AuthCode)
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

        #region Helper methoder

        private string GetClientName(Guid id)
        {
            Client client = Clients.FirstOrDefault(x => x.Id == id);
            if(client != null) return client.Name;
            return null;
        }

        private void ClientResponded(Guid id)
        {
            Client responder = Clients.FirstOrDefault(x => x.Id == id);
            if (responder != null) responder.LastActivity = _beursfuifData.BeursfuifCurrentTime;
        }
        #endregion

    }
}
