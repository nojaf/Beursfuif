using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Beursfuif.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IOManager io = new IOManager();
            Interval[] intervals = io.LoadArrayFromBinary<Interval>(@"C:\Skydrive\Downloads\back up beursfuif afloop\Data\intervals.bin");
            List<Drink> drinks = io.LoadListFromXml<Drink>(@"C:\Skydrive\Downloads\back up beursfuif afloop\Data\drinks.xml");
            var orders = io.LoadObservableCollectionFromBinary<ShowOrder>(@"C:\Skydrive\Downloads\back up beursfuif afloop\Data\auto_saved_all_orders.bin");
            //using(StreamWriter sw = File.CreateText(@"C:\SkyDrive\Desktop\allBestellingen.csv"))
            //{
            //    sw.WriteLine("Tijdstip,Interval,Hoeveelheid,Drank,Prijs per eenheid");
            //    foreach (var item in orders)
            //    {
            //        foreach (var orderItem in item.Orders)
            //        {
            //            Interval interval = intervals.FirstOrDefault(x => x.Id == orderItem.IntervalId);
            //            Drink drink = interval.Drinks.FirstOrDefault(x => x.Id == orderItem.DrinkId);
            //            sw.WriteLine(item.Time.ToShortTimeString() + "," + interval.StartTime.ToShortTimeString() + "-" + interval.EndTime.ToShortTimeString() + "," + orderItem.Count + "," + drink.Name + "," + drink.CurrentPrice);
            //        }
            //    }
            //}
            using(StreamWriter sw = File.CreateText(@"C:\SkyDrive\Desktop\prijzenPerInterval.csv"))
            {
                drinks = drinks.OrderBy(x => x.Name).ToList();
                sw.WriteLine("Interval," + string.Join(",",drinks.Select(x => x.Name)));
                foreach (Interval item in intervals)
                {
                    string line = item.StartTime.ToShortTimeString() + "-" + item.EndTime.ToShortTimeString()+",";
                    foreach (Drink dr in drinks)
                    {
                        Drink inIntervalDrink = item.Drinks.FirstOrDefault(x => x.Id == dr.Id);
                        if (inIntervalDrink == null)
                        {
                            line += "?,";
                            continue;
                        }
                        line += inIntervalDrink.CurrentPrice + ",";
                    }
                    line = line.Substring(0, line.Length - 1);
                    sw.WriteLine(line);
                }
            }
        }

    }
}
