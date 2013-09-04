using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Beursfuif.BL.Extensions
{
    public static class BeursfuifExtensions
    {
        public static ClientInterval ToClientInterval(this Interval interval, DateTime currentTime)
        {
            return new ClientInterval()
            {
                ClientDrinks = interval.Drinks.ToClientDrinkArray(interval.Id),
                CurrentTime = currentTime,
                Start = interval.StartTime,
                Id = interval.Id,
                End = interval.EndTime
            };
        }

        public static ClientDrink[] ToClientDrinkArray(this Drink[] drinks, int intervalId)
        {
            int length = drinks.Length;
            ClientDrink[] clDrinks = new ClientDrink[length];
            for (int i = 0; i < length; i++)
            {
                clDrinks[i] = new ClientDrink()
                {
                    DrinkId = drinks[i].Id,
                    IntervalId = intervalId,
                    Price = drinks[i].CurrentPrice,
                    Name = drinks[i].Name,
                    Base64Image = CreateBase64Image(drinks[i].ImageString)
                };
            }
            return clDrinks;
        }

        private static string CreateBase64Image(string file)
        {
            Stream fs = File.Open(file, FileMode.Open);
            string baseEncodedImage = Convert.ToBase64String(ReadFully(fs));
            fs.Close();
            return baseEncodedImage;
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static int TotalPrice(this ClientDrinkOrder[] clientDrinkOrders, Interval currentInterval)
        {
            var query = from item in clientDrinkOrders
                        join drink in currentInterval.Drinks
                        on item.DrinkId equals drink.Id
                        select item.Count * drink.CurrentPrice;
            return query.Sum();           

        }

        public static string ToContentString(this ClientDrinkOrder[] clientDrinkOrders, ObservableCollection<Drink> drinks)
        {
            return  String.Join("\n\r", (from clientDrink in clientDrinkOrders
                                          join drink in drinks
                                          on clientDrink.DrinkId equals drink.Id
                                          select clientDrink.Count + " X " + drink.Name).ToArray());
        }
    }
}
