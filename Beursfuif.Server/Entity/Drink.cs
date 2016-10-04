using System;
using Beursfuif.BL.Extensions;

namespace Beursfuif.Server.Entity
{
    [Serializable]
    public class Drink : BFObservableObject
    {
        #region properties
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

                RaisePropertyChanging(nameof(Id));
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

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

                RaisePropertyChanging(nameof(Name));
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        private string _imageString = string.Empty;

        /// <summary>
        /// Sets and gets the ImageString property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ImageString
        {
            get
            {
                return _imageString;
            }

            set
            {
                if (_imageString == value)
                {
                    return;
                }

                RaisePropertyChanging(nameof(ImageString));
                _imageString = value;
                RaisePropertyChanged(nameof(ImageString));
            }
        }

        private bool _available = true;

        /// <summary>
        /// Sets and gets the Available property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool Available
        {
            get
            {
                return _available;
            }

            set
            {
                if (_available == value)
                {
                    return;
                }

                RaisePropertyChanging(nameof(Available));
                _available = value;
                RaisePropertyChanged(nameof(Available));
            }
        }

        private int _initialPrice = 14;

        /// <summary>
        /// Sets and gets the InitialPrice property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int InitialPrice
        {
            get
            {
                return _initialPrice;
            }

            set
            {
                if (_initialPrice == value)
                {
                    return;
                }

                RaisePropertyChanging(nameof(InitialPrice));
                _initialPrice = value;
                RaisePropertyChanged(nameof(InitialPrice));
            }
        }

        private int _minimumPrice = 10;

        /// <summary>
        /// Sets and gets the MiniumPrice property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int MiniumPrice
        {
            get
            {
                return _minimumPrice;
            }

            set
            {
                if (_minimumPrice == value)
                {
                    return;
                }

                RaisePropertyChanging(nameof(MiniumPrice));
                _minimumPrice = value;
                RaisePropertyChanged(nameof(MiniumPrice));
            }
        }

        private int _maximumPrice = 30;

        /// <summary>
        /// Sets and gets the MaximumPrice property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int MaximumPrice
        {
            get
            {
                return _maximumPrice;
            }

            set
            {
                if (_maximumPrice == value)
                {
                    return;
                }

                RaisePropertyChanging(nameof(MaximumPrice));
                _maximumPrice = value;
                RaisePropertyChanged(nameof(MaximumPrice));
            }
        }

        private int _currentPrice = 15;

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

