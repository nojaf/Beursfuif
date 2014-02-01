using Beursfuif.BL;
using Beursfuif.Server.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beursfuif.Server.Services
{
    public static class PriceCalculation
    {
        public static Interval CalculatePriceUpdates(List<ClientDrinkOrder> allOrdersItems, Interval[] intervals, int idCurrentInterval, bool predict, BeursfuifViewModelBase viewmodel)
        {
            viewmodel.PointInCode("PriceCalculation: CalculatePriceUpdates");

            Interval currentInterval = intervals.FirstOrDefault(x => x.Id == idCurrentInterval);
            if (currentInterval == null) throw new Exception("Current interval isn't part of the Interval array");

            int currentIntervalIndex = Array.IndexOf(intervals, intervals.FirstOrDefault(x => x.Id == idCurrentInterval));
            //the first to intervals don't trigger an update
            if (currentIntervalIndex == 0) return intervals[1];

            Interval previousInterval = intervals[currentIntervalIndex - 1];
            //no new update possible, end of beursfuif
            if (currentIntervalIndex == intervals.Length - 1) return null;

            Interval nextInterval = (predict ? intervals[currentIntervalIndex + 1].Clone() : intervals[currentIntervalIndex + 1]);

            int previousAllDrinkCount = allOrdersItems.Where(x => x.IntervalId == previousInterval.Id).Sum(x => x.Count);
            viewmodel.SendLogMessage("Previous drink count: " + previousAllDrinkCount, LogType.SETTINGS_VM);
            int currentAllDrinkCount = allOrdersItems.Where(x => x.IntervalId == currentInterval.Id).Sum(x => x.Count);
            viewmodel.SendLogMessage("Current drink count: " + previousAllDrinkCount, LogType.SETTINGS_VM);

            //2 in the excel file
            int differenceAllDrinks = currentAllDrinkCount - previousAllDrinkCount;
            viewmodel.SendLogMessage("Current all drinkcount - Previous all drinkcount", LogType.SETTINGS_VM);

            Drink[] drinksForNextInterval = currentInterval.Drinks.Where(x => x.Available).ToArray();
            int numberOfDrinks = drinksForNextInterval.Length;
            for (int i = 0; i < numberOfDrinks; i++)
            {
                Drink drink = drinksForNextInterval[i];

                int previousDrinkCount = allOrdersItems.Where(x => x.DrinkId == drink.Id && x.IntervalId == previousInterval.Id).Sum(x => x.Count);
                int currentDrinkCount = allOrdersItems.Where(x => x.DrinkId == drink.Id && x.IntervalId == currentInterval.Id).Sum(x => x.Count);
                //1 in the excel
                int differenceDrinkCount = currentDrinkCount - previousDrinkCount;

                //3 in the excel
                double differenceProcentage = ((double)currentDrinkCount / (double)currentAllDrinkCount)
                                                                        -
                                               ((double)previousDrinkCount / (double)previousAllDrinkCount);

                if (differenceDrinkCount >= 0)
                {
                    #region 1A
                    //the drink has been drank more
                    if (differenceAllDrinks >= 0)
                    {
                        #region 2AA
                        //more drinks have been drunk in general
                        if (differenceProcentage > 0)
                        {
                            //the drink has been drunk more procentually
                            drink.PriceFactor = PriceFactor.BIG_RISE;
                        }
                        else
                        {
                            //the drink has been drunk less procentually
                            drink.PriceFactor = PriceFactor.BIG_DECREASE;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 2AB
                        //less drinks have been drunk in general
                        if (differenceProcentage > 0)
                        {
                            //the drink has been drunk more procentually
                            drink.PriceFactor = PriceFactor.SMALL_RISE;
                        }
                        else
                        {
                            //the drink has been drunk less procentually
                            drink.PriceFactor = PriceFactor.SMALL_DECREASE;
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 1B
                    //the drink has been drank less
                    if (differenceAllDrinks > 0)
                    {
                        #region 2BA
                        //more drinks have been drunk in general
                        if (differenceProcentage > 0)
                        {
                            //the drink has been drunk more procentually
                            drink.PriceFactor = PriceFactor.BIG_DECREASE;
                        }
                        else
                        {
                            //the drink has been drunk less procentually
                            drink.PriceFactor = PriceFactor.BIG_RISE;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 2BB
                        //more drinks have been drunk in general
                        if (differenceProcentage > 0)
                        {
                            //the drink has been drunk more procentually
                            drink.PriceFactor = PriceFactor.SMALL_DECREASE;
                        }
                        else
                        {
                            //the drink has been drunk less procentually
                            drink.PriceFactor = PriceFactor.SMALL_RISE;
                        }
                        #endregion
                    }
                    #endregion
                }

                //check if we need to use the override factor
                if (drink.OverrideFactor != 0 && !predict) drink.PriceFactor = PriceFactor.OVERRIDE;



                Drink nextDrink = nextInterval.Drinks.FirstOrDefault(x => x.Id == drink.Id);
                double priceFactor = drink.GetPriceFactorValue();
                double priceWithoutRouding = drink.CurrentPrice * priceFactor;
                byte nextPrice = (byte)Math.Round(priceWithoutRouding);
                if (nextPrice > nextDrink.MaximumPrice) nextPrice = nextDrink.MaximumPrice;
                if (nextPrice < nextDrink.MiniumPrice) nextPrice = nextDrink.MiniumPrice;

                nextDrink.CurrentPrice = nextPrice;
                if (predict) nextDrink.PriceFactor = drink.PriceFactor;
            }

            return nextInterval;
        }

    }
}
