using Beursfuif.BL;
using Beursfuif.Server.Services;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.ViewModel
{
    public class PredictionViewModel : BeursfuifViewModelBase
    {
        #region Fields and properties
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

        public RelayCommand RecalculateCommand { get; set; }
        public RelayCommand PersistOverrideFactorCommand { get; set; }
        #endregion

        public PredictionViewModel(IBeursfuifData data):base(data)
        {
            if (!IsInDesignMode)
            {
                PointInCode("PredictionViewModel: Ctor");

                InitCommands();
            }
        }

        private void InitCommands()
        {
            PointInCode("PredictionViewModel: InitCommands");

            RecalculateCommand = new RelayCommand(Recalculate);
            PersistOverrideFactorCommand = new RelayCommand(PersistAdditions, () => { return IsDirty; });
        }

        private void PersistAdditions()
        {
            PointInCode("PredictionViewModel: PersistAdditions");

            Interval current = _beursfuifData.Intervals.FirstOrDefault(x => x.Id ==  _beursfuifData.CurrentInterval.Id);
            var queryChanged = PredictDrinks.Where(x => x.OverrideFactor != 0.0);
            foreach (PredictDrink pd in queryChanged)
            {
                Drink drink = current.Drinks.FirstOrDefault(x => x.Id == pd.DrinkId);
                if (drink != null)
                {
                    drink.OverrideFactor = pd.OverrideFactor;
                }
            }

            IsDirty = false;
        }

        private void Recalculate()
        {
            PointInCode("PredictionViewModel: Recalculate");

            IsDirty = false;

            Interval currentInterval = _beursfuifData.CurrentInterval;
            if (currentInterval.Id == _beursfuifData.Intervals.Last().Id)
            {
                //The last interval is in progress, you can't recalcutate right here
                return;
            }

            Interval nextInterval = PriceCalculation.CalculatePriceUpdates(_beursfuifData.AllOrderItems,  _beursfuifData.Intervals, currentInterval.Id, true, this);

            int drinkLength = nextInterval.Drinks.Length;
            PredictDrinks = new PredictDrink[drinkLength];
            for (int i = 0; i < drinkLength; i++)
            {
                PredictDrinks[i] = new PredictDrink()
                {
                    CurrentPrice = currentInterval.Drinks[i].CurrentPrice,
                    DrinkId = currentInterval.Drinks[i].Id,
                    Factor = nextInterval.Drinks[i].GetProcentAndFactorType,
                    Maximum = currentInterval.Drinks[i].MaximumPrice,
                    Minimum = currentInterval.Drinks[i].MiniumPrice,
                    Name = currentInterval.Drinks[i].Name,
                    NextPrice = nextInterval.Drinks[i].CurrentPrice,
                    OverrideFactor = 0.0
                };
            }
            RaisePropertyChanged(PredictDrinksPropertyName);

        }

    }
}
