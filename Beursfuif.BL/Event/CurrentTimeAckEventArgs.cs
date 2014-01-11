using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL.Event
{
    public class BasicAuthAckEventArgs:EventArgs
    {
        public int ClientId { get; set; }
        public string  AuthCode { get; set; }

        public BasicAuthAckEventArgs(int clientId, string authCode)
        {
            ClientId = clientId;
            AuthCode = authCode;
        }
    }
}
