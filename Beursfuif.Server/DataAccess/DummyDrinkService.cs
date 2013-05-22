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
        public System.Collections.ObjectModel.ObservableCollection<BL.Drink> GetDrinksFromXml(string filepath)
        {
            ObservableCollection<Drink> drinks = new ObservableCollection<Drink>();
            drinks.Add(new Drink()
            {
                Available = true,
                Id = 1,
                Name = "Coke"
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
