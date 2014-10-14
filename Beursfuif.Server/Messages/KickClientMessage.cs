using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.Messages
{
    public class KickClientMessage
    {
        public Guid ClientId { get; set; }
        public KickWasKickedReason Reason { get; set; }
    }

    public enum KickWasKickedReason
    {
        WRONG_AUTH_CODE = 1,
        TIME_OUT_ERROR = 2,
        I_DONT_LIKE_HIM = 4,
        END_OF_PARTY = 8,
        PARTY_PAUSED = 16
    }
}
