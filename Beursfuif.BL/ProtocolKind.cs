using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    public enum ProtocolKind
    {
        NEW_CLIENT_CONNECTS = 1,
        ACK_NEW_CLIENT_CONNECTS = 2,
        TIME_UPDATE = 3,
        ACK_TIME_UPDATE = 4,
        NEW_ORDER =5,
        ACK_NEW_ORDER = 6,
        UPDATE_CLIENT_INTERVAL = 7,
        ACK_UPDATE_CLIENT_INTERVAL = 8,
        RETRANSMIT_CLIENT_INTERVAL = 9,
        KICK_CLIENT = 10,
        CLIENT_LEAVES_SERVER = 11
    }
}
