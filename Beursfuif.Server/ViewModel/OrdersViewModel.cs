using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beursfuif.BL.Extensions;
using System.Collections.ObjectModel;

namespace Beursfuif.Server.ViewModel
{
    public class OrdersViewModel:BeursfuifViewModelBase
    {
        private BeursfuifServer _server;

        /// <summary>
        /// The <see cref="AllClientDrinkOrders" /> property's name.
        /// </summary>
        public const string AllClientDrinkOrdersPropertyName = "AllClientDrinkOrders";

        private ObservableCollection<ClientDrinkOrder> _allClientDrinkOrders = new ObservableCollection<ClientDrinkOrder>();

        /// <summary>
        /// Sets and gets the AllClientDrinkOrders property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ClientDrinkOrder> AllClientDrinkOrders
        {
            get
            {
                return _allClientDrinkOrders;
            }

            set
            {
                if (_allClientDrinkOrders == value)
                {
                    return;
                }

                RaisePropertyChanging(AllClientDrinkOrdersPropertyName);
                _allClientDrinkOrders = value;
                RaisePropertyChanged(AllClientDrinkOrdersPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ShowOrderList" /> property's name.
        /// This ObservableCollection is only to display the data
        /// </summary>
        public const string ShowOrderListPropertyName = "ShowOrderList";

        private ObservableCollection<ShowOrder> _showOrderList = new ObservableCollection<ShowOrder>();

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

        public OrdersViewModel(BeursfuifServer server)
        {
            _server = server;

            InitServer();
        }

        private void InitServer()
        {
            _server.NewOrderEvent += Server_NewOrderEvent;
        }

        void Server_NewOrderEvent(object sender, BL.Event.NewOrderEventArgs e)
        {
            var locator = base.GetLocator();
            var currentInterval  = locator.Settings.CurrentInterval;
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ShowOrderList.Add(new ShowOrder()
                {
                    ClientName = locator.Clients.Clients.FirstOrDefault(x => x.Id == e.ClientId).Name,
                    IntervalId = currentInterval.Id,
                    OrderContent = e.Order.ToContentString(locator.Drink.Drinks),
                    Time = DateTime.Now,
                    TotalPrice = e.Order.TotalPrice(currentInterval)
                });

                foreach (ClientDrinkOrder clientOrderDrink in e.Order)
                {
                    clientOrderDrink.IntervalId = currentInterval.Id;
                    AllClientDrinkOrders.Add(clientOrderDrink);
                }
            }));
        }
    }
}
