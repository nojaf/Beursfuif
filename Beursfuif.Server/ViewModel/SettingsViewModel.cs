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

        #endregion

        public SettingsViewModel(IOManager ioManager, BeursfuifServer server)
        {
            _ioManager = ioManager;
            _server = server;

            if (System.IO.File.Exists(PathManager.BUSY_AND_TIME_PATH))
            {
                SaveSettings settings = _ioManager.LoadObjectFromXml<SaveSettings>(PathManager.BUSY_AND_TIME_PATH);
                BeursfuifBusy = settings.Busy;
                BeursfuifCurrentTime = settings.CurrentTime;
                CurrentInterval = LoadCurrentInterval();

                if (BeursfuifBusy)
                {
                    MainActionButtonContent = PAUSE_PARTY;
                    ResumeParty();
                }
                else
                {
                    MainActionButtonContent = RESUME_PARTY;
                }


            }
            else
            {
                MainActionButtonContent = PARTY_NEVER_STARTED;
            }

            InitCommands();
            InitServer();

            MessengerInstance.Send<BeursfuifBusyMessage>(new BeursfuifBusyMessage() { Value = this.BeursfuifBusy });
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
                        return true;
                    }
                }
                catch
                {
                    return true;
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
            BeursfuifBusy = false;
            RaisePropertyChanged(BeursfuifBusyVisibilityPropertyName);

            _tmrMain.Change(Timeout.Infinite, Timeout.Infinite);

            _server.Pause();

            _server.StopServer();

            MainActionButtonContent = RESUME_PARTY;

            ThreadPool.QueueUserWorkItem(SaveSettings);
            MessengerInstance.Send<ToastMessage>(new ToastMessage("Server paused"));
        }

        private void ResumeParty()
        {
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

            base.MessengerInstance.Send<ToastMessage>(new ToastMessage("Server restarted"));
            ThreadPool.QueueUserWorkItem(SaveSettings);
        }

        public void InitParty(object state)
        {
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
            base.MessengerInstance.Send<ToastMessage>(new ToastMessage("Server started"));

            SaveSettings(state);
        }

        private void SaveSettings(object state)
        {
            //Busy? bool    }
            //CurrentTime   } => Tuple<bool,DateTime>
            _ioManager.SaveObjectToXml<SaveSettings>(PathManager.BUSY_AND_TIME_PATH, new SaveSettings(BeursfuifBusy, BeursfuifCurrentTime));

            //CurrentInterval
            _ioManager.SaveObjectToXml<Interval>(PathManager.CURRENT_INTERVAL_XML_PATH, CurrentInterval);
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

                    //sync time with clients
                    _server.UpdateTime(BeursfuifCurrentTime);
                }

                if (BeursfuifCurrentTime > CurrentInterval.EndTime)
                {
                    //TODO Update time
                    Console.WriteLine("Update");
                    var locator = base.GetLocator();

                    ThreadPool.QueueUserWorkItem(new WaitCallback((object target) =>
                    {
                        Interval next = CalculatePriceUpdates(locator.Interval.Intervals, locator.Orders.AllOrderItems, CurrentInterval.Id);
                        App.Current.Dispatcher.BeginInvoke(new Action(() => {
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

        #region Price Updates
        private Interval CalculatePriceUpdates(Interval[] intervals,List<ClientDrinkOrder>  allOrdersItems, int currentIntervalId)
        {
            Interval currentInterval = intervals.FirstOrDefault(x => x.Id == currentIntervalId);
            int indexCurrentInterval = Array.IndexOf(intervals, currentInterval);
            if (indexCurrentInterval == intervals.Length - 1) return null;

            Interval nextInterval = intervals[indexCurrentInterval + 1];
            if (indexCurrentInterval == 0) return nextInterval;

            //In this scenario the change of prices will only trigger after the second interval
            Interval previousInterval = intervals[indexCurrentInterval - 1];

            foreach (Drink dr in nextInterval.Drinks)
            {
                int currentPrice = currentInterval.Drinks.First(x => x.Id == dr.Id).CurrentPrice;

                int previousCount = allOrdersItems.Where(x => x.IntervalId == previousInterval.Id &&
                    x.DrinkId == dr.Id).Sum(x => x.Count);
                int currentCount = allOrdersItems.Where(x => x.IntervalId == currentIntervalId &&
                    x.DrinkId == dr.Id).Sum(x => x.Count);

                dr.CurrentPrice = (byte)Math.Round(currentPrice * ((double)currentCount / (double)previousCount));

                if (dr.CurrentPrice > dr.MaximumPrice) dr.CurrentPrice = dr.MaximumPrice;
                if (dr.CurrentPrice < dr.MiniumPrice) dr.CurrentPrice = dr.MiniumPrice;
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
            };
        }

        void Server_NewClientEvent(object sender, BL.Event.NewClientEventArgs e)
        {
            _server.SendAckInitialClientConnect(CurrentInterval.ToClientInterval(BeursfuifCurrentTime), e.Id, BeursfuifCurrentTime);
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
