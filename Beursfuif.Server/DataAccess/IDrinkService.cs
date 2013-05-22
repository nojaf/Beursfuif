using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.DataAccess
{
    interface IDrinkService
    {
        ObservableCollection<Drink> GetDrinksFromXml(string filepath);

        bool SaveDrinksToXml(string filepath);
    }
}
