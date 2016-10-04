using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using Beursfuif.BL.Extensions;
using System.Collections.ObjectModel;
using Beursfuif.Server.Messages;
using System.Threading;
using Beursfuif.Server.Services;
using Beursfuif.BL.Event;
using Beursfuif.BL.Entity;
using Beursfuif.Server.Entity;

namespace Beursfuif.Server.ViewModel
{
    public class OrdersViewModel : BeursfuifViewModelBase
    {
        #region Fields and Properties
        private const string AllIntervals = "Alle intervallen";
        private IBeursfuifServer _server;

        public List<ClientDrinkOrder> AllOrderItems
        {
            get { return _beursfuifData.AllOrderItems; }
        }

        /// <summary>
        /// The <see cref="AllOrders" /> property's name.
        /// </summary>
        public const string AllOrdersPropertyName = "AllOrders";

        /// <summary>
        /// Sets and gets the AllOrders property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ShowOrder> AllOrders
        {
            get
            {
                return _beursfuifData.AllOrders;
            }

            set
            {
                if (_beursfuifData.AllOrders == value)
                {
                    return;
                }

                RaisePropertyChanging(AllOrdersPropertyName);
                _beursfuifData.AllOrders = value;
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
                UpdateDrinkStats();
                RaisePropertyChanged(SelectedIntervalPropertyName);
            }
        }

        private void UpdateDrinkStats()
        {
            DrinkStats = null;
            DrinkStats = CreateDrinkStatistics();
        }


        /// <summary>
        /// The <see cref="DrinkStats" /> property's name.
        /// </summary>
        public const string DrinkStatsPropertyName = "DrinkStats";

        private IEnumerable<DrinkStatistic> _drinkStats;

        /// <summary>
        /// Sets and gets the DrinkStats property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<DrinkStatistic> DrinkStats
        {
            get
            {
                return _drinkStats;
            }
            set
            {
                if (_drinkStats == value)
                {
                    return;
                }

                RaisePropertyChanging(DrinkStatsPropertyName);
                _drinkStats = value;
                RaisePropertyChanged(DrinkStatsPropertyName);
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

        public OrdersViewModel(IBeursfuifServer server, IBeursfuifData beursfuifData)
            : base(beursfuifData)
        {
            if (!IsInDesignMode)
            {
                PointInCode("OrdersViewModel: Ctor");

                _server = server;
                InitServer();
                InitMessages();

                if (_beursfuifData.BeursfuifEverStarted)
                {
                    InitData();
                }

                _beursfuifData.DataReset += BeursfuifData_DataReset;
                _beursfuifData.BeursfuifDataImported += BeursfuifData_BeursfuifDataImported;
            }
        }

        void BeursfuifData_BeursfuifDataImported(object sender, EventArgs e)
        {
            RaisePropertyEvents();
        }

        void BeursfuifData_DataReset(object sender, bool e)
        {
            RaisePropertyEvents();
        }

        private void RaisePropertyEvents()
        {
            RaisePropertyChanged(AllOrdersPropertyName);
            RaisePropertyChanged(ShowOrderListPropertyName);
            RaisePropertyChanged(ReducedIntervalsPropertyName);
            RaisePropertyChanged(SelectedIntervalPropertyName);
            RaisePropertyChanged(ReducedDrinksPropertyName);
            RaisePropertyChanged(DrinkStatsPropertyName);
        }

        #region Messages
        private void InitMessages()
        {
            PointInCode("OrdersViewModel: InitMessages");

            MessengerInstance.Register<AutoSaveAllOrdersMessage>(this, AutoSaveAllOrdersHandler);
        }

        private void AutoSaveAllOrdersHandler(AutoSaveAllOrdersMessage obj)
        {
            PointInCode("OrdersViewModel: AutoSaveAllOrdersHandler");

            ThreadPool.QueueUserWorkItem(new WaitCallback((object target) =>
            {
                _beursfuifData.SaveAllOrders();
            }));
            SendToastMessage("Autosaved", "Alle bestellingen werden bewaard.");
            SendLogMessage("Autosave all orders", LogType.ORDER_VM);
        }

        #endregion

        #region Data
        private void InitData()
        {
            PointInCode("OrdersViewModel: InitData");

            if (AllOrders == null)
            {
                SendLogMessage("No orders where found in data folder", LogType.ORDER_VM);
                AllOrders = new ObservableCollection<ShowOrder>();
            }

            foreach (ShowOrder sh in AllOrders)
            {
                _beursfuifData.AllOrderItems.AddRange(sh.Orders);
            }

            ShowOrderList = new ObservableCollection<ShowOrder>();

            //ReducedIntervals to populate the combobox
            Interval[] intervals = _beursfuifData.Intervals;
            int length = intervals.Length + 1;
            ReducedIntervals = new ReducedInterval[length];
            ReducedIntervals[0] = new ReducedInterval(AllIntervals);
            for (int i = 1; i < length; i++)
            {
                ReducedIntervals[i] = new ReducedInterval(intervals[i - 1]);
            }
            SelectedInterval = ReducedIntervals[0];

            //Reduced drinks to populate the graph control
            var drinks = _beursfuifData.Drinks;
            int drinksLength = drinks.Count;
            ReducedDrinks = new ReducedDrink[drinksLength];
            for (int j = 0; j < drinksLength; j++)
            {
                ReducedDrinks[j] = new ReducedDrink() { Id = drinks[j].Id, Name = drinks[j].Name };
            }

            UpdateDrinkStats();

        }

        private void UpdateShowOrderList()
        {
            PointInCode("OrdersViewModel: UpdateShowOrderList");

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

        private IEnumerable<DrinkStatistic> CreateDrinkStatistics()
        {
            if (ReducedDrinks == null || SelectedInterval == null || _beursfuifData.AllOrderItems == null) return null;

            Interval interval = _beursfuifData.Intervals.FirstOrDefault(x => x.Id == SelectedInterval.Id);


            DrinkStatistic[] drinkStatistics = new DrinkStatistic[ReducedDrinks.Length];
            for (int i = 0; i < ReducedDrinks.Length; i++)
            {
                int drinkId = ReducedDrinks[i].Id;
                DrinkStatistic drinkStatistic = new DrinkStatistic()
                {
                    DrinkId = drinkId,
                    DrinkName = ReducedDrinks[i].Name
                };


                drinkStatistic.OrderCount = (interval == null ?
                    _beursfuifData.AllOrderItems.Where(x => x.DrinkId == drinkId).Sum(x => x.Count) :
                    _beursfuifData.AllOrderItems.Where(x => x.DrinkId == drinkId
                        && x.IntervalId == interval.Id).Sum(x => x.Count));

                if (interval != null)
                {
                    drinkStatistic.Price = interval.Drinks.FirstOrDefault(x => x.Id == drinkId).CurrentPrice;
                }

                drinkStatistics[i] = drinkStatistic;
            }

            return drinkStatistics.OrderByDescending(x => x.OrderCount);
        }

        #endregion

        #region Server
        private void InitServer()
        {
            PointInCode("OrdersViewModel: InitServer");

            _server.NewOrderEvent += Server_NewOrderEvent;
        }

        void Server_NewOrderEvent(object sender, NewOrderEventArgs e)
        {
            PointInCode("OrdersViewModel: Server_NewOrderEvent");


            if (_beursfuifData.AuthenticationString() == e.AuthenticationCode)
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddOrder(e);
                }));
                return;
            }
            //else
            //client doesn't have a valid code

