using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    [Serializable]
    public class ClientDrinkOrder
    {
        public int DrinkId { get; set; }

        public byte Count { get; set; }

        public int IntervalId { get; set; }

        public ClientDrinkOrder()
        {

        }

        public override string ToString()
        {
            return string.Format("[ClientOrder]:[{0} x {1} in {2}]", DrinkId, Count,IntervalId);
        }

        public override bool Equals(object obj)
        {
            if (obj is ClientDrinkOrder)
            {
                ClientDrinkOrder other = obj as ClientDrinkOrder;
                if (other.DrinkId == this.DrinkId && other.Count == this.Count && other.IntervalId == this.IntervalId) return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
             return base.GetHashCode();
        }
    }
}
