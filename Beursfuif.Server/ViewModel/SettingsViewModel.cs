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
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Ionic.Zip;
using Beursfuif.Server.Services;

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
        private IBeursfuifServer _server;

        /// <summary>
        /// The <see cref="BackupLocation" /> property's name.
        /// </summary>
        public const string BackupLocationPropertyName = "BackupLocation";

        private string _backupLocation = string.Empty;

        /// <summary>
        /// Sets and gets the BackupLocation property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string BackupLocation
        {
            get
            {
                return _backupLocation;
            }

            set
            {
                if (_backupLocation == value)
                {
                    return;
                }

                RaisePropertyChanging(BackupLocationPropertyName);
                _backupLocation = value;
                RaisePropertyChanged(BackupLocationPropertyName);
            }
        }

        public string IPAdress
        {
            get
            {
                var ip = LocalIPAddress();
                return (ip != null ? ip.ToString() : "localhost");
            }
        }

        public RelayCommand AddOneMinute { get; set; }

        public RelayCommand ForceAutoSaveAllOrders { get; set; }

        public RelayCommand AvailableChangedCommand { get; set; }

        public RelayCommand ResetFuifCommand { get; set; }

        public RelayCommand ResetAllCommand { get; set; }

        public RelayCommand ChangeBackupLocationCommand { get; set; }

        public RelayCommand RestoreBackupCommand { get; set; }
        #endregion

        public SettingsViewModel(IOManager ioManager, IBeursfuifServer server)
        {
            if (!IsInDesignMode)
            {
                PointInCode("SettingsViewModel: Ctor");

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
            PointInCode("SettingsViewModel: InitCommands");

            MainActionButtonCommand = new RelayCommand(MainActionCommand, ValidatePartyConditions);
            AddOneMinute = new RelayCommand(() => { BeursfuifCurrentTime = BeursfuifCurrentTime.AddMinutes(1); });
            ForceAutoSaveAllOrders = new RelayCommand(() => { MessengerInstance.Send<AutoSaveAllOrdersMessage>(new AutoSaveAllOrdersMessage()); });
            ResetFuifCommand = new RelayCommand(ResetFuifData);
            ResetAllCommand = new RelayCommand(ResetAll);
            ChangeBackupLocationCommand = new RelayCommand(ChangeBackupLocation);
            RestoreBackupCommand = new RelayCommand(RestoreBackup, () => { return !BeursfuifBusy; });
        }

        private void RestoreBackup()
        {
            PointInCode("SettingsViewModel: RestoreBackup");


            FileDialog dialog = new OpenFileDialog()
            {
                Filter = "Zip Files|*.zip"
            };

            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if(dialog.CheckFileExists){
                    string zipLocation = dialog.FileName;
                    IEnumerable<string> folders = Directory.EnumerateDirectories(PathManager.BEURSFUIF_FOLDER);
                    foreach (string folder in folders)
                    {
                        Directory.Delete(folder, true);
                    }

                    using (ZipFile zip = ZipFile.Read(zipLocation))
                    {
                        zip.ExtractAll(PathManager.BEURSFUIF_FOLDER);
                    }


                    System.Windows.Forms.Application.Restart();
                    System.Windows.Application.Current.Shutdown();
                }
             
            }

        }

        private void ChangeBackupLocation()
        {
            PointInCode("SettingsViewModel: ChangeBackupLocation");

            FolderBrowserDialog dialog = new FolderBrowserDialog();
           var result = dialog.ShowDialog();
           if (result == DialogResult.OK)
           {
               BackupLocation = dialog.SelectedPath;
           }
        }

        private bool ValidatePartyConditions()
        {
            PointInCode("SettingsViewModel: ValidatePartyConditions");

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

        #region MainActionCommands
        private void MainActionCommand()
        {
            PointInCode("SettingsViewModel: MainActionCommand");

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
            PointInCode("SettingsViewModel: PauseParty");

            SendLogMessage("Party paused method", LogType.SETTINGS_VM);
            BeursfuifBusy = false;
            RaisePropertyChanged(BeursfuifBusyVisibilityPropertyName);

            _tmrMain.Change(Timeout.Infinite, Timeout.Infinite);

            _server.Active = false;

            MainActionButtonContent = RESUME_PARTY;

            ThreadPool.QueueUserWorkItem(SaveSettings);
            SendToastMessage("Server paused");
        }

        private async void ResumeParty()
        {
            PointInCode("SettingsViewModel: ResumeParty");

            SendLogMessage("Resuming party method", LogType.SETTINGS_VM);
            BeursfuifBusy = true;
            RaisePropertyChanged(BeursfuifBusyVisibilityPropertyName);
            _server.Active = true;


            bool succes = await _server.Start(IPAdress, 5678); //TODO: add port property
            
            if(!succes)
            {
                _server.Active = false;
                BeursfuifBusy = false;
                RaisePropertyChanged(BeursfuifBusyVisibilityPropertyName);
                _dm = new DialogMessage("Kon de server niet opstarten");
                _dm.Errors.Add("Werd het programma als administrator opgestart?");
                _dm.Errors.Add("Is er netwerk beschikbaar?");
                _dm.Answer = false;
                _dm.Nay = Visibility.Collapsed;
                MessengerInstance.Send<DialogMessage>(_dm);
                return;
            }


            if (_tmrMain == null)
            {
                //start timer
                _tmrMain = new System.Threading.Timer(MainTimer_Tick, null, 1000, 1000);
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
            PointInCode("SettingsViewModel: InitParty");

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
                FillInDrinks(locator.Interval.Intervals[i], drinks);
            }
            locator.Interval.SaveIntervals();

            CurrentInterval = locator.Interval.Intervals[0];

            BeursfuifCurrentTime = CurrentInterval.StartTime;

            //start timer
            _tmrMain = new System.Threading.Timer(MainTimer_Tick, null, 1000, 1000);

            _server.Start(IPAdress, 5678);
            _server.Active = true;
            MainActionButtonContent = PAUSE_PARTY;
            SendToastMessage("Server started");

            SendLogMessage("Beursfuif has been initialized and started", LogType.SETTINGS_VM);

            SaveSettings(state);
        }
        #endregion

        //Because c# is always byRef on objects, that's why
        private void FillInDrinks(Interval interval, Drink[] drinks)
        {
            PointInCode("SettingsViewModel: FillInDrinks");

            int length = drinks.Length;
            interval.Drinks = new Drink[length];
            for (int i = 0; i < length; i++)
            {
                interval.Drinks[i] = drinks[i].Clone();
            }
        }

        #region Save and reset
        private void SaveSettings(object state)
        {
            PointInCode("SettingsViewModel: SaveSettings");

            _ioManager.SaveObjectToXml<SaveSettings>(PathManager.BUSY_AND_TIME_PATH, new SaveSettings(BeursfuifBusy, BeursfuifCurrentTime));

            //CurrentInterval
            _ioManager.SaveObjectToXml<Interval>(PathManager.CURRENT_INTERVAL_XML_PATH, CurrentInterval);

            SendLogMessage("Beursfuifsettings and currentInterval have been saved", LogType.SETTINGS_VM);
        }

        private void ResetAll()
        {
            PointInCode("SettingsViewModel: ResetAll");

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
            PointInCode("SettingsViewModel: ResetFuifData");

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
                        locator.Interval.BeginTime = new DateTime(1970, 1, 1, 21, 0, 0);
                        locator.Interval.EndTime = new DateTime(1970, 1, 1, 22, 0, 0);
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
        #endregion

        #region Timer
        public void MainTimer_Tick(object state)
        {
            _tmrMain.Change(int.MaxValue, int.MaxValue);
            //BEGIN CODE
            if (BeursfuifBusy && CurrentInterval != null)
            {
                BeursfuifCurrentTime = BeursfuifCurrentTime.AddSeconds(1);

                if (BeursfuifCurrentTime.Second == 0)
                {
                    OneMinutePassed();
                }
                else if (BeursfuifCurrentTime > CurrentInterval.EndTime)
                {
                    EndOfInterval();
                    return;
                }
            }
            //END CODE
            _tmrMain.Change(1000, 1000);
        }

        private void EndOfInterval()
        {
            PointInCode("SettingsViewModel: EndOfInterval");

            //TODO Update time
            SendLogMessage("Server will commence calculating new prices", LogType.SETTINGS_VM);
            var locator = base.GetLocator();

            ThreadPool.QueueUserWorkItem(new WaitCallback((object target) =>
            {
                Interval next = PriceCalculation.CalculatePriceUpdates(locator.Orders.AllOrderItems, locator.Interval.Intervals, CurrentInterval.Id, false, this);
                if (next != null)
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        CurrentInterval = next;
                        locator.Interval.SaveIntervals();
                        _server.UpdateInterval(next.ToClientInterval(BeursfuifCurrentTime), BeursfuifCurrentTime);
                        _tmrMain.Change(1000, 1000);
                    }));
                    ThreadPool.QueueUserWorkItem(SaveSettings);
                    SendToastMessage("Update verstuurd");
                    SendLogMessage("Interval update sent to all clients", LogType.SETTINGS_VM);
                }
                else
                {
                    //end of fuif
                    MainActionCommand();
                    locator.Clients.KickAll(KickWasKickedReason.END_OF_FUIF);
                    SendLogMessage("Beursfuif has ended", LogType.SETTINGS_VM | LogType.GOOD_NEWS);
                    SendToastMessage("Beursfuif completed", "De fuif is gedaan");
                    //TODO, disable restart fuif button
                }
            }));
        }

        private void OneMinutePassed()
        {
            PointInCode("SettingsViewModel: OneMinutePassed");

            ThreadPool.QueueUserWorkItem(SaveSettings);
            //TODO save all orders (bin)
            MessengerInstance.Send<AutoSaveAllOrdersMessage>(new AutoSaveAllOrdersMessage());
            SendLogMessage("Auto saved has been completed", LogType.SETTINGS_VM);

            //sync time with clients
            _server.UpdateTime(BeursfuifCurrentTime, CurrentInterval.AuthenticationString());
            SendLogMessage("Server send update current time to clients", LogType.SETTINGS_VM);
            ThreadPool.QueueUserWorkItem(BackupData);
        }

        private void BackupData(object state)
        {
            PointInCode("SettingsViewModel: BackupData");


            if (string.IsNullOrEmpty(BackupLocation))
            {
                SendToastMessage("Can't back up", "De data kon niet worden opgeslaan. Heb je een map gekozen om te backuppen?");
                return;
            }

            if (!Directory.Exists(BackupLocation))
            {
                SendToastMessage("Backup map bestaat niet.", "De backup map bestaat niet (meer)");
                return;
            }

            using (ZipFile zip = new ZipFile())
            {
                // add this map file into the "images" directory in the zip archive
                zip.AddDirectory(PathManager.BEURSFUIF_FOLDER);
                zip.Save(BackupLocation + "\\beursfuif_back_up.zip");
            }

          
        }
        #endregion

        #region Messages
        private void InitMessages()
        {
            PointInCode("SettingsViewModel: InitMessages");

            MessengerInstance.Send<BeursfuifBusyMessage>(new BeursfuifBusyMessage() { Value = this.BeursfuifBusy });
            MessengerInstance.Register<DrinkAvailableMessage>(this, DrinkAvailableMessageReceived);
            MessengerInstance.Register<DrinkModifiedMessage>(this, DrinkModifiedHandler);
        }

        private void DrinkModifiedHandler(DrinkModifiedMessage msg)
        {
            PointInCode("SettingsViewModel: DrinkModifiedHandler");

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
            PointInCode("SettingsViewModel: DrinkAvailableMessageReceived");

            Drink changed = CurrentInterval.Drinks.FirstOrDefault(x => x.Id == msg.DrinkId);
            if (changed != null)
            {
                changed.Available = msg.Available;
                _server.SendDrinkAvailableChanged(msg.DrinkId, (msg.Available ? changed : null), CurrentInterval.Id);
            }
            ThreadPool.QueueUserWorkItem(SaveSettings);
        }
        #endregion

        #region Websocket
        private void InitServer()
        {
            PointInCode("SettingsViewModel: InitServer");

            App.Current.MainWindow.Closing += (a, b) =>
            {
                _server.Active = false;
                _server.DisposeConnection();
                SendLogMessage("Window is closing, shutdown server", LogType.SETTINGS_VM);
            };

            NetworkChange.NetworkAddressChanged += new
            NetworkAddressChangedEventHandler(AddressChangedCallback);
        }

        private void AddressChangedCallback(object sender, EventArgs e)
        {
            RaisePropertyChanged("IPAdress");
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
