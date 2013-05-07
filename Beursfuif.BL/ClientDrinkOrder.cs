using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    public class ClientDrinkOrder
    {
        public int DrinkId { get; set; }

        public byte Count { get; set; }

        public ClientDrinkOrder()
        {

        }

        public override string ToString()
        {
            return string.Format("[ClientOrder]:[{0} x {1}]", DrinkId, Count);
        }

        public override bool Equals(object obj)
        {
            if (obj is ClientDrinkOrder)
            {
                ClientDrinkOrder other = obj as ClientDrinkOrder;
                if (other.DrinkId == this.DrinkId && other.Count == this.Count) return true;
            }
            return false;
        }
    }
}
