using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using Beursfuif.Server.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;

namespace Beursfuif.Server.ViewModel
{
    public class DrinkViewModel : BeursfuifViewModelBase
    {
        #region fields and properties
        private IOManager _ioManager;

        /// <summary>
        /// The <see cref="Drinks" /> property's name.
        /// </summary>
        public const string DrinksPropertyName = "Drinks";

        private ObservableCollection<Drink> _drinks = null;

        /// <summary>
        /// Sets and gets the Drinks property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Drink> Drinks
        {
            get
            {
                return _drinks;
            }

            set
            {
                if (_drinks == value)
                {
                    return;
                }

                RaisePropertyChanging(DrinksPropertyName);
                _drinks = value;
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
                    _newEditDrink = null;
                    DownloadUrl = string.Empty;
                    RaisePropertyChanged(NewEditDrinkPropertyName);
                }

                RaisePropertyChanging(NewEditDrinkPropertyName);

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

        public RelayCommand<int> RemoveDrinkCommand { get; set; }

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

        public RelayCommand DownloadImageCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public RelayCommand ChooseLocalImageCommand { get; set; }

        public RelayCommand SaveDrinkCommand { get; set; }

        public RelayCommand<int> AvailableChangedCommand {get;set;}               
        #endregion

        public DrinkViewModel(IOManager iomanager)
        {
            _ioManager = iomanager;
            Drinks = iomanager.LoadObservableCollectionFromXml<Drink>(PathManager.DRINK_XML_PATH);
            ThreadPool.QueueUserWorkItem(CleanUpImages);
            
            InitCommands();
        }

        private void CleanUpImages(object state)
        {
            string[] paths = Directory.GetFiles(PathManager.ASSETS_PATH);
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
                }
            }
        }

        private void InitCommands()
        {
            //It shouldn't be posible to remove a drink when the party is busy
            RemoveDrinkCommand = new RelayCommand<int>(DeleteDrink, (int id) => { return (!BeursfuifBusy && Drinks.Any(x => x.Id == id)); });
            AddNewDrinkCommand = new RelayCommand(delegate() { NewEditDrink = new Drink() { Id = Drinks.Max(x => x.Id) + 1 }; },
                () => { return (!BeursfuifBusy && NewEditDrink == null); });
            DownloadImageCommand = new RelayCommand(DownloadImage, () => { return (!string.IsNullOrEmpty(DownloadUrl) && !Downloading); });
            CancelCommand = new RelayCommand(ResetValues);
            ChooseLocalImageCommand = new RelayCommand(ChooseLocalImage);
            SaveDrinkCommand = new RelayCommand(SaveDrink);
            AvailableChangedCommand = new RelayCommand<int>(UpdateDrinkAvailability);
        }

        private bool ValidateDrink()
        {
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
            }

            return valid;
        }

        void ErrorMessage_AnswerChanged(object sender, AnswerChangedArgs e)
        {
            Console.WriteLine("You answered " + e);
            _dm.AnswerChanged -= ErrorMessage_AnswerChanged;
        }

        private void SaveDrink()
        {
            if (ValidateDrink())
            {
                NewEditDrink.CurrentPrice = NewEditDrink.InitialPrice;
                if (!Drinks.Any(x => x.Id == NewEditDrink.Id))
                {
                    Drinks.Add(NewEditDrink);
                }
                ThreadPool.QueueUserWorkItem(new WaitCallback((object target) =>
                {
                    _ioManager.SaveObservableCollectionToXml(PathManager.DRINK_XML_PATH, Drinks);
                }));
                MessengerInstance.Send<ToastMessage>(new ToastMessage("Dranken saved"));
                previousDrinkSaved = true;
                NewEditDrink = null;
                DownloadUrl = string.Empty;
               
            }
        }

        private void ChooseLocalImage()
        {
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
                    NewEditDrink.ImageString = destinationPath;
                }));
            }

        }

        private void ResetValues()
        {
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _dm = new DialogMessage("Downloaden mislukt");
                    _dm.Nay = Visibility.Collapsed;
                    _dm.Errors.Add("Geen geldige url of geen internettoegang");
                    MessengerInstance.Send<DialogMessage>(_dm);
                }
            }));
            Downloading = false;
        }

        private void ResizeImage(string originalPath, string destinationPath)
        {
            //Crop to square format
            ImageResizer.ImageJob i = new ImageResizer.ImageJob(originalPath, destinationPath, new ImageResizer.ResizeSettings(
               "height=100;format=png;mode=max;"));
            i.CreateParentDirectory = true; //Auto-create the uploads directory.
            i.Build();
            NewEditDrink.ImageString = destinationPath;
        }

        private void DeleteDrink(int id)
        {
            Drink drink = Drinks.FirstOrDefault(x => x.Id == id);
            if (drink != null)
            {
                Drinks.Remove(drink);
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state)
                {
                    _ioManager.SaveObservableCollectionToXml<Drink>(PathManager.DRINK_XML_PATH, Drinks);
                }), null);
                if (NewEditDrink != null && NewEditDrink.Id == id) NewEditDrink = null;

            }
        }

        private void DownloadRemoteImageFile(string uri, string path)
        {
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

        private void UpdateDrinkAvailability(int id)
        {
            var locator = base.GetLocator();
            Drink changed = Drinks.FirstOrDefault(x => x.Id == id);
            Interval currentInterval = locator.Settings.CurrentInterval;

            var intervals = locator.Interval.Intervals;
            int length = intervals.Length;
            int index = Array.IndexOf(intervals, currentInterval);
            for (int i = index; i < length; i++)
            {
                Drink drink = intervals[i].Drinks.FirstOrDefault(x => x.Id == id);
                if (drink != null) drink.Available = changed.Available;
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state)
            {
                _ioManager.SaveObservableCollectionToXml<Drink>(PathManager.DRINK_XML_PATH, Drinks);
            }), null);

            if (BeursfuifBusy)
            {
                //send message to SettingsViewmodel to update the clients that a drink is no longer available
                MessengerInstance.Send<DrinkAvailableMessage>(new DrinkAvailableMessage() { Available = changed.Available, DrinkId = changed.Id });
            }

            MessengerInstance.Send<ToastMessage>(new ToastMessage(changed.Name + " is "+(changed.Available ? "weer" : "niet meer")+" beschikbaar"));
   
        }

    }
}
