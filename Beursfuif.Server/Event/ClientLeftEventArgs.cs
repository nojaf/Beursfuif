using System;

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
