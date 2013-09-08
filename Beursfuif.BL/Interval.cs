using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    [Serializable]
    public class Interval:BFObservableObject
    {
        #region properties
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
        /// The <see cref="StartTime" /> property's name.
        /// </summary>
        public const string StartTimePropertyName = "StartTime";

        private DateTime _startTime = DateTime.MinValue;

        /// <summary>
        /// Sets and gets the StartTime property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }

            set
            {
                if (_startTime == value)
                {
                    return;
                }

                RaisePropertyChanging(StartTimePropertyName);
                _startTime = value;
                RaisePropertyChanged(StartTimePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="EndTime" /> property's name.
        /// </summary>
        public const string EndTimePropertyName = "EndTime";

        private DateTime _endTime = DateTime.MaxValue;

        /// <summary>
        /// Sets and gets the EndTime property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }

            set
            {
                if (_endTime == value)
                {
                    return;
                }

                RaisePropertyChanging(EndTimePropertyName);
                _endTime = value;
                RaisePropertyChanged(EndTimePropertyName);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return EndTime.Subtract(StartTime);
            }
        }

        /// <summary>
        /// The <see cref="Drinks" /> property's name.
        /// </summary>
        public const string DrinksPropertyName = "Drinks";

        private Drink[] _drinks = null;

        /// <summary>
        /// Sets and gets the Drinks property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Drink[] Drinks
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
        /// The <see cref="AuthenticationString" /> property's name.
        /// </summary>
        public const string AuthenticationStringPropertyName = "AuthenticationString";

        private string _authenticationString = string.Empty;

        /// <summary>
        /// Sets and gets the AuthenticationString property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string AuthenticationString
        {
            get
            {
                return _authenticationString;
            }

            set
            {
                if (_authenticationString == value)
                {
                    return;
                }

                RaisePropertyChanging(AuthenticationStringPropertyName);
                _authenticationString = value;
                RaisePropertyChanged(AuthenticationStringPropertyName);
            }
        }
        #endregion

        public Interval()
        {

        }

        public override string ToString()
        {
            return string.Format("[Interval]:[id = {0},{1}-{2},code = {3}]", Id, StartTime, EndTime, AuthenticationString);
        }

        public override bool Equals(object obj)
        {
            if (obj is Interval)
            {
                Interval other = obj as Interval;
                if (other.Id == this.Id) return true;
            }
            return false;
        }
    }
}
