using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    public class PredictDrink
    {
        public int DrinkId { get; set; }
        public byte CurrentPrice { get; set; }
        public byte NextPrice { get; set; }
        public string Name { get; set; }
        public sbyte Addition { get; set; }
    }
}
