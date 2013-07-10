using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.ViewModel
{
    public class DrinkViewModel:ViewModelBase
    {
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

        private Drink _newEditDrink = new Drink();

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
                RaisePropertyChanged(NewEditDrinkPropertyName);
            }
        }

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
            }

        }

    }
}
