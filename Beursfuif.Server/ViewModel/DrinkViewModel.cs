﻿using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using Beursfuif.Server.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

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
                RaisePropertyChanged(NewEditDrinkPropertyName);
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
            }

            InitMessages();
            InitCommands();

        }

        private void InitCommands()
        {
            //It shouldn't be posible to remove a drink when the party is busy
            RemoveDrink = new RelayCommand<int>(DeleteDrink, (int id) => { return (!BeursfuifBusy && Drinks.Any(x => x.Id == id)); });
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
    }
}