                RaisePropertyChanging(nameof(CurrentPrice));
                _currentPrice = value;
                RaisePropertyChanged(nameof(CurrentPrice));
            }
        }

        private double _bigRise = 1.12;

        /// <summary>
        /// Sets and gets the BigRise property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double BigRise
        {
            get
            {
                return _bigRise;
            }

            set
            {
                if (_bigRise == value)
                {
                    return;
                }

                RaisePropertyChanging(nameof(BigRise));
                _bigRise = value;
                PriceFactor = PriceFactor.BigRise;
                RaisePropertyChanged(nameof(BigRise));
            }
        }

        /// <summary>
        /// The <see cref="SmallRise" /> property's name.
        /// </summary>
        public const string SmallRisePropertyName = "SmallRise";

        private double _smallRise = 1.08;

        /// <summary>
        /// Sets and gets the SmallRise property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double SmallRise
        {
            get
            {
                return _smallRise;
            }

            set
            {
                if (_smallRise == value)
                {
                    return;
                }

                RaisePropertyChanging(SmallRisePropertyName);
                _smallRise = value;
                PriceFactor = PriceFactor.SmallRise;
                RaisePropertyChanged(SmallRisePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="BigDecrease" /> property's name.
        /// </summary>
        public const string BigDecreasePropertyName = "BigDecrease";

        private double _bigDecrease = 0.88;

        /// <summary>
        /// Sets and gets the BigDecrease property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double BigDecrease
        {
            get
            {
                return _bigDecrease;
            }

            set
            {
                if (_bigDecrease == value)
                {
                    return;
                }

                RaisePropertyChanging(BigDecreasePropertyName);
                _bigDecrease = value;
                PriceFactor = PriceFactor.BigDecrease;
                RaisePropertyChanged(BigDecreasePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SmallDecrease" /> property's name.
        /// </summary>
        public const string SmallDecreasePropertyName = "SmallDecrease";

        private double _smallDecrease = 0.92;

        /// <summary>
        /// Sets and gets the SmallDecrease property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double SmallDecrease
        {
            get
            {
                return _smallDecrease;
            }

            set
            {
                if (_smallDecrease == value)
                {
                    return;
                }

                RaisePropertyChanging(SmallDecreasePropertyName);
                _smallDecrease = value;
                PriceFactor = PriceFactor.SmallDecrease;
                RaisePropertyChanged(SmallDecreasePropertyName);
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
                PriceFactor = PriceFactor.Override;
                RaisePropertyChanged(OverrideFactorPropertyName);
            }
        }

        public PriceFactor PriceFactor { get; set; }

        public string GetProcentAndFactorType 
        { 
            get {
                string value = this.PriceFactor.GetDescription()  + " (";
                switch (PriceFactor)
                {
                    case PriceFactor.BigDecrease:
                        value += BigDecrease;
                        break;
                    case PriceFactor.SmallDecrease:
                        value += SmallDecrease;
                        break;
                    case PriceFactor.BigRise:
                        value += BigRise;
                        break;
                    case PriceFactor.SmallRise:
                        value += SmallRise;
                        break;
                    case PriceFactor.Override:
                        value += OverrideFactor;
                        break;
                }
                value += ")";
                return value;
            } 
        }
        #endregion

        public Drink()
        {

        }

        public override string ToString()
        {
            return $"[Drink]:[{Id},{Name}]";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Drink) obj);
        }

        protected bool Equals(Drink other)
        {
            return _id == other._id && string.Equals(_name, other._name) && _available == other._available && _initialPrice == other._initialPrice && _minimumPrice == other._minimumPrice && _maximumPrice == other._maximumPrice && _currentPrice == other._currentPrice && _bigRise.Equals(other._bigRise) && _smallRise.Equals(other._smallRise) && _bigDecrease.Equals(other._bigDecrease) && _overrideFactor.Equals(other._overrideFactor);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _id;
                hashCode = (hashCode*397) ^ (_name?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ _available.GetHashCode();
                hashCode = (hashCode*397) ^ _initialPrice;
                hashCode = (hashCode*397) ^ _minimumPrice;
                hashCode = (hashCode*397) ^ _maximumPrice;
                hashCode = (hashCode*397) ^ _currentPrice;
                hashCode = (hashCode*397) ^ _bigRise.GetHashCode();
                hashCode = (hashCode*397) ^ _smallRise.GetHashCode();
                hashCode = (hashCode*397) ^ _bigDecrease.GetHashCode();
                hashCode = (hashCode*397) ^ _overrideFactor.GetHashCode();
                return hashCode;
            }
        }

        public Drink Clone()
        {
            Drink clone = this.MemberwiseClone() as Drink;
            return clone;
        }

        public double GetPriceFactorValue()
        {
            switch (this.PriceFactor)
            {
                case PriceFactor.BigDecrease:
                    return this.BigDecrease;
                case PriceFactor.BigRise:
                    return this.BigRise;
                case PriceFactor.Override:
                    return this.OverrideFactor;
                case PriceFactor.SmallDecrease:
                    return this.SmallDecrease;
                case PriceFactor.SmallRise:
                    return this.SmallRise;
            }

            return 1.0;
        }

        /// <summary>
        /// Updates properties that are allowed to be changed when the party is started
        /// </summary>
        /// <param name="changed"></param>
        public void UpdateProperties(Drink changed)
        {
            Name = changed.Name;

            Available = changed.Available;
            BigDecrease = changed.BigDecrease;
            BigRise = changed.BigRise;
            SmallDecrease = changed.SmallDecrease;
            SmallRise = changed.SmallRise;

            MiniumPrice = changed.MiniumPrice;
            MaximumPrice = changed.MaximumPrice;
        }
    }
}
