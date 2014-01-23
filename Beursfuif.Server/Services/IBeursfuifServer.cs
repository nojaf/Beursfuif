using Beursfuif.BL;
using Beursfuif.BL.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beursfuif.Server.Services
{
    public interface IBeursfuifServer
    {
        event EventHandler<NewClientEventArgs> NewClientEvent;
        event EventHandler<NewOrderEventArgs> NewOrderEvent;
        event EventHandler<ClientLeftEventArgs> ClientLeftEvent;
        event EventHandler<BasicAuthAckEventArgs> CurrentTimeAckEvent;
        event EventHandler<BasicAuthAckEventArgs> IntervalUpdateAckEvent;

        List<Client> Clients { get; set; }

        int Port { get; set; }

        string Ip { get; set; }

        bool Active { get; set; }

        Task<bool> Start(string ip, int port);

        void SendAckInitialClientConnect(ClientInterval currentInterval, Guid clientId, DateTime currentBeursfuifTime);

        void KickClient(Guid id);

        void UpdateTime(DateTime currentTime, string authenticationCode);

        void UpdateInterval(ClientInterval clientInterval, DateTime currentBFTime);

        void SendDrinkAvailableChanged(int drinkId, Drink drink, int intervalId);

        void SendAckNewOrder(Guid clientId, string authCode, DateTime currentBeursfuifTime);

        void DisposeConnection();

    }
}
