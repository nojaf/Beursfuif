using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    public class ClientDrink
    {
        #region properties

        public int IntervalId { get; set; }

        public int DrinkId { get; set; }

        public string Name { get; set; }

        public byte Price { get; set; }

        #endregion

        public ClientDrink()
        {

        }

        public override string ToString()
        {
            return string.Format("[ClientDrink]:[{0},{1},{2}]",DrinkId,Name,Price);
        }

        public override bool Equals(object obj)
        {
            if (obj is ClientDrink)
            {
                ClientDrink other = obj as ClientDrink;
                if(other.DrinkId == this.DrinkId && other.IntervalId == this.IntervalId) return true;
            }
            return false;
        }
    }
}
