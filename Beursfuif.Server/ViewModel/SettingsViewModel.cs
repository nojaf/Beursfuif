using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using Beursfuif.BL.Extensions;
using Beursfuif.Server.Messages;
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
        private System.Threading.Timer _tmrMain;
        private IBeursfuifServer _server;

        /// <summary>
        /// The <see cref="CurrentInterval" /> property's name.
        /// </summary>
        public const string CurrentIntervalPropertyName = "CurrentInterval";

        /// <summary>
        /// Sets and gets the CurrentInterval property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Interval CurrentInterval
        {
            get
            {
                return _beursfuifData.CurrentInterval;
            }

            set
            {
                if (_beursfuifData.CurrentInterval == value)
                {
                    return;
                }

                RaisePropertyChanging(CurrentIntervalPropertyName);
                _beursfuifData.CurrentInterval = value;
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

        /// <summary>
        /// Sets and gets the BeursfuifCurrentTime property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime BeursfuifCurrentTime
        {
            get
            {
                return _beursfuifData.BeursfuifCurrentTime;
            }

            set
            {
                if (_beursfuifData.BeursfuifCurrentTime == value)
                {
                    return;
                }

                RaisePropertyChanging(BeursfuifCurrentTimePropertyName);
                _beursfuifData.BeursfuifCurrentTime = value;
                RaisePropertyChanged(BeursfuifCurrentTimePropertyName);
            }
        }


        /// <summary>
        /// The <see cref="BackupLocation" /> property's name.
        /// </summary>
        public const string BackupLocationPropertyName = "BackupLocation";

        /// <summary>
        /// Sets and gets the BackupLocation property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string BackupLocation
        {
            get
            {
                return _beursfuifData.BackUpLocation;
            }

            set
            {
                if (_beursfuifData.BackUpLocation == value)
                {
                    return;
                }

                RaisePropertyChanging(BackupLocationPropertyName);
                _beursfuifData.BackUpLocation = value;
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

        /// <summary>
        /// The <see cref="Port" /> property's name.
        /// </summary>
        public const string PortPropertyName = "Port";

        /// <summary>
        /// Sets and gets the Port property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Port
        {
            get
            {
                return _beursfuifData.Port;
            }

            set
            {
                if (value < 1000) return;

                if (_beursfuifData.Port == value)
                {
                    return;
                }

                RaisePropertyChanging(PortPropertyName);
                _beursfuifData.Port = value;
                RaisePropertyChanged(PortPropertyName);
            }
        }

        public RelayCommand MainActionButtonCommand { get; set; }

        public RelayCommand AddOneMinute { get; set; }

        public RelayCommand ForceAutoSaveAllOrders { get; set; }

        public RelayCommand AvailableChangedCommand { get; set; }

        public RelayCommand ResetFuifCommand { get; set; }

        public RelayCommand ResetAllCommand { get; set; }

        public RelayCommand ChangeBackupLocationCommand { get; set; }

        public RelayCommand RestoreBackupCommand { get; set; }
        #endregion

        public SettingsViewModel(IBeursfuifData data, IBeursfuifServer server):base(data)
        {
            if (!IsInDesignMode)
            {
                PointInCode("SettingsViewModel: Ctor");
                _server = server;
                _beursfuifData.DataReset += BeursfuifData_DataReset;

                if (Directory.Exists(_beursfuifData.BackUpLocation))
                {
                    RaisePropertyChanged(BackupLocationPropertyName);
                }

                if (_beursfuifData.BeursfuifEverStarted)
                {
                    SendLogMessage("Beursfuif has already started", LogType.SETTINGS_VM);
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

        void BeursfuifData_DataReset(object sender, bool e)
        {
            MainActionButtonContent = PARTY_NEVER_STARTED;
        }

        private void InitCommands()
        {
            PointInCode("SettingsViewModel: InitCommands");

            MainActionButtonCommand = new RelayCommand(MainActionCommand, ValidatePartyConditions);
            AddOneMinute = new RelayCommand(() => { BeursfuifCurrentTime = BeursfuifCurrentTime.AddMinutes(1); OneMinutePassed(); });
            ForceAutoSaveAllOrders = new RelayCommand(() => {
                ThreadPool.QueueUserWorkItem(BackupData);
                MessengerInstance.Send<AutoSaveAllOrdersMessage>(new AutoSaveAllOrdersMessage()); 
            });
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


                    _beursfuifData.RestoreAllData();
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

            if (Port < 1000) return false;

            if (_beursfuifData.IsBeursfuifCompleted) return false;

            return (_beursfuifData.Drinks != null && _beursfuifData.Drinks.Any() && _beursfuifData.Intervals != null && _beursfuifData.Intervals.Any());
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
            _beursfuifData.ChangeBeursfuifBusy(false);
            RaisePropertyChanged(BeursfuifBusyVisibilityPropertyName);

            _tmrMain.Change(Timeout.Infinite, Timeout.Infinite);

            _server.Active = false;
            _server.DisposeConnection();

            MainActionButtonContent = RESUME_PARTY;

            ThreadPool.QueueUserWorkItem(SaveSettings);
            SendToastMessage("Server paused");
        }

        private async void ResumeParty()
        {
            PointInCode("SettingsViewModel: ResumeParty");

            SendLogMessage("Resuming party method", LogType.SETTINGS_VM);
            _beursfuifData.ChangeBeursfuifBusy(true);
            RaisePropertyChanged(BeursfuifBusyVisibilityPropertyName);
            _server.Active = true;

            bool succes = await _server.Start(IPAdress, Port);
            
            if(!succes)
            {
                _server.Active = false;
                _beursfuifData.ChangeBeursfuifBusy(false);
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
            _beursfuifData.ChangeBeursfuifBusy(true);

            RaisePropertyChanged(BeursfuifBusyVisibilityPropertyName);

            //drinks
            var drinks = _beursfuifData.Drinks.Where(x => x.Available).ToArray();

            //fill all intervals
            int intervalCount = _beursfuifData.Intervals.Length;

            for (int i = 0; i < intervalCount; i++)
            {
                FillInDrinks(_beursfuifData.Intervals[i], drinks);
            }

            _beursfuifData.SaveIntervals();

            _beursfuifData.CurrentInterval = _beursfuifData.Intervals[0];

            _beursfuifData.BeursfuifCurrentTime = CurrentInterval.StartTime;

            //start timer
            _tmrMain = new System.Threading.Timer(MainTimer_Tick, null, 1000, 1000);

            _server.Start(IPAdress, Port);
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
            SaveSettings settings = new SaveSettings(BeursfuifBusy, BeursfuifCurrentTime, Port,_beursfuifData.IsBeursfuifCompleted , BackupLocation);
            _beursfuifData.SaveSettings(settings);
            _beursfuifData.SaveCurrentInterval();
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
                        _beursfuifData.ResetAll();
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
                        _beursfuifData.ResetData();
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

            _tmrMain.Change(1000, 1000);
        }

        private void EndOfInterval()
        {
            PointInCode("SettingsViewModel: EndOfInterval");
            //Save all data first just in case
            ThreadPool.QueueUserWorkItem(BackupData);
            ThreadPool.QueueUserWorkItem(SaveSettings);
            ThreadPool.QueueUserWorkItem(_beursfuifData.SaveAllData);

            //TODO Update time
            SendLogMessage("Server will commence calculating new prices", LogType.SETTINGS_VM);

            ThreadPool.QueueUserWorkItem(new WaitCallback((object target) =>
            {
                Interval next = PriceCalculation.CalculatePriceUpdates(_beursfuifData.AllOrderItems, _beursfuifData.Intervals, _beursfuifData.CurrentInterval.Id, false, this);
                if (next != null)
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        CurrentInterval = next;
                        _beursfuifData.SaveIntervals();
                        _server.UpdateInterval(next.ToClientInterval(BeursfuifCurrentTime, PathManager.ASSETS_PATH), BeursfuifCurrentTime);
                        _tmrMain.Change(1000, 1000);
                    }));
                    ThreadPool.QueueUserWorkItem(SaveSettings);
                    SendToastMessage("Update verstuurd");
                    SendLogMessage("Interval update sent to all clients", LogType.SETTINGS_VM);
                }
                else
                {
                    //end of fuif
                    EndOfBeursfuif();
                }
            }));
        }

        private void EndOfBeursfuif()
        {
            MainActionCommand();

            foreach (var client in _beursfuifData.Clients)
            {
                KickClientMessage kickClientMessage = new KickClientMessage()
                {
                    Reason = KickWasKickedReason.END_OF_PARTY,
                    ClientId = client.Id
                };
                base.MessengerInstance.Send<KickClientMessage>(kickClientMessage);
            }

            SendLogMessage("Beursfuif has ended", LogType.SETTINGS_VM | LogType.GOOD_NEWS);
            SendToastMessage("Beursfuif completed", "De fuif is gedaan");
            //TODO, disable restart fuif button

            _beursfuifData.EndOfBeursfuif();
            MainActionButtonCommand.CanExecute(true);
        }

        private void OneMinutePassed()
        {
            PointInCode("SettingsViewModel: OneMinutePassed");

            ThreadPool.QueueUserWorkItem(SaveSettings);

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

            var intervals = _beursfuifData.Intervals;
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
            ThreadPool.QueueUserWorkItem(SaveSettings);
            ClientInterval clientInterval = this.CurrentInterval.ToClientInterval(this.BeursfuifCurrentTime, PathManager.ASSETS_PATH);
            _server.SendDrinkAvailableChanged(clientInterval);
        }
        #endregion

        #region Server
        private void InitServer()
        {
            PointInCode("SettingsViewModel: InitServer");

            if (App.Current != null)
            {
                App.Current.MainWindow.Closing += (a, b) =>
                {
                    _server.Active = false;
                    _server.DisposeConnection();
                    SendLogMessage("Window is closing, shutdown server", LogType.SETTINGS_VM);
                };

            }

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
