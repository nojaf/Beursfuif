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
            if(Price == 0) return $"{DrinkName} #{OrderCount}";
            return $"{DrinkName} #{OrderCount} aan {Price}";
        }
    }
}
