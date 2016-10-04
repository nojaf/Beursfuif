using Beursfuif.BL;
using Beursfuif.BL.Event;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Beursfuif.Server.Entity;

namespace Beursfuif.Server.Services
{
    public interface IBeursfuifServer
    {
        event EventHandler<NewClientEventArgs> NewClientEvent;
        event EventHandler<NewOrderEventArgs> NewOrderEvent;
        event EventHandler<ClientLeftEventArgs> ClientLeftEvent;
        event EventHandler<BasicAuthAckEventArgs> CurrentTimeAckEvent;
        event EventHandler<BasicAuthAckEventArgs> IntervalUpdateAckEvent;

        ObservableCollection<Client> Clients { get; set; }

        int Port { get; }

        bool Active { get; set; }

        Task<bool> Start();

        void SendAckInitialClientConnect(ClientInterval currentInterval, Guid clientId, DateTime currentBeursfuifTime);

        void KickClient(Guid id);

        void UpdateTime(DateTime currentTime, string authenticationCode);

        void UpdateInterval(ClientInterval clientInterval, DateTime currentBFTime);

        void SendDrinkAvailableChanged(ClientInterval currentInterval);

        void SendAckNewOrder(Guid clientId, string authCode, DateTime currentBeursfuifTime);

        void DisposeConnection();

    }
}
