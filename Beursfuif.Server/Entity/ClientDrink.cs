namespace Beursfuif.Server.Entity
{
    public class ClientDrink
    {
        #region properties

        public int IntervalId { get; set; }

        public int DrinkId { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public string Base64Image { get; set; }

        #endregion

        public ClientDrink()
        {

        }

        public override string ToString()
        {
            return $"[ClientDrink]:[{DrinkId},{Name},{Price}]";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClientDrink) obj);
        }

        protected bool Equals(ClientDrink other)
        {
            return IntervalId == other.IntervalId && DrinkId == other.DrinkId && string.Equals(Name, other.Name) && Price == other.Price;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IntervalId;
                hashCode = (hashCode*397) ^ DrinkId;
                hashCode = (hashCode*397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ Price;
                return hashCode;
            }
        }
    }
}
