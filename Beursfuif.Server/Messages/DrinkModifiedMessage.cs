using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.Messages
{
    public class DrinkModifiedMessage
    {
        public Drink Drink { get; set; }
        public bool Added { get; set; }
        public bool Changed { get; set; }
    }
}
