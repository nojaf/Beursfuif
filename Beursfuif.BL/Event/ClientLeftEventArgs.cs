using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL.Event
{
    public class ClientLeftEventArgs:EventArgs
    {
        public Guid ClientId { get; set; }
        public ClientLeftEventArgs(Guid clientId)
        {
            ClientId = clientId;
        }
    }
}
