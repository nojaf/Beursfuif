using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL.Event
{
    public class NewOrderEventArgs:EventArgs
    {
        public int ClientId { get; set; }
        public ClientDrinkOrder[] Order { get; set; }
        public string AuthenticationCode { get; set; }

        public NewOrderEventArgs(int clientId, string auth, ClientDrinkOrder[] clientDrinkOrder)
        {
            // TODO: Complete member initialization
            this.ClientId = clientId;
            this.Order = clientDrinkOrder;
            this.AuthenticationCode = auth;
        }

    }
}
