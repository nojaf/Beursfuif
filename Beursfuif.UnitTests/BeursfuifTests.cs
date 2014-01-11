using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Beursfuif.BL;
using System.Collections.Generic;
using Beursfuif.Server.ViewModel;
using System.Linq;

namespace Beursfuif.UnitTests
{
    [TestClass]
    public class BeursfuifTests
    {
        [TestMethod]
        public void TestPriceCalculation()
        {
            Drink[] drinks = GenerateDrinks();
            Interval[] intervals = GenerateIntervals(drinks);
            List<ClientDrinkOrder> orders = GenerateListOfOrders();
            
            Interval secondInterval = SettingsViewModel.CalculatePriceUpdates(orders, intervals, 1, false, new BeursfuifViewModelBase());
            Interval thirdInterval = SettingsViewModel.CalculatePriceUpdates(orders, intervals, 2, false, new BeursfuifViewModelBase());
            Interval fourthInterval = SettingsViewModel.CalculatePriceUpdates(orders, intervals, 3, false, new BeursfuifViewModelBase());
            Interval fifthInterval = SettingsViewModel.CalculatePriceUpdates(orders, intervals, 4, false, new BeursfuifViewModelBase());
            Interval sixthInterval = SettingsViewModel.CalculatePriceUpdates(orders, intervals, 5, false, new BeursfuifViewModelBase());
            
            //second Interval, everything should be the same as the first interval
            for (int i = 0; i < drinks.Length; i++)
            {
                Assert.AreEqual(secondInterval.Drinks[i].CurrentPrice, intervals[0].Drinks[i].CurrentPrice);
            }
           
            //third Interval
            Assert.AreEqual(13,thirdInterval.Drinks[0].CurrentPrice);
            Assert.AreEqual(24, thirdInterval.Drinks[1].CurrentPrice);
            Assert.AreEqual(17, thirdInterval.Drinks[2].CurrentPrice);
            Assert.AreEqual(18, thirdInterval.Drinks[3].CurrentPrice);
            Assert.AreEqual(17, thirdInterval.Drinks[4].CurrentPrice);
            Assert.AreEqual(15, thirdInterval.Drinks[5].CurrentPrice);
            Assert.AreEqual(13, thirdInterval.Drinks[6].CurrentPrice);
            Assert.AreEqual(16, thirdInterval.Drinks[7].CurrentPrice);
            Assert.AreEqual(13, thirdInterval.Drinks[8].CurrentPrice);
            Assert.AreEqual(22, thirdInterval.Drinks[9].CurrentPrice);
            Assert.AreEqual(22, thirdInterval.Drinks[10].CurrentPrice);
                
            //fourth
            Assert.AreEqual(15, fourthInterval.Drinks[0].CurrentPrice);
            Assert.AreEqual(27, fourthInterval.Drinks[1].CurrentPrice);
            Assert.AreEqual(15, fourthInterval.Drinks[2].CurrentPrice);
            Assert.AreEqual(20, fourthInterval.Drinks[3].CurrentPrice);
            Assert.AreEqual(15, fourthInterval.Drinks[4].CurrentPrice);
            Assert.AreEqual(13, fourthInterval.Drinks[5].CurrentPrice);
            Assert.AreEqual(15, fourthInterval.Drinks[6].CurrentPrice);
            Assert.AreEqual(18, fourthInterval.Drinks[7].CurrentPrice);
            Assert.AreEqual(15, fourthInterval.Drinks[8].CurrentPrice);
            Assert.AreEqual(25, fourthInterval.Drinks[9].CurrentPrice);
            Assert.AreEqual(25, fourthInterval.Drinks[10].CurrentPrice);

            //fifth
            Assert.AreEqual(13, fifthInterval.Drinks[0].CurrentPrice);
            Assert.AreEqual(30, fifthInterval.Drinks[1].CurrentPrice);
            Assert.AreEqual(13, fifthInterval.Drinks[2].CurrentPrice);
            Assert.AreEqual(22, fifthInterval.Drinks[3].CurrentPrice);
            Assert.AreEqual(17, fifthInterval.Drinks[4].CurrentPrice);
            Assert.AreEqual(15, fifthInterval.Drinks[5].CurrentPrice);
            Assert.AreEqual(17, fifthInterval.Drinks[6].CurrentPrice);
            Assert.AreEqual(20, fifthInterval.Drinks[7].CurrentPrice);
            Assert.AreEqual(17, fifthInterval.Drinks[8].CurrentPrice);
            Assert.AreEqual(28, fifthInterval.Drinks[9].CurrentPrice);
            Assert.AreEqual(28, fifthInterval.Drinks[10].CurrentPrice);

            //sixth
            Assert.AreEqual(12, sixthInterval.Drinks[0].CurrentPrice);
            Assert.AreEqual(32, sixthInterval.Drinks[1].CurrentPrice);
            Assert.AreEqual(14, sixthInterval.Drinks[2].CurrentPrice);
            Assert.AreEqual(24, sixthInterval.Drinks[3].CurrentPrice);
            Assert.AreEqual(18, sixthInterval.Drinks[4].CurrentPrice);
            Assert.AreEqual(16, sixthInterval.Drinks[5].CurrentPrice);
            Assert.AreEqual(18, sixthInterval.Drinks[6].CurrentPrice);
            Assert.AreEqual(22, sixthInterval.Drinks[7].CurrentPrice);
            Assert.AreEqual(16, sixthInterval.Drinks[8].CurrentPrice);
            Assert.AreEqual(30, sixthInterval.Drinks[9].CurrentPrice);
            Assert.AreEqual(30, sixthInterval.Drinks[10].CurrentPrice);

        }

