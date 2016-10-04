using System;

namespace Beursfuif.Server.Entity
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
            return $"[ClientOrder]:[{DrinkId} x {Count} in {IntervalId}]";
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
