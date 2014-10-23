using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beursfuif.BL.Entity
{
    /// <summary>
    /// DTO to represent the amount the drink has been drunk in the selected interval
    /// </summary>
    public class DrinkStatistic
    {
        public int OrderCount { get; set; }

        public int DrinkId { get; set; }

        public string DrinkName { get; set; }

        public int Price { get; set; }

        public override string ToString()
        {
            if(Price == 0) return string.Format("{0} #{1}",DrinkName, OrderCount);
            return string.Format("{0} #{1} aan {2}", DrinkName, OrderCount, Price);
        }
    }
}
