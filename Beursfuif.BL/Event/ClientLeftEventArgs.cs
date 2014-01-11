using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL.Event
{
    public class ClientLeftEventArgs:EventArgs
    {
        public int ClientId { get; set; }
        public ClientLeftEventArgs(int clientId)
        {
            ClientId = clientId;
        }
    }
}
