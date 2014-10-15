using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Beursfuif.BL.Exceptions;
using Beursfuif.Server.Messages;

namespace Beursfuif.Server.Services
{
    public class BeursfuifData : IBeursfuifData
    {
        #region event
        public event EventHandler<bool> BeursfuifBusyChanged;

        public void RaiseBeursfuifBusyChanged()
        {
            if (BeursfuifBusyChanged != null)
            {
                BeursfuifBusyChanged(this, BeursfuifBusy);
            }
        }


        //The boolean indicates if it's a full reset
        public event EventHandler<bool> DataReset;

        public void RaisDataReset(bool fullReset)
        {
            if (DataReset != null)
            {
                DataReset(this, fullReset);
            }
        }

        public event EventHandler BeursfuifDataImported;

        public void RaiseBeursfuifDataImported()
        {
            if(BeursfuifDataImported != null)
            {
                BeursfuifDataImported(this, EventArgs.Empty);
            }
        }
        #endregion

        #region fields and properties
        private IIOManager _ioManager;

        public ObservableCollection<Drink> Drinks
        {
            get;
            set;
        }

        public DateTime BeginTime
        {
            get;
            set;
        }

        public DateTime EndTime
        {
            get;
            set;
        }

        public Interval[] Intervals
        {
            get;
            set;
        }

        public Interval CurrentInterval
        {
            get;
            set;
        }

        public DateTime BeursfuifCurrentTime
        {
            get;
            set;
        }

        public ObservableCollection<Client> Clients
        {
            get;
            set;
        }

        public bool BeursfuifBusy
        {
            get;
            set;
        }

        public bool BeursfuifEverStarted
        {
            get;
            set;
        }

        public ObservableCollection<ShowOrder> AllOrders
        {
            get;
            set;
        }

        public List<ClientDrinkOrder> AllOrderItems
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public string BackUpLocation { get; set; }
        #endregion

        public BeursfuifData(IIOManager ioManager)
        {
            _ioManager = ioManager;
            LoadAllData();
        }

        #region Load
        public void LoadAllData()
        {
            //First load the setting.json, this determines if the party has already started
            var settings = _ioManager.Load<SaveSettings>(PathManager.SETTINGS_PATH);
            if (settings != null)
            {
                BeursfuifBusy = settings.Busy;
                BeursfuifEverStarted = true;
                Port = settings.Port;
                BeursfuifCurrentTime = settings.CurrentTime;
                BackUpLocation = settings.BackupLocation;

                LoadDrinksAndIntervals();
                LoadOrders();
                LoadCurrentInterval();
            }
            else
            {
                //Beursfuif never happened
                LoadDrinksAndIntervals();
                DateTime now = DateTime.Now;
                BeginTime = new DateTime(now.Year, 1, 1, 21, 0, 0);
                EndTime = new DateTime(now.Year, 1, 2, 5, 0, 0);
                Port = 5678;
            }

            Clients = new ObservableCollection<Client>();
        }

        private void LoadCurrentInterval()
        {
            CurrentInterval = _ioManager.Load<Interval>(PathManager.CURRENT_INTERVAL_PATH);
        }

        private void LoadOrders()
        {
            AllOrders = _ioManager.Load<ObservableCollection<ShowOrder>>(PathManager.ALL_ORDERS);

            if (AllOrders != null && AllOrders.Any())
            {
                AllOrderItems = AllOrders.SelectMany(x => x.Orders).ToList();
            }
            else
            {
                AllOrderItems = new List<ClientDrinkOrder>();
            }
        }

        private void LoadDrinksAndIntervals()
        {
            Drinks = _ioManager.Load<ObservableCollection<Drink>>(PathManager.DRINK_PATH);

            if (Drinks == null)
            {
                Drinks = new ObservableCollection<Drink>();
            }

            Intervals = _ioManager.Load<Interval[]>(PathManager.INTERVAL_PATH);
        }

        public void RestoreAllData()
        {
            LoadAllData();
            BackUpLocation = string.Empty;
            RaiseBeursfuifDataImported();
        }
        #endregion

        #region Save

        public void SaveAllData()
        {
            SaveDrinks();
            SaveCurrentInterval();
            SaveIntervals();
            SaveAllOrders();
        }

        public void SaveDrinks()
        {
            _ioManager.Save<ObservableCollection<Drink>>(PathManager.DRINK_PATH, Drinks);
        }

        public void SaveIntervals()
        {
            _ioManager.Save<Interval[]>(PathManager.INTERVAL_PATH, Intervals);
        }

        public void SaveCurrentInterval()
        {
            _ioManager.Save<Interval>(PathManager.CURRENT_INTERVAL_PATH, CurrentInterval);
        }

        public void SaveSettings(SaveSettings settings)
        {
            _ioManager.Save<SaveSettings>(PathManager.SETTINGS_PATH, settings);
        }

        public void SaveAllOrders()
        {
            _ioManager.Save<ObservableCollection<ShowOrder>>(PathManager.ALL_ORDERS, AllOrders);
        }

        #endregion

        #region Helper
        public void ChangeBeursfuifBusy(bool busy)
        {
            if (busy != BeursfuifBusy)
            {
                BeursfuifBusy = busy;
            }
            RaiseBeursfuifBusyChanged();
        }

        public string AuthenticationString()
        {
            if(CurrentInterval == null)
            {
                throw new BeursfuifException("No current interval", new ArgumentNullException("CurrentInterval"));
            }

            return CurrentInterval.AuthenticationString();
        }

        public string GetClientName(Guid guid)
        {
            if (Clients != null && Clients.Any(x => x.Id == guid))
            {
                return Clients.First(x => x.Id == guid).Name;
            }

            return "Unknown client";
        }
        #endregion

        #region Reset
        public void ResetAll()
        {
            ResetData();
            Drinks = new ObservableCollection<Drink>();
            SafeDeleteFile(PathManager.DRINK_PATH);
            SafeDeleteFolder(PathManager.ASSETS_PATH);
            RaisDataReset(true);
        }

        public void ResetData()
        {
            Clients = new ObservableCollection<Client>();
            Intervals = null;
            AllOrders = new ObservableCollection<ShowOrder>();
            AllOrderItems = new List<ClientDrinkOrder>();
            DateTime now = DateTime.Now;
            BeginTime = new DateTime(now.Year, 1, 1, 21, 0, 0);
            EndTime = new DateTime(now.Year, 1, 2, 5, 0, 0);
            ChangeBeursfuifBusy(false);
            BeursfuifEverStarted = false;
            CurrentInterval = null;

            //Delete json files
            SafeDeleteFile(PathManager.INTERVAL_PATH);
            SafeDeleteFile(PathManager.SETTINGS_PATH);
            SafeDeleteFile(PathManager.ALL_ORDERS);
            SafeDeleteFile(PathManager.CURRENT_INTERVAL_PATH);
            SafeDeleteFolder(PathManager.LOG_FOLDER);
            RaisDataReset(false);
        }

        private void SafeDeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        private void SafeDeleteFolder(string path)
        {
            if (System.IO.Directory.Exists(path))
            {
                string[] files = System.IO.Directory.GetFiles(path);
                foreach (var file in files)
                {
                    SafeDeleteFile(file);
                }
            }
        }
        #endregion

        public bool Directory { get; set; }



    }
}
