using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using Beursfuif.BL.Extensions;
using Beursfuif.Server.Messages;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Beursfuif.Server.ViewModel
{
    public class SettingsViewModel : BeursfuifViewModelBase
    {
        #region Fields and Properties
        /// <summary>
        /// The <see cref="CurrentInterval" /> property's name.
        /// </summary>
        public const string CurrentIntervalPropertyName = "CurrentInterval";

        private Interval _currentInterval = null;

        /// <summary>
        /// Sets and gets the CurrentInterval property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Interval CurrentInterval
        {
            get
            {
                return _currentInterval;
            }

            set
            {
                if (_currentInterval == value)
                {
                    return;
                }

                RaisePropertyChanging(CurrentIntervalPropertyName);
                _currentInterval = value;
                RaisePropertyChanged(CurrentIntervalPropertyName);
            }
        }

        //Button content to control the fuif
        private const string PARTY_NEVER_STARTED = "Fuif starten";
        private const string RESUME_PARTY = "Fuif hervatten";
        private const string PAUSE_PARTY = "Fuif pauseren";

        /// <summary>
        /// The <see cref="MainActionButtonContent" /> property's name.
        /// </summary>
        public const string MainActionButtonContentPropertyName = "MainActionButtonContent";

        private string _mainActionButtonContent = PARTY_NEVER_STARTED;

        /// <summary>
        /// Sets and gets the MainActionButtonContent property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string MainActionButtonContent
        {
            get
            {
                return _mainActionButtonContent;
            }

            set
            {
                if (_mainActionButtonContent == value)
                {
                    return;
                }

                RaisePropertyChanging(MainActionButtonContentPropertyName);
                _mainActionButtonContent = value;
                RaisePropertyChanged(MainActionButtonContentPropertyName);
            }
        }

        private const string BeursfuifBusyVisibilityPropertyName = "BeursfuifBusyVisibility";
        public Visibility BeursfuifBusyVisibility
        {
            get
            {
                return (BeursfuifBusy ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        /// <summary>
        /// The <see cref="BeursfuifCurrentTime" /> property's name.
        /// </summary>
        public const string BeursfuifCurrentTimePropertyName = "BeursfuifCurrentTime";

        private DateTime _beursfuifCurrentTime = DateTime.Now;

        /// <summary>
        /// Sets and gets the BeursfuifCurrentTime property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime BeursfuifCurrentTime
        {
            get
            {
                return _beursfuifCurrentTime;
            }

            set
            {
                if (_beursfuifCurrentTime == value)
                {
                    return;
                }

                RaisePropertyChanging(BeursfuifCurrentTimePropertyName);
                _beursfuifCurrentTime = value;
                RaisePropertyChanged(BeursfuifCurrentTimePropertyName);
            }
        }

        public RelayCommand MainActionButtonCommand { get; set; }

        private IOManager _ioManager;
        private System.Threading.Timer _tmrMain;
        private BeursfuifServer _server;

        public string IPAdress
        {
            get
            {
                var ip = LocalIPAddress();
                return (ip != null ? ip.ToString() + ":" + BeursfuifServer.Port : "");
            }
        }

        public RelayCommand AddOneMinute { get; set; }

        public RelayCommand ForceAutoSaveAllOrders { get; set; }

        public RelayCommand AvailableChangedCommand { get; set; }

        public RelayCommand ResetFuifCommand { get; set; }

        public RelayCommand ResetAllCommand { get; set; }
        #endregion

        public SettingsViewModel(IOManager ioManager, BeursfuifServer server)
        {
            if (!IsInDesignMode)
            {
                _ioManager = ioManager;
                _server = server;

                if (System.IO.File.Exists(PathManager.BUSY_AND_TIME_PATH))
                {
                    SendLogMessage("Beursfuif has already started", LogType.SETTINGS_VM);
                    SaveSettings settings = _ioManager.LoadObjectFromXml<SaveSettings>(PathManager.BUSY_AND_TIME_PATH);
                    BeursfuifBusy = settings.Busy;
                    BeursfuifCurrentTime = settings.CurrentTime;
                    CurrentInterval = LoadCurrentInterval();

                    if (BeursfuifBusy)
                    {
                        SendLogMessage("Resuming Beursfuif", LogType.SETTINGS_VM);
                        MainActionButtonContent = PAUSE_PARTY;
                        ResumeParty();
                    }
                    else
                    {
                        SendLogMessage("Beursfuif paused", LogType.SETTINGS_VM);
                        MainActionButtonContent = RESUME_PARTY;
                    }


                }
                else
                {
                    MainActionButtonContent = PARTY_NEVER_STARTED;
                    SendLogMessage("Beursfuif has never started", LogType.SETTINGS_VM);
                }

                InitCommands();
                InitServer();
                InitMessages();

            }
        }

        private Interval LoadCurrentInterval()
        {
            return _ioManager.LoadObjectFromXml<Interval>(PathManager.CURRENT_INTERVAL_XML_PATH);
        }

        private void InitCommands()
        {
            MainActionButtonCommand = new RelayCommand(MainActionCommand, ValidatePartyConditions);
            AddOneMinute = new RelayCommand(() => { BeursfuifCurrentTime = BeursfuifCurrentTime.AddMinutes(1); });
            ForceAutoSaveAllOrders = new RelayCommand(() => { MessengerInstance.Send<AutoSaveAllOrdersMessage>(new AutoSaveAllOrdersMessage()); });
            ResetFuifCommand = new RelayCommand(ResetFuifData);
            ResetAllCommand = new RelayCommand(ResetAll);
        }

        private bool ValidatePartyConditions()
        {
            var locator = GetLocator();
            if (locator != null)
            {
                try
                {

                    if (locator.Drink.Drinks.Count > 0 && locator.Interval.Intervals.Length > 0)
                    {
                        //SendLogMessage("PartyConditions are valid", LogType.SETTINGS_VM);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    //SendLogMessage("ValidatePartyConditions threw ex = " + ex.Message, LogType.ERROR | LogType.SETTINGS_VM);
                    return false;
                }
            }
            return false;
        }

        private void MainActionCommand()
        {
            switch (MainActionButtonContent)
            {
                case PARTY_NEVER_STARTED:
                    ThreadPool.QueueUserWorkItem(InitParty);
                    break;
                case RESUME_PARTY:
                    ResumeParty();
                    break;
                case PAUSE_PARTY:
                    PauseParty();
                    break;
            }
        }

        private void PauseParty()
        {
            SendLogMessage("Party paused method", LogType.SETTINGS_VM);
            BeursfuifBusy = false;
            RaisePropertyChanged(BeursfuifBusyVisibilityPropertyName);

            _tmrMain.Change(Timeout.Infinite, Timeout.Infinite);

            _server.Pause();

            _server.StopServer();

            MainActionButtonContent = RESUME_PARTY;

            ThreadPool.QueueUserWorkItem(SaveSettings);
            SendToastMessage("Server paused");
        }

        private void ResumeParty()
        {
            SendLogMessage("Resuming party method", LogType.SETTINGS_VM);
            BeursfuifBusy = true;
            RaisePropertyChanged(BeursfuifBusyVisibilityPropertyName);
            _server.RestartServer();

            if (_tmrMain == null)
            {
                //start timer
                _tmrMain = new Timer(MainTimer_Tick, null, 1000, 1000);
            }
            else
            {
                _tmrMain.Change(1000, 1000);
            }

            MainActionButtonContent = PAUSE_PARTY;

            SendToastMessage("Server restarted");
            ThreadPool.QueueUserWorkItem(SaveSettings);
        }

        public void InitParty(object state)
        {
            //initial save triggers the views (Interval & Drink) to disable controls
            SaveSettings(state);

            BeursfuifBusy = true;
            RaisePropertyChanged(BeursfuifBusyVisibilityPropertyName);

            var locator = GetLocator();

            //drinks
            var drinks = locator.Drink.Drinks.Where(x => x.Available).ToArray();

            //fill all intervals
            int intervalCount = locator.Interval.Intervals.Length;

            for (int i = 0; i < intervalCount; i++)
            {
                locator.Interval.Intervals[i].Drinks = drinks;
            }
            locator.Interval.SaveIntervals();

            CurrentInterval = locator.Interval.Intervals[0];

            BeursfuifCurrentTime = CurrentInterval.StartTime;

            //start timer
            _tmrMain = new Timer(MainTimer_Tick, null, 1000, 1000);

            _server.StartServer();
            MainActionButtonContent = PAUSE_PARTY;
            SendToastMessage("Server started");

            SendLogMessage("Beursfuif has been initialized and started", LogType.SETTINGS_VM);

            SaveSettings(state);
        }

        private void SaveSettings(object state)
        {
            _ioManager.SaveObjectToXml<SaveSettings>(PathManager.BUSY_AND_TIME_PATH, new SaveSettings(BeursfuifBusy, BeursfuifCurrentTime));

            //CurrentInterval
            _ioManager.SaveObjectToXml<Interval>(PathManager.CURRENT_INTERVAL_XML_PATH, CurrentInterval);

            SendLogMessage("Beursfuifsettings and currentInterval have been saved", LogType.SETTINGS_VM);
        }

        private void ResetAll()
        {
            _dm = new DialogMessage();

            if (BeursfuifBusy)
            {
                _dm.Title = "Fuif is nog bezig";
                _dm.Errors.Add("Je kan geen reset uitvoeren zolang de fuif nog bezig is.");
                _dm.Nay = Visibility.Hidden;
                MessengerInstance.Send<DialogMessage>(_dm);
                return;
            }
            else
            {
                _dm.Title = "Ben je zeker?";
                _dm.Errors.Add("Alles zal verwijderd worden.");
                _dm.Errors.Add("De data is permanent weg.");
                _dm.Errors.Add("Doorgaan?");
                         _dm.Nay = Visibility.Visible;
                _dm.AnswerChanged += (s, b) =>
                {
                    if (b.Value)
                    {
                        ResetFuifData();

                        var drinkVM = base.GetLocator().Drink;

                        //drink vm
                        drinkVM.Drinks.Clear();
                        drinkVM.NewEditDrink = null;
                        drinkVM.DownloadUrl = "";
                        drinkVM.Downloading = false;
                        drinkVM.CanModify = true;

                        Task.Factory.StartNew(() =>
                        {
                            File.Delete(PathManager.DRINK_XML_PATH);
                        });
                    }
                };

                MessengerInstance.Send<DialogMessage>(_dm);
                SendLogMessage("All data removed", LogType.SETTINGS_VM);
            }
        }

        private void ResetFuifData()
        {
            _dm = new DialogMessage();

            if (BeursfuifBusy)
            {
                _dm.Title = "Fuif is nog bezig";
                _dm.Errors.Add("Je kan geen reset uitvoeren zolang de fuif nog bezig is.");
                _dm.Nay = Visibility.Hidden;
                MessengerInstance.Send<DialogMessage>(_dm);
                return;
            }
            else
            {
                _dm.Title = "Ben je zeker?";
                _dm.Errors.Add("Alles behalve de dranken zal verwijderd worden.");
                _dm.Errors.Add("De data is permanent weg.");
                _dm.Errors.Add("Doorgaan?");
                _dm.Nay = Visibility.Visible;
                _dm.AnswerChanged += (s, b) =>
                {
                    if (b.Value)
                    {
                        var locator = base.GetLocator();
                        //drink vm
                        locator.Drink.CanModify = true;

                        //interval vm
                        locator.Interval.Intervals = null;
                        locator.Interval.BeginTime = DateTime.MinValue;
                        locator.Interval.EndTime = DateTime.MinValue;
                        locator.Interval.CanModify = true;
                        locator.Interval.ChosenInterval = TimeSpan.Zero;

                        //clients vm
                        locator.Clients.Clients.Clear();

                        //settings vm
                        this.CurrentInterval = null;
                        this.BeursfuifCurrentTime = DateTime.MinValue;
                        this.MainActionButtonContent = PARTY_NEVER_STARTED;

                        //orders vm
                        locator.Orders.AllOrderItems.Clear();
                        locator.Orders.ShowOrderList = null;
                        locator.Orders.AllOrders = null;
                        locator.Orders.SelectedInterval = null;
                        locator.Orders.ReducedDrinks = null;
                        locator.Orders.ReducedIntervals = null;

                        //log vm
                        locator.Log.AllLogMessages.Clear();
                        locator.Log.SelectedLogMessages.Clear();
                        locator.Log.SelectedLogType = LogType.ALL;

                        //prediction vm
                        locator.Prediction.PredictDrinks = null;
                        locator.Prediction.IsDirty = false;




                        Task.Factory.StartNew(() =>
                        {
                            File.Delete(PathManager.BUSY_AND_TIME_PATH);
                            File.Delete(PathManager.CURRENT_INTERVAL_XML_PATH);
                            File.Delete(PathManager.INTERVAL_BINARY_PATH);
                            File.Delete(PathManager.AUTO_SAVE_ALL_ORDERS);
                        });

                        SendLogMessage("Fuifdata removed", LogType.SETTINGS_VM);
                    }
                };

                MessengerInstance.Send<DialogMessage>(_dm);
            }           
        }

        public void MainTimer_Tick(object state)
        {
            _tmrMain.Change(int.MaxValue, int.MaxValue);
            //BEGIN CODE
            if (BeursfuifBusy)
            {
                BeursfuifCurrentTime = BeursfuifCurrentTime.AddSeconds(1);

                if (BeursfuifCurrentTime.Second == 0)
                {
                    ThreadPool.QueueUserWorkItem(SaveSettings);
                    //TODO save all orders (bin)
                    MessengerInstance.Send<AutoSaveAllOrdersMessage>(new AutoSaveAllOrdersMessage());
                    SendLogMessage("Auto saved has been completed", LogType.SETTINGS_VM);

                    //sync time with clients
                    _server.UpdateTime(BeursfuifCurrentTime, CurrentInterval.AuthenticationString());
                    SendLogMessage("Server send update current time to clients", LogType.SETTINGS_VM);
                }

                if (BeursfuifCurrentTime > CurrentInterval.EndTime)
                {
                    //TODO Update time
                    SendLogMessage("Server will commence calculating new prices", LogType.SETTINGS_VM);
                    var locator = base.GetLocator();

                    ThreadPool.QueueUserWorkItem(new WaitCallback((object target) =>
                    {
                        Interval next = CalculatePriceUpdates(locator.Interval.Intervals, locator.Orders.AllOrderItems, CurrentInterval.Id, true);
                        App.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            CurrentInterval = next;
                            locator.Interval.SaveIntervals();
                            _server.UpdateInterval(next.ToClientInterval(BeursfuifCurrentTime), BeursfuifCurrentTime);
                            _tmrMain.Change(1000, 1000);
                        }));

                    }));
                    return;
                }
            }
            //END CODE
            _tmrMain.Change(1000, 1000);
        }

        #region Messages
        private void InitMessages()
        {
            MessengerInstance.Send<BeursfuifBusyMessage>(new BeursfuifBusyMessage() { Value = this.BeursfuifBusy });
            MessengerInstance.Register<DrinkAvailableMessage>(this, DrinkAvailableMessageReceived);
            MessengerInstance.Register<DrinkModifiedMessage>(this, DrinkModifiedHandler);
        }

        private void DrinkModifiedHandler(DrinkModifiedMessage msg)
        {
            var intervals = base.GetLocator().Interval.Intervals;
            if (intervals != null && intervals.Length > 0)
            {
                foreach (Interval interval in intervals)
                {
                    interval.UpdateDrink(msg.Drink);
                }
            }
        }

        private void DrinkAvailableMessageReceived(DrinkAvailableMessage msg)
        {
            Drink changed = CurrentInterval.Drinks.FirstOrDefault(x => x.Id == msg.DrinkId);
            if (changed != null)
            {
                changed.Available = msg.Available;
                _server.SendDrinkAvailableChanged(msg.DrinkId, (msg.Available ? changed : null), CurrentInterval.Id);
            }
            ThreadPool.QueueUserWorkItem(SaveSettings);
        }
        #endregion

        #region Price Updates
        public Interval CalculatePriceUpdates(Interval[] intervals, List<ClientDrinkOrder> allOrdersItems, int currentIntervalId, bool addAddition)
        {
            Interval currentInterval = intervals.FirstOrDefault(x => x.Id == currentIntervalId);
            int indexCurrentInterval = Array.IndexOf(intervals, currentInterval);
            if (indexCurrentInterval == intervals.Length - 1) return null;

            Interval nextInterval = (addAddition ? intervals[indexCurrentInterval + 1] : new Interval());


            if (indexCurrentInterval == 0) return nextInterval;

            //create a copy of the interval object, because we don't want to affect the real values
            if (!addAddition)
            {
                Interval realNextInterval = intervals[indexCurrentInterval + 1];
                nextInterval.Id = realNextInterval.Id;
                nextInterval.Drinks = realNextInterval.Drinks;
            }

            //In this scenario the change of prices will only trigger after the second interval
            Interval previousInterval = intervals[indexCurrentInterval - 1];

            var availableDrinks = nextInterval.Drinks.Where(x => x.Available);
            SendLogMessage("Prices update stats\n------------------------", LogType.SETTINGS_VM);



            foreach (Drink dr in availableDrinks)
            {
                int currentPrice = currentInterval.Drinks.First(x => x.Id == dr.Id).CurrentPrice;

                int previousCount = allOrdersItems.Where(x => x.IntervalId == previousInterval.Id &&
                    x.DrinkId == dr.Id).Sum(x => x.Count);
                int currentCount = allOrdersItems.Where(x => x.IntervalId == currentIntervalId &&
                    x.DrinkId == dr.Id).Sum(x => x.Count);

                if (previousCount == 0) previousCount = 1;

                sbyte addition = (addAddition ? currentInterval.Drinks.First(x => x.Id == dr.Id).NextPriceAddition : (sbyte)0);

                dr.CurrentPrice = (byte)(Math.Round(currentPrice * ((double)currentCount / (double)previousCount)) + addition);

                if (dr.CurrentPrice > dr.MaximumPrice) dr.CurrentPrice = dr.MaximumPrice;
                if (dr.CurrentPrice < dr.MiniumPrice) dr.CurrentPrice = dr.MiniumPrice;

                SendLogMessage(string.Format("{0}: current price: {1}, previous count: {2}, current count: {3}, new price: {4}",
                    dr.Name, currentPrice, previousCount, currentCount, dr.CurrentPrice), LogType.SETTINGS_VM);
            }


            return nextInterval;
        }
        #endregion

        #region Websocket
        private void InitServer()
        {
            _server.NewClientEvent += Server_NewClientEvent;

            App.Current.MainWindow.Closing += (a, b) =>
            {
                _server.StopServer();
                SendLogMessage("Window is closing, shutdown server", LogType.SETTINGS_VM);
            };
        }

        void Server_NewClientEvent(object sender, BL.Event.NewClientEventArgs e)
        {
            _server.SendAckInitialClientConnect(CurrentInterval.ToClientInterval(BeursfuifCurrentTime), e.Id, BeursfuifCurrentTime);
            SendLogMessage("Repley on " + e.Name + "'s connection request", LogType.SETTINGS_VM);
        }





        private IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }


            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork && IsIPLocal(ip));
        }

        private bool IsIPLocal(IPAddress ipaddress)
        {
            String[] straryIPAddress = ipaddress.ToString().Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            int[] iaryIPAddress = new int[] { int.Parse(straryIPAddress[0]), int.Parse(straryIPAddress[1]), int.Parse(straryIPAddress[2]), int.Parse(straryIPAddress[3]) };
            if (iaryIPAddress[0] == 10 || (iaryIPAddress[0] == 192 && iaryIPAddress[1] == 168) || (iaryIPAddress[0] == 172 && (iaryIPAddress[1] >= 16 && iaryIPAddress[1] <= 31)))
            {
                return true;
            }
            else
            {
                // IP Address is "probably" public. This doesn't catch some VPN ranges like OpenVPN and Hamachi.
                return false;
            }
        }
        #endregion

    }
}
