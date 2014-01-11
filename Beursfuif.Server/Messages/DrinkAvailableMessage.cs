using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.Messages
{
   public class DrinkAvailableMessage
    {
        public int DrinkId { get; set; }
        public bool Available { get; set; }
    }
}
