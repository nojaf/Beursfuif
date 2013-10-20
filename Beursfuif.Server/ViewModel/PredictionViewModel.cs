using Beursfuif.BL;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.ViewModel
{
    public class PredictionViewModel: BeursfuifViewModelBase
    {
        /// <summary>
        /// The <see cref="PredictDrinks" /> property's name.
        /// </summary>
        public const string PredictDrinksPropertyName = "PredictDrinks";

        private PredictDrink[] _predictDrinks = null;

        /// <summary>
        /// Sets and gets the PredictDrinks property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public PredictDrink[] PredictDrinks
        {
            get
            {
                return _predictDrinks;
            }

            set
            {
                if (_predictDrinks == value)
                {
                    return;
                }

                RaisePropertyChanging(PredictDrinksPropertyName);
                _predictDrinks = value;
                RaisePropertyChanged(PredictDrinksPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsDirty" /> property's name.
        /// </summary>
        public const string IsDirtyPropertyName = "IsDirty";

        private bool _isDirty = false;

        /// <summary>
        /// Sets and gets the IsDirty property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }

            set
            {
                if (_isDirty == value)
                {
                    return;
                }

                RaisePropertyChanging(IsDirtyPropertyName);
                _isDirty = value;
                RaisePropertyChanged(IsDirtyPropertyName);
            }
        }

        public RelayCommand<int> AddAdditionCommand { get; set; }
        public RelayCommand<int> RemoveAdditionCommand { get; set; }
        public RelayCommand RecalculateCommand { get; set; }
        public RelayCommand PersistAdditionsCommand { get; set; }

        public PredictionViewModel()
        {
            if (!IsInDesignMode)
            {
                InitCommands();
            }           
        }

        private void InitCommands()
        {
            AddAdditionCommand = new RelayCommand<int>(AddAddition, CanChangeAddition);
            RemoveAdditionCommand = new RelayCommand<int>(RemoveAddition, CanChangeAddition);
            RecalculateCommand = new RelayCommand(Recalculate);
            PersistAdditionsCommand = new RelayCommand(PersistAdditions, () => { return CanChangeAddition(0) && IsDirty; });
        }

        private void PersistAdditions()
        {
            var locator = GetLocator();
            Interval current = locator.Interval.Intervals.FirstOrDefault(x => x.Id == locator.Settings.CurrentInterval.Id);
            foreach (Drink item in current.Drinks)
            {
                //TODO: solve
                //item.NextPriceAddition = PredictDrinks.FirstOrDefault(x => x.DrinkId == item.Id).Addition;
            }

            IsDirty = false;
        }

        private void Recalculate()
        {
            IsDirty = false;
            if (!CanChangeAddition(0)) return;

            var locator = GetLocator();


            var availableDrinks = locator.Settings.CurrentInterval.Drinks.Where(x => x.Available).ToArray();
            int length = availableDrinks.Length;
            PredictDrinks = new PredictDrink[length];
            for (int i = 0; i < length; i++)
            {
                PredictDrinks[i] = new PredictDrink()
                {
                    CurrentPrice = availableDrinks[i].CurrentPrice,
                    DrinkId = availableDrinks[i].Id,
                    Name = availableDrinks[i].Name,
                    Addition = (sbyte)0
                };
            }

            //these contain the prices that would be set if we didn't interfere with them.
            Interval nextInterval = SettingsViewModel.CalculatePriceUpdates(
                locator.Orders.AllOrderItems, locator.Interval.Intervals, locator.Settings.CurrentInterval.Id, true, this);

            for (int i = 0; i < length; i++)
            {
                Drink drink = nextInterval.Drinks.FirstOrDefault(x => x.Id == PredictDrinks[i].DrinkId);
                if(drink != null){
                    PredictDrinks[i].NextPrice = drink.CurrentPrice;         
                }
            }

        }

        private bool CanChangeAddition(int id)
        {
            if (GetLocator().Settings.CurrentInterval == null) return false;

            //TODO, find some more reasons why this shouldn't be allowed

            return true;
        }

        private void RemoveAddition(int id)
        {
            PredictDrink dr = PredictDrinks.FirstOrDefault(x => x.DrinkId == id);
            int index = Array.IndexOf(PredictDrinks, dr);
            if (dr != null)
            {
                PredictDrinks[index].Addition -= 1;
            }
            IsDirty = true;
            RaisePropertyChanged(PredictDrinksPropertyName);
        }

        private void AddAddition(int id)
        {
            PredictDrink dr = PredictDrinks.FirstOrDefault(x => x.DrinkId == id);
            int index = Array.IndexOf(PredictDrinks, dr);
            if (dr != null)
            {
                PredictDrinks[index].Addition += 1;
            }
            IsDirty = true;
            RaisePropertyChanged(PredictDrinksPropertyName);
        }




    }
}
