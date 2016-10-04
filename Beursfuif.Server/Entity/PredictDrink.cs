using Beursfuif.Server.Entity;

namespace Beursfuif.BL
{
    public class PredictDrink : BFObservableObject
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

        private int _currentPrice = 0;

        /// <summary>
        /// Sets and gets the CurrentPrice property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int CurrentPrice
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
        //public int CurrentPrice { get; set; }

        /// <summary>
        /// The <see cref="NextPrice" /> property's name.
        /// </summary>
        public const string NextPricePropertyName = "NextPrice";

        private int _nextPrice = 0;

        /// <summary>
        /// Sets and gets the NextPrice property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int NextPrice
        {
            get
            {
                if (OverrideFactor == 0.0)
                {
                    return _nextPrice;
                }
                int nextPrice = (int)(CurrentPrice * OverrideFactor);
                if (nextPrice > Maximum) return Maximum;
                if (nextPrice < Minimum) return Minimum;
                return nextPrice;
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
        //public int NextPrice { get; set; }

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
        /// The <see cref="Factor" /> property's name.
        /// </summary>
        public const string FactorPropertyName = "Factor";

        private string _factor = string.Empty;

        /// <summary>
        /// Sets and gets the Factor property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Factor
        {
            get
            {
                if (OverrideFactor == 0.0)
                {
                    return _factor;
                }
                string factor = "override (" + OverrideFactor + ")";
                return factor;
            }

            set
            {
                if (_factor == value)
                {
                    return;
                }

                RaisePropertyChanging(FactorPropertyName);
                _factor = value;
                RaisePropertyChanged(FactorPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="OverrideFactor" /> property's name.
        /// </summary>
        public const string OverrideFactorPropertyName = "OverrideFactor";

        private double _overrideFactor = 0.0;

        /// <summary>
        /// Sets and gets the OverrideFactor property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double OverrideFactor
        {
            get
            {
                return _overrideFactor;
            }

            set
            {
                if (_overrideFactor == value)
                {
                    return;
                }

                RaisePropertyChanging(OverrideFactorPropertyName);
                _overrideFactor = value;
                RaisePropertyChanged(OverrideFactorPropertyName);
                RaisePropertyChanged(NextPricePropertyName);
                RaisePropertyChanged(FactorPropertyName);
            }
        }

        public int Maximum { get; set; }
        public int Minimum { get; set; }
    }
}