        private List<ClientDrinkOrder> GenerateListOfOrders()
        {
            List<ClientDrinkOrder> orders = new List<ClientDrinkOrder>
            {
                new ClientDrinkOrder(){
                    Count = 43,
                    IntervalId  = 1,
                    DrinkId = 1
                },
                new ClientDrinkOrder(){
                    Count = 3,
                    IntervalId = 1,
                    DrinkId = 2
                },
                new ClientDrinkOrder(){
                    Count = 4,
                    IntervalId = 1,
                    DrinkId = 3
                },
                new ClientDrinkOrder(){
                    Count = 9,
                    IntervalId = 1,
                    DrinkId = 6
                },
                new ClientDrinkOrder(){
                    Count = 6,
                    IntervalId = 1,
                    DrinkId = 7
                },
                new ClientDrinkOrder(){
                    Count = 4,
                    IntervalId = 1,
                    DrinkId = 8
                },
                new ClientDrinkOrder(){
                    Count = 2,
                    IntervalId = 1,
                    DrinkId = 9
                },
                new ClientDrinkOrder(){
                    Count = 6,
                    IntervalId = 1,
                    DrinkId = 10
                },
                new ClientDrinkOrder(){
                    Count = 4,
                    IntervalId = 1,
                    DrinkId = 11
                },
                //second interval
                new ClientDrinkOrder(){
                    Count = 37,
                    IntervalId = 2,
                    DrinkId = 1
                },
                new ClientDrinkOrder(){
                    Count = 4,
                    IntervalId = 2,
                    DrinkId = 3
                },
                new ClientDrinkOrder(){
                    Count = 7,
                    IntervalId = 2,
                    DrinkId = 6
                },
                new ClientDrinkOrder(){
                    Count = 5,
                    IntervalId = 2,
                    DrinkId = 7
                },
                new ClientDrinkOrder(){
                    Count = 2,
                    IntervalId = 2,
                    DrinkId = 8
                },
                new ClientDrinkOrder(){
                    Count = 1,
                    IntervalId = 2,
                    DrinkId = 9
                },
                new ClientDrinkOrder(){
                    Count = 6,
                    IntervalId = 2,
                    DrinkId = 10
                },
                new ClientDrinkOrder(){
                    Count = 4,
                    IntervalId = 2,
                    DrinkId = 11
                },
                //third interval
                new ClientDrinkOrder(){
                    Count = 78,
                    IntervalId = 3,
                    DrinkId = 1
                },
                new ClientDrinkOrder(){
                    Count = 3,
                    IntervalId = 3,
                    DrinkId = 2
                },
                new ClientDrinkOrder(){
                    Count = 6,
                    IntervalId = 3,
                    DrinkId = 3
                },
                new ClientDrinkOrder(){
                    Count = 2,
                    IntervalId = 3,
                    DrinkId = 4
                },
                new ClientDrinkOrder(){
                    Count = 9,
                    IntervalId = 3,
                    DrinkId = 6
                },
                new ClientDrinkOrder(){
                    Count = 13,
                    IntervalId = 3,
                    DrinkId = 7
                },
                new ClientDrinkOrder(){
                    Count = 4,
                    IntervalId = 3,
                    DrinkId = 8
                },
                new ClientDrinkOrder(){
                    Count = 2,
                    IntervalId = 3,
                    DrinkId = 9
                },
                new ClientDrinkOrder(){
                    Count = 2,
                    IntervalId = 3,
                    DrinkId = 10
                },
                new ClientDrinkOrder(){
                    Count = 1,
                    IntervalId = 3,
                    DrinkId = 11
                },
                //fourth interval
                new ClientDrinkOrder(){
                    Count = 91,
                    IntervalId = 4,
                    DrinkId = 1
                },
                new ClientDrinkOrder(){
                    Count = 6,
                    IntervalId = 4,
                    DrinkId = 2
                },
                new ClientDrinkOrder(){
                    Count = 7,
                    IntervalId =4,
                    DrinkId = 3
                },
                new ClientDrinkOrder(){
                    Count = 6,
                    IntervalId = 4,
                    DrinkId = 4
                },
                new ClientDrinkOrder(){
                    Count = 2,
                    IntervalId = 4,
                    DrinkId = 5
                },
                new ClientDrinkOrder(){
                    Count = 14,
                    IntervalId = 4,
                    DrinkId = 6
                },
                new ClientDrinkOrder(){
                    Count = 9,
                    IntervalId = 4,
                    DrinkId = 7
                },
                new ClientDrinkOrder(){
                    Count = 9,
                    IntervalId = 4,
                    DrinkId = 8
                },
                new ClientDrinkOrder(){
                    Count = 3,
                    IntervalId = 4,
                    DrinkId = 10
                },
               //fifth interval
               new ClientDrinkOrder(){
                   Count = 87,
                   IntervalId = 5,
                   DrinkId = 1,
               },
               new ClientDrinkOrder(){
                   Count = 2,
                   IntervalId = 5,
                   DrinkId = 2
               },
               new ClientDrinkOrder(){
                   Count = 12,
                   IntervalId = 5,
                   DrinkId = 3
               },
               new ClientDrinkOrder(){
                   Count = 2,
                   IntervalId = 5,
                   DrinkId = 4
               },
               new ClientDrinkOrder(){
                   Count = 2,
                   IntervalId = 5,
                   DrinkId = 5
               },
               new ClientDrinkOrder(){
                   Count = 7,
                   IntervalId = 5,
                   DrinkId = 6
               },
               new ClientDrinkOrder(){
                   Count = 1,
                   IntervalId = 5,
                   DrinkId = 7
               },
               new ClientDrinkOrder(){
                   Count = 6,
                   IntervalId = 5,
                   DrinkId = 10
               },
               new ClientDrinkOrder(){
                   Count = 4,
                   IntervalId = 5,
                   DrinkId = 11
               }
            };
            return orders;
        }
 
