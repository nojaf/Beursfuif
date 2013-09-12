using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beursfuif.BL.Extensions;
using System.Collections.ObjectModel;
using Beursfuif.Server.Messages;
using System.Threading;
using Beursfuif.Server.DataAccess;
using System.Threading.Tasks;
using System.IO;

namespace Beursfuif.Server.ViewModel
{
    public class OrdersViewModel : BeursfuifViewModelBase
    {
        #region Fields and Properties
        private BeursfuifServer _server;
        private IOManager _ioManager;

        private List<ClientDrinkOrder> _allOrderItems = new List<ClientDrinkOrder>();
        public List<ClientDrinkOrder> AllOrderItems
        {
            get { return _allOrderItems; }
        }

        /// <summary>
        /// The <see cref="AllOrders" /> property's name.
        /// </summary>
        public const string AllOrdersPropertyName = "AllOrders";

        private ObservableCollection<ShowOrder> _allOrders = null;

        /// <summary>
        /// Sets and gets the AllOrders property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ShowOrder> AllOrders
        {
            get
            {
                return _allOrders;
            }

            set
            {
                if (_allOrders == value)
                {
                    return;
                }

                RaisePropertyChanging(AllOrdersPropertyName);
                _allOrders = value;
                RaisePropertyChanged(AllOrdersPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ShowOrderList" /> property's name.
        /// This ObservableCollection is only to display the data
        /// </summary>
        public const string ShowOrderListPropertyName = "ShowOrderList";

        private ObservableCollection<ShowOrder> _showOrderList = null;

        /// <summary>
        /// Sets and gets the ShowOrderList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ShowOrder> ShowOrderList
        {
            get
            {
                return _showOrderList;
            }

            set
            {
                if (_showOrderList == value)
                {
                    return;
                }

                RaisePropertyChanging(ShowOrderListPropertyName);
                _showOrderList = value;
                RaisePropertyChanged(ShowOrderListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ReducedIntervals" /> property's name.
        /// </summary>
        public const string ReducedIntervalsPropertyName = "ReducedIntervals";

        private ReducedInterval[] _reducedIntervals = null;

        /// <summary>
        /// Sets and gets the ReducedIntervals property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ReducedInterval[] ReducedIntervals
        {
            get
            {
                return _reducedIntervals;
            }

            set
            {
                if (_reducedIntervals == value)
                {
                    return;
                }

                RaisePropertyChanging(ReducedIntervalsPropertyName);
                _reducedIntervals = value;
                RaisePropertyChanged(ReducedIntervalsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedInterval" /> property's name.
        /// </summary>
        public const string SelectedIntervalPropertyName = "SelectedInterval";

        private ReducedInterval _selectedInterval = null;

        /// <summary>
        /// Sets and gets the SelectedInterval property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ReducedInterval SelectedInterval
        {
            get
            {
                return _selectedInterval;
            }

            set
            {
                if (_selectedInterval == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedIntervalPropertyName);
                _selectedInterval = value;
                UpdateShowOrderList();
                RaisePropertyChanged(SelectedIntervalPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="DrinkCount" /> property's name.
        /// </summary>
        public const string DrinkCountPropertyName = "DrinkCount";

        private Tuple<string,int>[] _drinkCount = null;

        /// <summary>
        /// Sets and gets the DrinkCount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Tuple<string,int>[] DrinkCount
        {
            get
            {
                return _drinkCount;
            }

            set
            {
                if (_drinkCount == value)
                {
                    return;
                }

                RaisePropertyChanging(DrinkCountPropertyName);
                _drinkCount = value;
                RaisePropertyChanged(DrinkCountPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ReducedDrinks" /> property's name.
        /// </summary>
        public const string ReducedDrinksPropertyName = "ReducedDrinks";

        private ReducedDrink[] _reducedDrinks = null;

        /// <summary>
        /// Sets and gets the ReducedDrinks property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ReducedDrink[] ReducedDrinks
        {
            get
            {
                return _reducedDrinks;
            }

            set
            {
                if (_reducedDrinks == value)
                {
                    return;
                }

                RaisePropertyChanging(ReducedDrinksPropertyName);
                _reducedDrinks = value;
                RaisePropertyChanged(ReducedDrinksPropertyName);
            }
        }
        #endregion

        public OrdersViewModel(BeursfuifServer server, IOManager ioManager)
        {
            _server = server;
            _ioManager = ioManager;
            InitServer();
            InitMessages();

            if (File.Exists(PathManager.BUSY_AND_TIME_PATH))
            {
                InitData();
            }
        }

        #region Messages
        private void InitMessages()
        {
            MessengerInstance.Register<AutoSaveAllOrdersMessage>(this, AutoSaveAllOrdersHandler);
        }

        private void AutoSaveAllOrdersHandler(AutoSaveAllOrdersMessage obj)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((object target) =>
            {
                _ioManager.SaveObservableCollectionToBinary<ShowOrder>(PathManager.AUTO_SAVE_ALL_ORDERS, AllOrders);
            }));
            MessengerInstance.Send<ToastMessage>(new ToastMessage("Autosaved", "Alle bestellingen werden bewaard."));

        }

        #endregion

        #region Data
        private void InitData()
        {
            AllOrders = _ioManager.LoadObservableCollectionFromBinary<ShowOrder>(PathManager.AUTO_SAVE_ALL_ORDERS);
            if (AllOrders == null) AllOrders = new ObservableCollection<ShowOrder>();

            foreach (ShowOrder sh in AllOrders)
            {
                _allOrderItems.AddRange(sh.Orders);
            }

            ShowOrderList = new ObservableCollection<ShowOrder>();

            //ReducedIntervals to populate the combobox
            Interval[] intervals = base.GetLocator().Interval.Intervals;
            int length = intervals.Length + 1;
            ReducedIntervals = new ReducedInterval[length];
            ReducedIntervals[0] = new ReducedInterval("Alle Intervalen");
            for (int i = 1; i < length; i++)
            {
                ReducedIntervals[i] = new ReducedInterval(intervals[i - 1]);
            }
            SelectedInterval = ReducedIntervals[0];

            //Reduced drinks to populate the graph control
            var drinks = base.GetLocator().Drink.Drinks;
            int drinksLength = drinks.Count;
            ReducedDrinks = new ReducedDrink[drinksLength];
            for (int j = 0; j < drinksLength; j++)
            {
                ReducedDrinks[j] = new ReducedDrink() { Id = drinks[j].Id, Name = drinks[j].Name };
            }
            
        }

        private void UpdateShowOrderList()
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ShowOrderList != null && SelectedInterval != null && AllOrders.Count > 0)
                {
                    ShowOrderList.Clear();
                    var query = from allOrder in AllOrders
                                where allOrder.IntervalId == SelectedInterval.Id || SelectedInterval.Id == int.MaxValue
                                select allOrder;
                    foreach (var item in query)
                    {
                        ShowOrderList.Add(item);
                    }

                }
            }));
        }

        protected override void ChangePartyBusy(BeursfuifBusyMessage obj)
        {
            base.ChangePartyBusy(obj);
            if (BeursfuifBusy && AllOrders == null)
            {
                InitData();
            }
        }
        #endregion

        #region Server
        private void InitServer()
        {
            _server.NewOrderEvent += Server_NewOrderEvent;
        }

        void Server_NewOrderEvent(object sender, BL.Event.NewOrderEventArgs e)
        {
            var locator = base.GetLocator();
            var currentInterval = locator.Settings.CurrentInterval;
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                //TODO: Check with auth code
                ClientDrinkOrder[] items = e.Order;
                foreach (var drinkItem in items)
                {
                    drinkItem.IntervalId = currentInterval.Id;
                }

                ShowOrder newOrder = new ShowOrder()
                {
                    ClientName = locator.Clients.Clients.FirstOrDefault(x => x.Id == e.ClientId).Name,
                    IntervalId = currentInterval.Id,
                    OrderContent = e.Order.ToContentString(locator.Drink.Drinks),
                    Time = locator.Settings.BeursfuifCurrentTime,
                    TotalPrice = e.Order.TotalPrice(currentInterval),
                    Orders = items
                };

                AllOrders.Add(newOrder);
                RaisePropertyChanged(AllOrdersPropertyName);
                if (SelectedInterval == null) SelectedInterval = ReducedIntervals[0];

                if (SelectedInterval.Id == currentInterval.Id || SelectedInterval.Id == int.MaxValue)
                {
                    ShowOrderList.Add(newOrder);
                }

                _allOrderItems.AddRange(e.Order);
            }));
        }
        #endregion
    }
}
