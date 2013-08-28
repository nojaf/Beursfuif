using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    public static class ProtocolKind
    {
        public const int NEW_CLIENT_CONNECTS = 1;
        public const int ACK_NEW_CLIENT_CONNECTS = 2;
        public const int TIME_UPDATE = 3;
        public const int ACK_TIME_UPDATE = 4;
        public const int NEW_ORDER = 5;
        public const int ACK_NEW_ORDER = 6;
        public const int UPDATE_CLIENT_INTERVAL = 7;
        public const int ACK_UPDATE_CLIENT_INTERVAL = 8;
        public const int RETRANSMIT_CLIENT_INTERVAL = 9;
        public const int KICK_CLIENT = 10;
        public const int CLIENT_LEAVES_SERVER = 11;
    }
}
