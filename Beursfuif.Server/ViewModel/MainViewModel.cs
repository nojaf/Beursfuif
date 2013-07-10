using Beursfuif.Server.UserControls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;

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
        /// <summary>
        /// The <see cref="DrinkVisible" /> property's name.
        /// </summary>
        public const string DrinkVisiblePropertyName = "DrinkVisible";

        private Visibility _drinkVisible = Visibility.Visible;

        /// <summary>
        /// Sets and gets the DrinkVisible property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility DrinkVisible
        {
            get
            {
                return _drinkVisible;
            }

            set
            {
                if (_drinkVisible == value)
                {
                    return;
                }

                RaisePropertyChanging(DrinkVisiblePropertyName);
                _drinkVisible = value;
                RaisePropertyChanged(DrinkVisiblePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IntervalVisible" /> property's name.
        /// </summary>
        public const string IntervalVisiblePropertyName = "IntervalVisible";

        private Visibility _intervalVisible = Visibility.Hidden;

        /// <summary>
        /// Sets and gets the IntervalVisible property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility IntervalVisible
        {
            get
            {
                return _intervalVisible;
            }

            set
            {
                if (_intervalVisible == value)
                {
                    return;
                }

                RaisePropertyChanging(IntervalVisiblePropertyName);
                _intervalVisible = value;
                RaisePropertyChanged(IntervalVisiblePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ClientsVisible" /> property's name.
        /// </summary>
        public const string ClientsVisiblePropertyName = "ClientsVisible";

        private Visibility _clientsVisible = Visibility.Hidden;

        /// <summary>
        /// Sets and gets the ClientsVisible property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility ClientsVisible
        {
            get
            {
                return _clientsVisible;
            }

            set
            {
                if (_clientsVisible == value)
                {
                    return;
                }

                RaisePropertyChanging(ClientsVisiblePropertyName);
                _clientsVisible = value;
                RaisePropertyChanged(ClientsVisiblePropertyName);
            }
        }

        public RelayCommand<string> MenuClicked
        {
            get;
            set;
        }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            MenuClicked = new RelayCommand<string>(MenuClickAction);
        }

        private void MenuClickAction(string menuItem)
        {
            DrinkVisible = Visibility.Collapsed;
            IntervalVisible = Visibility.Collapsed;
            ClientsVisible = Visibility.Collapsed;


            switch (menuItem)
            {
                case "MenuDrink":
                    DrinkVisible = Visibility.Visible;
                    break;
                case "MenuInterval":
                    IntervalVisible = Visibility.Visible;
                    break;
                case "MenuClient":
                    ClientsVisible = Visibility.Visible;
                    break;
            }
            Console.WriteLine(menuItem);
        }
    }
}