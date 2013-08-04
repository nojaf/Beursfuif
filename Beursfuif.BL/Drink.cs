using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Beursfuif.BL
{
    [Serializable]
    public class Drink:BFObservableObject
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
                RaisePropertyChanged("ImageUri");
            }
        }

        [XmlIgnore]
        public Uri ImageUri
        {
            get
            {
                if (!string.IsNullOrEmpty(ImageString))
                {
                    return new Uri(ImageString, UriKind.RelativeOrAbsolute);
                }
                return null;
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

        private byte _initialPrice = 10;

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

        private byte _minimumPrice = 0;

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

        private byte _maximumPrice = 20;

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

        /// <summary>
        /// The <see cref="NextPriceAddition" /> property's name.
        /// </summary>
        public const string NextPriceAdditionPropertyName = "NextPriceAddition";

        private byte _nextPriceAddiction = 0;

        /// <summary>
        /// Sets and gets the NextPriceAddition property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public byte NextPriceAddition
        {
            get
            {
                return _nextPriceAddiction;
            }

            set
            {
                if (_nextPriceAddiction == value)
                {
                    return;
                }

                RaisePropertyChanging(NextPriceAdditionPropertyName);
                _nextPriceAddiction = value;
                RaisePropertyChanged(NextPriceAdditionPropertyName);
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
    }
    
}
