using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.DataAccess
{
    public class DummyDrinkService:IDrinkService
    {
        public System.Collections.ObjectModel.ObservableCollection<BL.Drink> GetDrinksFromXml()
        {
            ObservableCollection<Drink> drinks = new ObservableCollection<Drink>();
            drinks.Add(new Drink()
            {
                Available = true,
                Id = 1,
                Name = "Coke",
                ImageString =  @"C:\Skydrive\Projects\Beursfuif\Beursfuif.Server\bin\Debug\Images\DummyDrinkImage.png"
            });

            drinks.Add(new Drink()
            {
                Available = true,
                Id = 2,
                Name = "Tongerloo Blond",
                ImageString = @"C:\Skydrive\Projects\Beursfuif\Beursfuif.Server\bin\Debug\Images\DummyDrinkImage.png"
            });

            return drinks;
        }

        public bool SaveDrinksToXml(string filepath)
        {
            return true;
            //Will not be used in design time.
        }
    }
}
