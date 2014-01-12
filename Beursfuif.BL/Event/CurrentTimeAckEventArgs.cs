using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL.Event
{
    public class BasicAuthAckEventArgs:EventArgs
    {
        public Guid ClientId { get; set; }
        public string  AuthCode { get; set; }

        public BasicAuthAckEventArgs(Guid clientId, string authCode)
        {
            ClientId = clientId;
            AuthCode = authCode;
        }
    }
}