        private Interval[] GenerateIntervals(Drink[] drinks)
        {
            Interval[] intervals = new Interval[6];
            for (int i = 0; i < 6; i++)
            {
                intervals[i] = new Interval() { Id = i + 1 };
                intervals[i].Drinks = new Drink[drinks.Length];
                for (int j = 0; j < drinks.Length; j++)
                {
                    intervals[i].Drinks[j] = drinks[j].Clone();
                }
            }
            return intervals;
        }

        private Drink[] GenerateDrinks()
        {
            Drink[] drinks = new Drink[]
            {
                new Drink(){
                    Name = "Bier",
                    InitialPrice = 14,
                    Id = 1,
                },
                new Drink(){
                    Name = "St-Bernardus Triple",
                    InitialPrice = 22,
                    Id = 2
                },
                new Drink(){
                    Name = "Kriek",
                    InitialPrice = 16,
                    Id = 3
                },
                new Drink(){
                    Name = "Hommel",
                    InitialPrice = 20,
                    Id = 4
                },
                new Drink(){
                    Name = "Keizer Karel",
                    InitialPrice = 18,
                    Id = 5
                },
                new Drink(){
                    Name = "Cola",
                    InitialPrice = 14,
                    Id = 6
                },
                new Drink(){
                    Name = "Limonade",
                    InitialPrice = 14,
                    Id = 7
                },
                new Drink(){
                    Name = "Ice-Tea",
                    InitialPrice = 15,
                    Id = 8
                },
                new Drink(){
                    Name = "Water",
                    InitialPrice = 12,
                    Id = 9
                },
                new Drink(){
                    Name = "Witte Wijn",
                    InitialPrice = 20,
                    Id = 10
                },
                new Drink(){
                    Name = "Rosé",
                    InitialPrice = 20,
                    Id = 11
                }
            };

            foreach (Drink drink in drinks)
            {
                drink.Available = true;
                drink.CurrentPrice = drink.InitialPrice;
                drink.MiniumPrice = 7;
                drink.MaximumPrice = 37;
                drink.OverrideFactor = 0;
                drink.BigDecrease = 0.88;
                drink.SmallDecrease = 0.92;
                drink.BigRise = 1.12;
                drink.SmallRise = 1.08;
            }
            return drinks;
        }
    }
}

/*Bier	1.4
St-Bernardus Triple	2.2
Kriek	1.6
Hommel	2
Keizer Karel	1.8
Cola	1.4
Limonade	1.4
Ice-Tea	1.5
Water	1.2
Witte Wijn	2
Rosé	2*/