            SendLogMessage($"Invalid authcode from client {_beursfuifData.GetClientName(e.ClientId)}",
                LogType.ORDER_VM | LogType.CLIENT_SERVER_ERROR);
            MessengerInstance.Send<KickClientMessage>(new KickClientMessage()
            {
                ClientId = e.ClientId,
                Reason = KickWasKickedReason.TIME_OUT_ERROR
            });


        }

        private void AddOrder(NewOrderEventArgs e)
        {
            PointInCode("OrdersViewModel: AddOrder");
            Interval currentInterval = _beursfuifData.CurrentInterval;
            ClientDrinkOrder[] items = e.Order;
            foreach (var drinkItem in items)
            {
                drinkItem.IntervalId = currentInterval.Id;
            }

            Client client = _beursfuifData.Clients.FirstOrDefault(x => x.Id == e.ClientId);

            if (client != null)
            {
                ShowOrder newOrder = new ShowOrder()
                {
                    ClientName = client.Name,
                    IntervalId = currentInterval.Id,
                    OrderContent = e.Order.ToContentString(_beursfuifData.Drinks),
                    Time = _beursfuifData.BeursfuifCurrentTime,
                    TotalPrice = e.Order.TotalPrice(currentInterval),
                    Orders = items
                };

                if (AllOrders == null)
                {
                    AllOrders = new ObservableCollection<ShowOrder>();
                }

                if (ReducedIntervals == null)
                {
                    InitData();
                }

                AllOrders.Add(newOrder);
                RaisePropertyChanged(AllOrdersPropertyName);
                if (SelectedInterval == null) SelectedInterval = ReducedIntervals[0];

                if (SelectedInterval.Id == currentInterval.Id || SelectedInterval.Id == int.MaxValue)
                {
                    if (ShowOrderList == null)
                    {
                        ShowOrderList = new ObservableCollection<ShowOrder>();
                    }

                    ShowOrderList.Add(newOrder);
                }

                if (_beursfuifData.AllOrderItems == null)
                {
                    _beursfuifData.AllOrderItems = new List<ClientDrinkOrder>();
                }

                _beursfuifData.AllOrderItems.AddRange(e.Order);

                UpdateDrinkStats();
            }
        }
        #endregion
    }
}
