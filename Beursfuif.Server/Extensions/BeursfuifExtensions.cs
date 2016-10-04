using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Beursfuif.Server.Entity;

namespace Beursfuif.BL.Extensions
{
    public static class BeursfuifExtensions
    {
        public static ClientInterval ToClientInterval(this Interval interval, DateTime currentTime, string assetDirectory)
        {
            return new ClientInterval()
            {
                ClientDrinks = interval.Drinks.ToClientDrinkArray(interval.Id, assetDirectory),
                CurrentTime = currentTime,
                Start = interval.StartTime,
                Id = interval.Id,
                End = interval.EndTime
            };
        }

        public static ClientDrink[] ToClientDrinkArray(this Drink[] drinks, int intervalId, string assetDirectory)
        {
            var availableDrinks = drinks.Where(x => x.Available);
            List<ClientDrink> clDrinks = new List<ClientDrink>();
            foreach (Drink item in availableDrinks)
            {
                clDrinks.Add(item.ToClientDrink(intervalId, assetDirectory));
            }
            return clDrinks.ToArray();
        }

        public static ClientDrink ToClientDrink(this Drink drink, int intervalId, string assetDirectory)
        {
            ClientDrink clientDrink = new ClientDrink()
            {
                DrinkId = drink.Id,
                IntervalId = intervalId,
                Name = drink.Name,
                Price = drink.CurrentPrice,
                Base64Image = CreateBase64Image(assetDirectory + drink.ImageString)
            };
            return clientDrink;
        }

        private static string CreateBase64Image(string file)
        {
            if (string.IsNullOrEmpty(file)) return "";
            if (!File.Exists(file)) return "";

            
            byte[] imageBytes = File.ReadAllBytes(file);
            string baseEncodedImage = Convert.ToBase64String(imageBytes);
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
            return String.Join("\n\r", (from clientDrink in clientDrinkOrders
                                        join drink in drinks
                                        on clientDrink.DrinkId equals drink.Id
                                        select clientDrink.Count + " X " + drink.Name).ToArray());
        }

        public static string GetDescription(this PriceFactor value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr == null)
                    {
                        return name;
                    }
                    else
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }



    }
}
