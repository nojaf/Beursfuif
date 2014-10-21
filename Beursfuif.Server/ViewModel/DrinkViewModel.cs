using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using Beursfuif.Server.Messages;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using Beursfuif.BL.Extensions;
using Beursfuif.Server.Services;

namespace Beursfuif.Server.ViewModel
{
    public class DrinkViewModel : BeursfuifViewModelBase
    {
        #region fields and properties
        /// <summary>
        /// The <see cref="Drinks" /> property's name.
        /// </summary>
        public const string DrinksPropertyName = "Drinks";


        /// <summary>
        /// Sets and gets the Drinks property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Drink> Drinks
        {
            get
            {
                return _beursfuifData.Drinks;
            }

            set
            {
                if (_beursfuifData.Drinks == value)
                {
                    return;
                }

                RaisePropertyChanging(DrinksPropertyName);
                _beursfuifData.Drinks = value;
                RaisePropertyChanged(DrinksPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="NewEditDrink" /> property's name.
        /// </summary>
        public const string NewEditDrinkPropertyName = "NewEditDrink";

        private Drink _previousValuesOfEditDrink = new Drink();
        private Drink _newEditDrink = null;

        private bool previousDrinkSaved = false;

        /// <summary>
        /// Sets and gets the NewEditDrink property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Drink NewEditDrink
        {
            get
            {
                return _newEditDrink;
            }

            set
            {
                if (_newEditDrink == value)
                {
                    return;
                }

                if (_newEditDrink != null && !previousDrinkSaved)
                {
                    _newEditDrink.Name = _previousValuesOfEditDrink.Name;
                    _newEditDrink.InitialPrice = _previousValuesOfEditDrink.InitialPrice;
                    _newEditDrink.MiniumPrice = _previousValuesOfEditDrink.MiniumPrice;
                    _newEditDrink.MaximumPrice = _previousValuesOfEditDrink.MaximumPrice;
                    _newEditDrink.ImageString = _previousValuesOfEditDrink.ImageString;
                    _newEditDrink.PropertyChanged -= NewEditDrink_PropertyChanged;
                    _newEditDrink = null;
                    DownloadUrl = string.Empty;
                    RaisePropertyChanged(NewEditDrinkPropertyName);
                }

                RaisePropertyChanging(NewEditDrinkPropertyName);

         
                if (value != null)
                {
                    value.PropertyChanged += NewEditDrink_PropertyChanged;
                }
                else if(_newEditDrink != null)
                {
                    _newEditDrink.PropertyChanged -= NewEditDrink_PropertyChanged;
                }


                _newEditDrink = value;

                //previous values
                if (_newEditDrink != null)
                {
                    _previousValuesOfEditDrink.Name = _newEditDrink.Name;
                    _previousValuesOfEditDrink.InitialPrice = _newEditDrink.InitialPrice;
                    _previousValuesOfEditDrink.MiniumPrice = _newEditDrink.MiniumPrice;
                    _previousValuesOfEditDrink.MaximumPrice = _newEditDrink.MaximumPrice;
                    _previousValuesOfEditDrink.ImageString = _newEditDrink.ImageString;
                }

                RaisePropertyChanged(NewEditDrinkPropertyName);
                RaisePropertyChanged(AddOrEditMenuVisiblePropertyName);
                previousDrinkSaved = false;
            }
        }



        public const string AddOrEditMenuVisiblePropertyName = "AddOrEditMenuVisible";
        public Visibility AddOrEditMenuVisible
        {
            get
            {
                return (NewEditDrink != null ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public RelayCommand AddNewDrinkCommand { set; get; }

        /// <summary>
        /// The <see cref="DownloadUrl" /> property's name.
        /// </summary>
        public const string DownloadUrlPropertyName = "DownloadUrl";

        private string _downloadUrl = string.Empty;

        /// <summary>
        /// Sets and gets the DownloadUrl property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DownloadUrl
        {
            get
            {
                return _downloadUrl;
            }

            set
            {
                if (_downloadUrl == value)
                {
                    return;
                }

                RaisePropertyChanging(DownloadUrlPropertyName);
                _downloadUrl = value;
                RaisePropertyChanged(DownloadUrlPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Downloading" /> property's name.
        /// </summary>
        public const string DownloadingPropertyName = "Downloading";

        private bool _downloading = false;

        /// <summary>
        /// Sets and gets the Downloading property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool Downloading
        {
            get
            {
                return _downloading;
            }

            set
            {
                if (_downloading == value)
                {
                    return;
                }

                RaisePropertyChanging(DownloadingPropertyName);
                _downloading = value;
                RaisePropertyChanged(DownloadingPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CanModify" /> property's name.
        /// </summary>
        public const string CantModifyPropertyName = "CanModify";

        private bool _canModify = true;

        /// <summary>
        /// Sets and gets the CantModify property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool CanModify
        {
            get
            {
                return _canModify;
            }

            set
            {
                if (_canModify == value)
                {
                    return;
                }

                RaisePropertyChanging(CantModifyPropertyName);
                _canModify = value;
                RaisePropertyChanged(CantModifyPropertyName);
            }
        }

        public RelayCommand DownloadImageCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public RelayCommand ChooseLocalImageCommand { get; set; }

        public RelayCommand SaveDrinkCommand { get; set; }

        public RelayCommand<int> AvailableChangedCommand { get; set; }

        public RelayCommand<int> RemoveDrinkCommand { get; set; }
        #endregion

        public DrinkViewModel(IBeursfuifData beursfuifData):base(beursfuifData)
        {
            if (!IsInDesignMode)
            {
                PointInCode("DrinkViewModel: Ctor");
                _beursfuifData = beursfuifData;

                if (Drinks == null) Drinks = new ObservableCollection<Drink>();
                ThreadPool.QueueUserWorkItem(CleanUpImages);

                InitCommands();
                CheckCanEdit();

                _beursfuifData.BeursfuifBusyChanged += BeursfuifData_BeursfuifBusyChanged;
                _beursfuifData.DataReset += BeursfuifData_DataReset;
                _beursfuifData.BeursfuifDataImported += BeursfuifData_BeursfuifDataImported;
            }
        }

        void BeursfuifData_BeursfuifDataImported(object sender, EventArgs e)
        {
            RaisePropertyChanged(DrinksPropertyName);
            CheckCanEdit();
        }

        void BeursfuifData_DataReset(object sender, bool e)
        {
            if (e)
            {
                RaisePropertyChanged(DrinksPropertyName);
            }
            CheckCanEdit();
        }

        void BeursfuifData_BeursfuifBusyChanged(object sender, bool e)
        {
            RaisePropertyChanged(BeursfuifBusyPropertyName);
            CheckCanEdit();
        }

        private void CleanUpImages(object state)
        {
            PointInCode("DrinkViewModel: CleanUpImages");
            try
            {
                string[] paths = Directory.GetFiles(PathManager.ASSETS_PATH).Select(x => x.Replace(PathManager.ASSETS_PATH, "")).ToArray();
                string[] images = Drinks.Select(x => x.ImageString).ToArray();
                var query = from path in paths
                            where !images.Contains(path)
                            select path;

                foreach (var path in query)
                {
                    //Remove images
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }else if(File.Exists(PathManager.ASSETS_PATH + path)){
                        File.Delete(PathManager.ASSETS_PATH + path);
                    }
                }
                SendLogMessage("Unused images removed", LogType.DRINK_VM);
            }
            catch (Exception)
            {
                Debug.WriteLine("No images cleared");
            }

        }

        private void InitCommands()
        {
            PointInCode("DrinkViewModel: InitCommands");
            //It shouldn't be posible to remove a drink when the party is busy
            RemoveDrinkCommand = new RelayCommand<int>(DeleteDrink, (int id) => { return (!BeursfuifBusy && Drinks.Any(x => x.Id == id)); });
            AddNewDrinkCommand = new RelayCommand(delegate() { NewEditDrink = new Drink() { Id = (Drinks != null && Drinks.Any() ? Drinks.Max(x => x.Id) + 1 : 1) }; },
                () => { return (!BeursfuifBusy && NewEditDrink == null); });
            DownloadImageCommand = new RelayCommand(DownloadImage, () => { return (!string.IsNullOrEmpty(DownloadUrl) && !Downloading); });
            CancelCommand = new RelayCommand(ResetValues);
            ChooseLocalImageCommand = new RelayCommand(ChooseLocalImage);
            SaveDrinkCommand = new RelayCommand(SaveDrink);
            AvailableChangedCommand = new RelayCommand<int>(UpdateDrinkAvailability);

        }

        void NewEditDrink_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (NewEditDrink != null && NewEditDrink.Id != 0)
            {
                UpdateDrink(NewEditDrink);
            }
        }

        private bool ValidateDrink()
        {
            PointInCode("DrinkViewModel: ValidateDrink");

            bool valid = true;
            _dm = new DialogMessage("Drank niet in orde");
            _dm.Nay = Visibility.Collapsed;

            if (string.IsNullOrEmpty(NewEditDrink.Name))
            {
                valid = false;
                _dm.Errors.Add("De naam mag niet leeg zijn!");
            }

            if (NewEditDrink.InitialPrice < NewEditDrink.MiniumPrice)
            {
                valid = false;
                _dm.Errors.Add("De beginprijs mag niet onder de minimumprijs liggen.");
            }

            if (NewEditDrink.MaximumPrice < NewEditDrink.InitialPrice)
            {
                valid = false;
                _dm.Errors.Add("De beginprijs mag niet boven de maximumprijs liggen.");
            }

            if (NewEditDrink.MiniumPrice > NewEditDrink.MaximumPrice)
            {
                valid = false;
                _dm.Errors.Add("De minimumprijs mag niet boven de maximumprijs liggen.");
            }

            if (!valid)
            {
                _dm.AnswerChanged += ErrorMessage_AnswerChanged;
                MessengerInstance.Send<DialogMessage>(_dm);
                SendLogMessage("Drink not valid, " + string.Join(";", _dm.Errors.ToArray()), LogType.USER_ERROR | LogType.DRINK_VM);
            }

            return valid;
        }

        void ErrorMessage_AnswerChanged(object sender, AnswerChangedArgs e)
        {
            PointInCode("DrinkViewModel: ErrorMessage_AnswerChanged");
            _dm.AnswerChanged -= ErrorMessage_AnswerChanged;
        }

        private void SaveDrink()
        {
            PointInCode("DrinkViewModel: SaveDrink");

            if (ValidateDrink())
            {
                NewEditDrink.CurrentPrice = NewEditDrink.InitialPrice;
                if (!Drinks.Any(x => x.Id == NewEditDrink.Id))
                {
                    Drinks.Add(NewEditDrink);
                    MessengerInstance.Send<DrinkModifiedMessage>(new DrinkModifiedMessage()
                    {
                        Added = true,
                        Drink = NewEditDrink
                    });
                    SendLogMessage("Drink added", LogType.DRINK_VM);
                }
                else
                {
                    MessengerInstance.Send<DrinkModifiedMessage>(new DrinkModifiedMessage()
                    {
                        Changed = true,
                        Drink = NewEditDrink
                    });
                   SendLogMessage("Drink modified", LogType.DRINK_VM);
                }

                ThreadPool.QueueUserWorkItem(new WaitCallback((object target) =>
                {
                    _beursfuifData.SaveDrinks();
                }));
                SendToastMessage("Dranken saved");
                previousDrinkSaved = true;
                NewEditDrink = null;
                DownloadUrl = string.Empty;

            }
        }

        #region Images
        private void ChooseLocalImage()
        {
            PointInCode("DrinkViewModel: ChooseLocalImage");

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image Files (*.bmp, *.jpg, *.png, *.gif)|*.bmp;*.jpg;*.png;*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((object target) =>
                {
                    // Open document 
                    string filename = dlg.FileName;
                    string dateStamp = DateTime.Now.ToString("dd-MM-yyyy-hhmmss");
                    string originalPath = PathManager.ASSETS_PATH + dateStamp + "-original" + Path.GetExtension(filename);
                    string destinationPath = PathManager.ASSETS_PATH + dateStamp + ".png";
                    File.Copy(filename, originalPath, true);
                    ResizeImage(originalPath, destinationPath);
                    NewEditDrink.ImageString = destinationPath.Replace(PathManager.ASSETS_PATH, string.Empty);
                    SendLogMessage("Image for drink from file added", LogType.DRINK_VM);
                }));
            }

        }

        private void ResetValues()
        {
            PointInCode("DrinkViewModel: ResetValues");

            NewEditDrink.Name = _previousValuesOfEditDrink.Name;
            NewEditDrink.InitialPrice = _previousValuesOfEditDrink.InitialPrice;
            NewEditDrink.MiniumPrice = _previousValuesOfEditDrink.MiniumPrice;
            NewEditDrink.MaximumPrice = _previousValuesOfEditDrink.MaximumPrice;
            NewEditDrink.ImageString = _previousValuesOfEditDrink.ImageString;
            NewEditDrink = null;
            DownloadUrl = string.Empty;
        }

        private void DownloadImage()
        {
            PointInCode("DrinkViewModel: DownloadImage");

            Downloading = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback((object target) =>
            {
                try
                {

                    string dateStamp = DateTime.Now.ToString("dd-MM-yyyy-hhmmss");
                    string originalPath = PathManager.ASSETS_PATH + dateStamp + "-original" + Path.GetExtension(this.DownloadUrl);
                    string destinationPath = PathManager.ASSETS_PATH + dateStamp + ".png";
                    //Download image
                    DownloadRemoteImageFile(this.DownloadUrl, originalPath);
                    ResizeImage(originalPath, destinationPath);
                    SendLogMessage("Image from internet downloaded and used for drink", LogType.DRINK_VM);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _dm = new DialogMessage("Downloaden mislukt");
                    _dm.Nay = Visibility.Collapsed;
                    _dm.Errors.Add("Geen geldige url of geen internettoegang");
                    MessengerInstance.Send<DialogMessage>(_dm);
                    SendLogMessage("Image download failed, ex = " + ex.Message, LogType.ERROR | LogType.DRINK_VM);
                }
            }));
            Downloading = false;
        }

        private void ResizeImage(string originalPath, string destinationPath)
        {
            PointInCode("DrinkViewModel: ResizeImage");

            //Crop to square format
            ImageResizer.ImageJob i = new ImageResizer.ImageJob(originalPath, destinationPath, new ImageResizer.ResizeSettings(
               "height=200;format=png;mode=max;"));
            i.CreateParentDirectory = true; //Auto-create the uploads directory.
            i.Build();
            Uri assets = new Uri(PathManager.ASSETS_PATH);
            Uri image = new Uri(destinationPath);
            NewEditDrink.ImageString = assets.MakeRelativeUri(image).ToString();
        }
        #endregion

        private void DeleteDrink(int id)
        {
            PointInCode("DrinkViewModel: DeleteDrink");

            Drink drink = Drinks.FirstOrDefault(x => x.Id == id);
            if (drink != null)
            {
                MessengerInstance.Send<DrinkModifiedMessage>(new DrinkModifiedMessage()
                {
                    Drink = drink
                });

                SendToastMessage(drink.Name + " verwijderd");
                SendLogMessage("Drink " + drink.Name + "[" + drink.Id + "] removed", LogType.DRINK_VM);


                Drinks.Remove(drink);
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state)
                {
                    _beursfuifData.SaveDrinks();
                }), null);
                if (NewEditDrink != null && NewEditDrink.Id == id) NewEditDrink = null;

            }
        }

        private void DownloadRemoteImageFile(string uri, string path)
        {
            PointInCode("DrinkViewModel: DownloadRemoteImageFile");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {

                // if the remote file was found, download oit
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(path))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
            }
        }

        protected virtual void UpdateDrinkAvailability(int id)
        {
            PointInCode("DrinkViewModel: UpdateDrinkAvailability");
            Drink changed = Drinks.FirstOrDefault(x => x.Id == id);
            Interval currentInterval = _beursfuifData.CurrentInterval;

            UpdateDrink(changed);

            SendToastMessage(string.Format("{0}  is {1} beschikbaar", changed.Name, (changed.Available ? "weer" : "niet meer")));
            SendLogMessage(string.Format("Drink ({0}) available ({1}) changed",changed.Name ,changed.Available), LogType.DRINK_VM);
        }

        private void UpdateDrink(Drink changed)
        {
            Interval currentInterval = _beursfuifData.CurrentInterval;

            if (currentInterval != null && currentInterval.Drinks != null)
            {
                if (currentInterval.Drinks.Any(x => x.Id == changed.Id))
                {
                    Drink currentIntervalDrink = currentInterval.Drinks.FirstOrDefault(x => x.Id == changed.Id);
                    currentIntervalDrink.UpdateProperties(changed);
                }
            }

            if (_beursfuifData.Intervals != null)
            {
                int length = _beursfuifData.Intervals.Length;
                int index = Array.IndexOf(_beursfuifData.Intervals, currentInterval);
                for (int i = index; i < length; i++)
                {
                    Drink drink = _beursfuifData.Intervals[i].Drinks.FirstOrDefault(x => x.Id == changed.Id);
                    if (drink != null)
                    {
                        drink.UpdateProperties(changed);
                    }
                }
            }

            if (BeursfuifBusy)
            {
                //send message to clients with the current interval
                MessengerInstance.Send<DrinkAvailableMessage>(new DrinkAvailableMessage());
            }
        }


        private void CheckCanEdit()
        {
            CanModify = !_beursfuifData.BeursfuifEverStarted;  
        }
    }
}
