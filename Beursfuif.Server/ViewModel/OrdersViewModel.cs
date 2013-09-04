using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beursfuif.BL.Extensions;
using System.Collections.ObjectModel;

namespace Beursfuif.Server.ViewModel
{
    public class OrdersViewModel : BeursfuifViewModelBase
    {
        private BeursfuifServer _server;

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

        public OrdersViewModel(BeursfuifServer server)
        {
            _server = server;
            InitServer();
        }




        protected override void ChangePartyBusy(Messages.BeursfuifBusyMessage obj)
        {
            base.ChangePartyBusy(obj);
            if (BeursfuifBusy && AllOrders == null && ShowOrderList == null)
            {
                InitData();
            }
        }

        private void InitData()
        {
            AllOrders = new ObservableCollection<ShowOrder>();
            ShowOrderList = new ObservableCollection<ShowOrder>();

            Interval[] intervals = base.GetLocator().Interval.Intervals;
            int length = intervals.Length + 1;
            ReducedIntervals = new ReducedInterval[length];
            ReducedIntervals[0] = new ReducedInterval("Alle Intervalen");
            for (int i = 1; i < length; i++)
            {
                ReducedIntervals[i] = new ReducedInterval(intervals[i-1]);
            }

            SelectedInterval = ReducedIntervals[0];
        }


        private void UpdateShowOrderList()
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ShowOrderList != null && SelectedInterval != null)
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
                ShowOrder newOrder = new ShowOrder()
                {
                    ClientName = locator.Clients.Clients.FirstOrDefault(x => x.Id == e.ClientId).Name,
                    IntervalId = currentInterval.Id,
                    OrderContent = e.Order.ToContentString(locator.Drink.Drinks),
                    Time = DateTime.Now,
                    TotalPrice = e.Order.TotalPrice(currentInterval),
                    Orders = e.Order
                };

                AllOrders.Add(newOrder);
                if (SelectedInterval.Id == currentInterval.Id || SelectedInterval.Id == int.MaxValue)
                {
                    ShowOrderList.Add(newOrder);
                }       
            }));
        }
    }
}
