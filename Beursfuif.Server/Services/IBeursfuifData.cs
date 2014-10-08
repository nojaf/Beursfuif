using Beursfuif.BL;
using Beursfuif.Server.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beursfuif.Server.Services
{
    public interface IBeursfuifData
    {

        event EventHandler<bool> BeursfuifBusyChanged;

        event EventHandler<bool> DataReset;


        ObservableCollection<Drink> Drinks {get;set;}

        DateTime BeginTime { get; set; }

        DateTime EndTime { get; set; }

        Interval[] Intervals { get; set; }

        Interval CurrentInterval { get; set; }

        DateTime BeursfuifCurrentTime { get; set; }

        ObservableCollection<Client> Clients { get; set; }

        bool BeursfuifBusy {get; set;}

        bool BeursfuifEverStarted { get; set; }

        ObservableCollection<ShowOrder> AllOrders { get; set; }

        List<ClientDrinkOrder> AllOrderItems { get; set; }

        int Port { get; set; }

        string BackUpLocation { get; set; }

        void ChangeBeursfuifBusy(bool busy);

        void LoadAllData();

        void SaveAllData();

        void SaveDrinks();

        void SaveIntervals();

        void SaveCurrentInterval();

        void SaveSettings(SaveSettings settings);

        void SaveAllOrders();

        string  AuthenticationString();

        void ResetAll();

        void ResetData();

        string GetClientName(Guid guid);
    }
}
