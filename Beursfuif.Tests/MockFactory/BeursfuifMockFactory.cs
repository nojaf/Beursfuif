using Beursfuif.BL;
using System.Collections.ObjectModel;
using Beursfuif.Server.Entity;

namespace Beursfuif.Tests.MockFactory
{
    public class BeursfuifMockFactory
    {
        public ObservableCollection<Drink> MockDrinks()
        {
            ObservableCollection<Drink> drinks = new ObservableCollection<Drink>();

            Drink beer = new Drink()
            {
                Available = true,
                BigDecrease = 0.7,
                BigRise = 1.2,
                CurrentPrice = 13,
                Id = 1,
                InitialPrice = 13,
                MaximumPrice = 22,
                MiniumPrice = 10,
                Name = "Bier",
                OverrideFactor = 1,
                SmallDecrease = 0.9,
                SmallRise = 1.1
            };

            drinks.Add(beer);

            Drink water = new Drink()
            {
                Available = true,
                BigDecrease = 0.7,
                BigRise = 1.2,
                CurrentPrice = 10,
                Id = 1,
                InitialPrice = 10,
                MaximumPrice = 15,
                MiniumPrice = 10,
                Name = "Water",
                OverrideFactor = 1,
                SmallDecrease = 0.9,
                SmallRise = 1.1
            };

            drinks.Add(water);

            Drink coke = new Drink()
            {
                Available = true,
                BigDecrease = 0.6,
                BigRise = 1.1,
                CurrentPrice = 12,
                Id = 1,
                InitialPrice = 12,
                MaximumPrice = 20,
                MiniumPrice = 10,
                Name = "Water",
                OverrideFactor = 1,
                SmallDecrease = 0.9,
                SmallRise = 1.1
            };

            drinks.Add(coke);
            return drinks;
        }
    }
}
