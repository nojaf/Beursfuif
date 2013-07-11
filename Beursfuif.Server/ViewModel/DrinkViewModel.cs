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
    public class DrinkViewModel : ViewModelBase
    {
        #region fields and properties
        private IOManager _ioManager;
        private IStateChange _stateChanger;

        private const string FADE_IN = "FadeIn";
        private const string FADE_OUT = "FadeOut";
        private bool _visible = true;
        private  ErrorMessage _em = new Beursfuif.Server.Messages.ErrorMessage();

         

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

                RaisePropertyChanging(NewEditDrinkPropertyName);
                _newEditDrink = value;
                //previous values
                if (_newEditDrink != null)
                {
                    _previousValuesOfEditDrink.Name = _newEditDrink.Name;
                    _previousValuesOfEditDrink.InitialPrice = _newEditDrink.InitialPrice;
                    _previousValuesOfEditDrink.MiniumPrice = _newEditDrink.MiniumPrice;
                    _previousValuesOfEditDrink.MaximumPrice = _newEditDrink.MiniumPrice;
                    _previousValuesOfEditDrink.ImageString = _newEditDrink.ImageString;
                }

                RaisePropertyChanged(NewEditDrinkPropertyName);
                RaisePropertyChanged(AddOrEditMenuVisiblePropertyName);
                
            }
        }

        public RelayCommand<int> RemoveDrink { get; set; }

        /// <summary>
        /// The <see cref="BeursfuifBusy" /> property's name.
        /// </summary>
        public const string BeursfuifBusyPropertyName = "BeursfuifBusy";

        private bool _beursfuifBusy = false;

        /// <summary>
        /// Sets and gets the BeursfuifBusy property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool BeursfuifBusy
        {
            get
            {
                return _beursfuifBusy;
            }

            set
            {
                if (_beursfuifBusy == value)
                {
                    return;
                }

                RaisePropertyChanging(BeursfuifBusyPropertyName);
                _beursfuifBusy = value;
                RaisePropertyChanged(BeursfuifBusyPropertyName);
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

        public RelayCommand DownloadImageCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public RelayCommand ChooseLocalImageCommand { get; set; }

        public RelayCommand SaveDrinkCommand { get; set; }
        #endregion

        public DrinkViewModel(IOManager iomanager)
        {
            if (IsInDesignMode)
            {
                var dummyService = new DummyDrinkService();
                Drinks = dummyService.GetDrinksFromXml();
            }
            else
            {
                _ioManager = iomanager;
                Drinks = iomanager.LoadObservableCollectionFromXml<Drink>(PathManager.DRINK_XML_PATH);
                CleanUpImages();
            }

            InitMessages();
            InitCommands();

        }

        private void CleanUpImages()
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
            RemoveDrink = new RelayCommand<int>(DeleteDrink, (int id) => { return (!BeursfuifBusy && Drinks.Any(x => x.Id == id)); });
            AddNewDrinkCommand = new RelayCommand(delegate() { NewEditDrink = new Drink() { Id = Drinks.Max(x => x.Id) + 1 }; },
                () => { return (NewEditDrink == null); });
            DownloadImageCommand = new RelayCommand(DownloadImage, () => { return !string.IsNullOrEmpty(DownloadUrl); });
            CancelCommand = new RelayCommand(ResetValues);
            ChooseLocalImageCommand = new RelayCommand(ChooseLocalImage);
            SaveDrinkCommand = new RelayCommand(SaveDrink);
        }

        private bool ValidateDrink()
        {
            bool valid = true;
            _em = new ErrorMessage("Drank niet in orde");
            _em.Nay = Visibility.Collapsed;

            if (string.IsNullOrEmpty(NewEditDrink.Name))
            {
                valid = false;
                _em.Errors.Add("De naam mag niet leeg zijn!");
            }

            if (NewEditDrink.InitialPrice < NewEditDrink.MiniumPrice)
            {
                valid = false;
                _em.Errors.Add("De beginprijs mag niet onder de minimumprijs liggen.");
            }

            if (NewEditDrink.MaximumPrice < NewEditDrink.InitialPrice)
            {
                valid = false;
                _em.Errors.Add("De beginprijs mag niet boven de maximumprijs liggen.");
            }

            if (NewEditDrink.MiniumPrice > NewEditDrink.MaximumPrice)
            {
                valid = false;
                _em.Errors.Add("De minimumprijs mag niet boven de maximumprijs liggen.");
            }

            if (!valid)
            {
                _em.AnswerChanged += ErrorMessage_AnswerChanged;
                MessengerInstance.Send<ErrorMessage>(_em);
            }

            return valid;
        }

        void ErrorMessage_AnswerChanged(object sender, AnswerChangedArgs e)
        {
            Console.WriteLine("You answered " + e);
            _em.AnswerChanged -= ErrorMessage_AnswerChanged;
        }

        private void SaveDrink()
        {
            if (ValidateDrink())
            {
                if (!Drinks.Any(x => x.Id == NewEditDrink.Id))
                {
                    Drinks.Add(NewEditDrink);
                }
                ThreadPool.QueueUserWorkItem(new WaitCallback((object target) =>
                {
                    _ioManager.SaveObservableCollectionToXml(PathManager.DRINK_XML_PATH, Drinks);
                }));
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
                    string destinationPath = PathManager.ASSETS_PATH  + dateStamp + Path.GetExtension(filename);
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
            ThreadPool.QueueUserWorkItem(new WaitCallback((object target) => {
                try
                {
                    string dateStamp = DateTime.Now.ToString("dd-MM-yyyy-hhmmss");
                    string originalPath = PathManager.ASSETS_PATH + dateStamp + "-original" + Path.GetExtension(this.DownloadUrl);
                    string destinationPath = PathManager.ASSETS_PATH + dateStamp + Path.GetExtension(this.DownloadUrl);
                    //Download image
                    DownloadRemoteImageFile(this.DownloadUrl, originalPath);
                    ResizeImage(originalPath, destinationPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show("Geen geldige url of geen internettoegang");
                }
            }));

        }

        private void ResizeImage(string originalPath, string destinationPath)
        {
            //Crop to square format
            ImageResizer.ImageJob i = new ImageResizer.ImageJob(originalPath, destinationPath, new ImageResizer.ResizeSettings(
               "height=100;format=" + Path.GetExtension(this.DownloadUrl) + ";mode=max;"));
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

        private void InitMessages()
        {
            MessengerInstance.Register<ChangeVisibilityMessage>(this, ChangeState);
        }

        private void ChangeState(ChangeVisibilityMessage message)
        {
            if (!_visible && message.ClassName == typeof(DrinkViewModel).Name)
            {
                _stateChanger.GoToState(FADE_IN);
                _visible = true;
            }
            else if (_visible && message.ClassName != typeof(DrinkViewModel).Name)
            {
                _stateChanger.GoToState(FADE_OUT);
                _visible = false;
            }

        }

        public void SetStateChanger(IStateChange drinkView)
        {
            _stateChanger = drinkView;
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
    }
}
