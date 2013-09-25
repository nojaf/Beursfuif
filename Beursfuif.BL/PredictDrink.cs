using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    public class PredictDrink:Beursfuif.BL.BFObservableObject
    {
        /// <summary>
        /// The <see cref="DrinkId" /> property's name.
        /// </summary>
        public const string DrinkIdPropertyName = "DrinkId";

        private int _drinkId = 0;

        /// <summary>
        /// Sets and gets the DrinkId property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int DrinkId
        {
            get
            {
                return _drinkId;
            }

            set
            {
                if (_drinkId == value)
                {
                    return;
                }

                RaisePropertyChanging(DrinkIdPropertyName);
                _drinkId = value;
                RaisePropertyChanged(DrinkIdPropertyName);
            }
        }
        //public int DrinkId { get; set; }
        /// <summary>
        /// The <see cref="CurrentPrice" /> property's name.
        /// </summary>
        public const string CurrentPricePropertyName = "CurrentPrice";

        private byte _currentPrice = 0;

        /// <summary>
        /// Sets and gets the CurrentPrice property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public byte CurrentPrice
        {
            get
            {
                return _currentPrice;
            }

            set
            {
                if (_currentPrice == value)
                {
                    return;
                }

                RaisePropertyChanging(CurrentPricePropertyName);
                _currentPrice = value;
                RaisePropertyChanged(CurrentPricePropertyName);
            }
        }
        //public byte CurrentPrice { get; set; }

        /// <summary>
        /// The <see cref="NextPrice" /> property's name.
        /// </summary>
        public const string NextPricePropertyName = "NextPrice";

        private byte _nextPrice = 0;

        /// <summary>
        /// Sets and gets the NextPrice property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public byte NextPrice
        {
            get
            {
                return _nextPrice;
            }

            set
            {
                if (_nextPrice == value)
                {
                    return;
                }

                RaisePropertyChanging(NextPricePropertyName);
                _nextPrice = value;
                RaisePropertyChanged(NextPricePropertyName);
            }
        }
        //public byte NextPrice { get; set; }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = null;

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
       // public string Name { get; set; }

        /// <summary>
        /// The <see cref="Addition" /> property's name.
        /// </summary>
        public const string AdditionPropertyName = "Addition";

        private sbyte _addition = 0;

        /// <summary>
        /// Sets and gets the Addition property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public sbyte Addition
        {
            get
            {
                return _addition;
            }

            set
            {
                if (_addition == value)
                {
                    return;
                }

                RaisePropertyChanging(AdditionPropertyName);
                _addition = value;
                RaisePropertyChanged(AdditionPropertyName);
            }
        }
        //public sbyte Addition { get; set; }
    }
}
