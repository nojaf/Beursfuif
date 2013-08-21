using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace Beursfuif.Server.ViewModel
{
    public class SettingsViewModel:BeursfuifViewModelBase
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
            get{
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
        private Alchemy.WebSocketServer _server;
        private const int PORT = 5678;

        public string IPAdress
        {
            get
            {
                var ip = LocalIPAddress();
                return (ip != null ? ip.ToString() + ":"+PORT : "");
            }
        }

#endregion

        public SettingsViewModel(IOManager ioManager)
        {
            _ioManager = ioManager;
            CurrentInterval = LoadCurrentInterval();
            if (CurrentInterval != null)
            {
                MainActionButtonContent = RESUME_PARTY;
            }
            else
            {
                MainActionButtonContent = PARTY_NEVER_STARTED;
            }

            InitCommands();
            InitServer();
        }

        private Interval LoadCurrentInterval()
        {
            return _ioManager.LoadObjectFromBinary<Interval>(PathManager.CURRENT_INTERVAL_BINARY_PATH);
        }

        private void InitCommands()
        {
            MainActionButtonCommand = new RelayCommand(MainActionCommand, ValidatePartyConditions); 
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

        private void MainActionCommand() {
            switch(MainActionButtonContent)
            {
                case PARTY_NEVER_STARTED:
                    ThreadPool.QueueUserWorkItem(InitParty);
                    break;
                case RESUME_PARTY:

                    break;
                case PAUSE_PARTY:

                    break;
            }
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

            CurrentInterval = locator.Interval.Intervals[0];

            BeursfuifCurrentTime = CurrentInterval.StartTime;

            //start timer
            _tmrMain = new Timer(MainTimer_Tick, null, 1000, 1000);

            StartServer();
        }

        public void MainTimer_Tick(object state)
        {
            _tmrMain.Change(int.MaxValue,int.MaxValue);
            //BEGIN CODE
            BeursfuifCurrentTime = BeursfuifCurrentTime.AddSeconds(1);

            if (BeursfuifCurrentTime.CompareTo(CurrentInterval.EndTime) > 1)
            {
                //Update time
                Console.WriteLine("Update");
            }

            //END CODE
            _tmrMain.Change(1000, 1000);
        }

        #region Websocket
        private void InitServer()
        {
            _server = new Alchemy.WebSocketServer(5678, IPAddress.Any)
            {
                OnReceive = OnWebSocketMessageReceive,
                OnSend = OnWebSocketMessageSend,
                OnConnected = OnWebSocketClientConnect,
                OnDisconnect = OnWebSocketClientDisconnect,
                TimeOut = new TimeSpan(0,5,0)
            };

            App.Current.MainWindow.Closing += (a, b) => {
                StopServer();
            };
        }



        private void OnWebSocketClientDisconnect(Alchemy.Classes.UserContext context)
        {

        }

        private void OnWebSocketClientConnect(Alchemy.Classes.UserContext context)
        {
            Console.WriteLine("Client " + context.ClientAddress.ToString() + " connected");
        }

        private void OnWebSocketMessageSend(Alchemy.Classes.UserContext context)
        {

        }

        private void OnWebSocketMessageReceive(Alchemy.Classes.UserContext context)
        {

        }

        public void StartServer()
        {
            _server.Start();
        }

        public void StopServer()
        {
            _server.Stop();
        }

        public void RestartServer()
        {
            _server.Restart();
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

        public ViewModelLocator GetLocator()
        {
            return  Application.Current.Resources["Locator"] as ViewModelLocator;          
        }

        
    }
}
