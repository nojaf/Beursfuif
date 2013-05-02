﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    public class Client : ObservableObject
    {
        #region properties
        /// <summary>
        /// The <see cref="Ip" /> property's name.
        /// </summary>
        public const string IpPropertyName = "Ip";

        private string _ip = string.Empty;

        /// <summary>
        /// Sets and gets the Ip property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Ip
        {
            get
            {
                return _ip;
            }

            set
            {
                if (_ip == value)
                {
                    return;
                }

                RaisePropertyChanging(IpPropertyName);
                _ip = value;
                RaisePropertyChanged(IpPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = string.Empty;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                RaisePropertyChanging(NamePropertyName);
                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Id" /> property's name.
        /// </summary>
        public const string IdPropertyName = "Id";

        private int _id = 0;

        /// <summary>
        /// Sets and gets the Id property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="LastActivity" /> property's name.
        /// </summary>
        public const string LastActivityPropertyName = "LastActivity";

        private DateTime _lastActivity = DateTime.MinValue;

        /// <summary>
        /// Sets and gets the LastActivity property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime LastActivity
        {
            get
            {
                return _lastActivity;
            }

            set
            {
                if (_lastActivity == value)
                {
                    return;
                }

                RaisePropertyChanging(LastActivityPropertyName);
                _lastActivity = value;
                RaisePropertyChanged(LastActivityPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="OrderCount" /> property's name.
        /// </summary>
        public const string OrderCountPropertyName = "OrderCount";

        private int _orderCount = 0;

        /// <summary>
        /// Sets and gets the OrderCount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int OrderCount
        {
            get
            {
                return _orderCount;
            }

            set
            {
                if (_orderCount == value)
                {
                    return;
                }

                RaisePropertyChanging(OrderCountPropertyName);
                _orderCount = value;
                RaisePropertyChanged(OrderCountPropertyName);
            }
        }
        #endregion

        public Client()
        {

        }

        public override string ToString()
        {
            return string.Format("[Client]:[Id = {0}, Ip = {1}, Name = {2}, Latest update = {3}", Id, Ip, Name, LastActivity);
        }

        public override bool Equals(object obj)
        {
            if (obj is Client)
            {
                Client other = obj as Client;
                if (other.Id == this.Id && other.Ip == this.Ip)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
