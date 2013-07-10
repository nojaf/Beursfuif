using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.DataAccess
{
    public class DrinkService : IDrinkService
    {
        public System.Collections.ObjectModel.ObservableCollection<BL.Drink> GetDrinksFromXml()
        {
            return new IOManager().LoadObservableCollectionFromXml<BL.Drink>(PathManager.DRINK_XML_PATH);
        }

        public bool SaveDrinksToXml(string filepath)
        {
            throw new NotImplementedException();
        }
    }
}
