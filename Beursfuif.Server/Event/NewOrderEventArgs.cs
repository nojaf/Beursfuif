using System;
using Beursfuif.Server.Entity;

namespace Beursfuif.BL.Event
{
    public class NewOrderEventArgs:EventArgs
    {
        public Guid ClientId { get; set; }
        public ClientDrinkOrder[] Order { get; set; }
        public string AuthenticationCode { get; set; }

        public NewOrderEventArgs(Guid clientId, string auth, ClientDrinkOrder[] clientDrinkOrder)
        {
            this.ClientId = clientId;
            this.Order = clientDrinkOrder;
            this.AuthenticationCode = auth;
        }

    }
}
