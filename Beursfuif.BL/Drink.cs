using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Beursfuif.BL.Extensions;

namespace Beursfuif.BL
{
    [Serializable]
    public class Drink : BFObservableObject
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
        /// The <see cref="ImageString" /> property's name.
        /// </summary>
        public const string ImageStringPropertyName = "ImageString";

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

                RaisePropertyChanging(ImageStringPropertyName);
                _imageString = value;
                RaisePropertyChanged(ImageStringPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Available" /> property's name.
        /// </summary>
        public const string AvailablePropertyName = "Available";

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

                RaisePropertyChanging(AvailablePropertyName);
                _available = value;
                RaisePropertyChanged(AvailablePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="InitialPrice" /> property's name.
        /// </summary>
        public const string InitialPricePropertyName = "InitialPrice";

        private byte _initialPrice = 14;

        /// <summary>
        /// Sets and gets the InitialPrice property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public byte InitialPrice
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

                RaisePropertyChanging(InitialPricePropertyName);
                _initialPrice = value;
                RaisePropertyChanged(InitialPricePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="MiniumPrice" /> property's name.
        /// </summary>
        public const string MiniumPricePropertyName = "MiniumPrice";

        private byte _minimumPrice = 10;

        /// <summary>
        /// Sets and gets the MiniumPrice property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public byte MiniumPrice
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

                RaisePropertyChanging(MiniumPricePropertyName);
                _minimumPrice = value;
                RaisePropertyChanged(MiniumPricePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="MaximumPrice" /> property's name.
        /// </summary>
        public const string MaximumPricePropertyName = "MaximumPrice";

        private byte _maximumPrice = 30;

        /// <summary>
        /// Sets and gets the MaximumPrice property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public byte MaximumPrice
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

                RaisePropertyChanging(MaximumPricePropertyName);
                _maximumPrice = value;
                RaisePropertyChanged(MaximumPricePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CurrentPrice" /> property's name.
        /// </summary>
        public const string CurrentPricePropertyName = "CurrentPrice";

        private byte _currentPrice = 15;

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

        /// <summary>
        /// The <see cref="BigRise" /> property's name.
        /// </summary>
        public const string BigRisePropertyName = "BigRise";

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

                RaisePropertyChanging(BigRisePropertyName);
                _bigRise = value;
                PriceFactor = BL.PriceFactor.BIG_RISE;
                RaisePropertyChanged(BigRisePropertyName);
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
                PriceFactor = BL.PriceFactor.SMALL_RISE;
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
                PriceFactor = BL.PriceFactor.BIG_DECREASE;
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
                PriceFactor = BL.PriceFactor.SMALL_DECREASE;
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
                PriceFactor = BL.PriceFactor.OVERRIDE;
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
                    case BL.PriceFactor.BIG_DECREASE:
                        value += BigDecrease;
                        break;
                    case BL.PriceFactor.SMALL_DECREASE:
                        value += SmallDecrease;
                        break;
                    case BL.PriceFactor.BIG_RISE:
                        value += BigRise;
                        break;
                    case BL.PriceFactor.SMALL_RISE:
                        value += SmallRise;
                        break;
                    case BL.PriceFactor.OVERRIDE:
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
            return string.Format("[Drink]:[{0},{1}]", Id, Name);
        }

        public override bool Equals(object obj)
        {
            if (obj is Drink)
            {
                Drink other = obj as Drink;
                if (other.Id == this.Id) return true;
            }
            return false;
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
                case BL.PriceFactor.BIG_DECREASE:
                    return this.BigDecrease;
                case BL.PriceFactor.BIG_RISE:
                    return this.BigRise;
                case BL.PriceFactor.OVERRIDE:
                    return this.OverrideFactor;
                case BL.PriceFactor.SMALL_DECREASE:
                    return this.SmallDecrease;
                case BL.PriceFactor.SMALL_RISE:
                    return this.SmallRise;
            }

            return 1.0;
        }
    }

    public enum PriceFactor
    {
        [Description("Grote daling")]
        BIG_DECREASE,
        [Description("Kleine daling")]
        SMALL_DECREASE,
        [Description("Grote stijging")]
        BIG_RISE,
        [Description("Kleine stijging")]
        SMALL_RISE,
        [Description("Overschrijven")]
        OVERRIDE
    }

}
