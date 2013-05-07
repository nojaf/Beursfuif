using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Beursfuif.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ObservableCollection<Drink> drinks = new ObservableCollection<Drink>(){
                new Drink() { Id = 1, Name = "Jupiler" },
                new Drink() { Id = 2, Name = "Cola" }
            };

            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<Drink>));
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Skydrive\Desktop\drinks.xml");
            writer.Serialize(file, drinks);
            file.Close();

            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<Drink>));
            System.IO.StreamReader fileRead = new System.IO.StreamReader(@"C:\Skydrive\Desktop\drinks.xml");
            ObservableCollection<Drink> readedDrinks = reader.Deserialize(fileRead) as ObservableCollection<Drink>;
            if(readedDrinks != null)
            {
                foreach (Drink dr in readedDrinks)
                {
                    Console.WriteLine(dr.ToString());
                }
            }

            Console.ReadLine();
            
        }
    }
}
