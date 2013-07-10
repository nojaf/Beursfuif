using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.DataAccess
{
    public interface IDrinkService
    {
        ObservableCollection<Drink> GetDrinksFromXml();

        bool SaveDrinksToXml(string filepath);
    }
}
