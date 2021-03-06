using Beursfuif.Server.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;

namespace Beursfuif.Server.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Fields and properties
        private IStateChange _stateChanger;
        private IToastManager _toastManager;
        /// <summary>
        /// The <see cref="ErrorMessage" /> property's name.
        /// </summary>
        public const string ErrorMessagePropertyName = "ErrorMessage";

        private DialogMessage _errorMessage = null;

        /// <summary>
        /// Sets and gets the ErrorMessage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DialogMessage ErrorMessage
        {
            get
            {
                return _errorMessage;
            }

            set
            {
                if (_errorMessage == value)
                {
                    return;
                }

                RaisePropertyChanging(ErrorMessagePropertyName);
                _errorMessage = value;
                RaisePropertyChanged(ErrorMessagePropertyName);
            }
        }

        public RelayCommand<string> MenuClicked
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            InitCommands();
            MessengerInstance.Register<DialogMessage>(this,HandleErrorMessages);
            MessengerInstance.Register<ToastMessage>(this, HandleToasts);
        }

        private void HandleToasts(ToastMessage obj)
        {
            if (_toastManager != null)
            {
                _toastManager.ShowToast(obj.Title, obj.Message);
            }
        }

        private void HandleErrorMessages(DialogMessage em)
        {
            ErrorMessage = em;
            _stateChanger.GoToState("FadeIn", true,"messageGrid");
        }

        private void InitCommands()
        {
            MenuClicked = new RelayCommand<string>(MenuClickAction);
        }

        private void MenuClickAction(string menuItem)
        {

            switch (menuItem)
            {
                case "MenuDrink":
                    MessengerInstance.Send<ChangeVisibilityMessage>(new ChangeVisibilityMessage(typeof(DrinkViewModel).Name));
                    break;
                case "MenuInterval":
                    MessengerInstance.Send<ChangeVisibilityMessage>(new ChangeVisibilityMessage(typeof(IntervalViewModel).Name));
                    break;
                case "MenuClient":
                    MessengerInstance.Send<ChangeVisibilityMessage>(new ChangeVisibilityMessage(typeof(ClientsViewModel).Name));
                    break;
                case "MenuSetting":
                    MessengerInstance.Send<ChangeVisibilityMessage>(new ChangeVisibilityMessage(typeof(SettingsViewModel).Name));
                    break;
                case "MenuOrder":
                    MessengerInstance.Send<ChangeVisibilityMessage>(new ChangeVisibilityMessage(typeof(OrdersViewModel).Name));
                    break;
                case "MenuLog":
                    MessengerInstance.Send<ChangeVisibilityMessage>(new ChangeVisibilityMessage(typeof(LogViewModel).Name));
                    break;
                case "MenuPredict":
                    MessengerInstance.Send<ChangeVisibilityMessage>(new ChangeVisibilityMessage(typeof(PredictionViewModel).Name));
                    break;
            }
            Console.WriteLine(menuItem);
        }

        public void SetStateChanger(IStateChange mainWindow)
        {
            _stateChanger = mainWindow;
        }

        public void SetToastManager(IToastManager mainWindow)
        {
            _toastManager = mainWindow;
        }
    }
}