﻿using System;
using System.Collections.Generic;
using System.Linq;
using Beursfuif.Server.Entity;

namespace Beursfuif.BL
{
    [Serializable]
    public class Interval : BFObservableObject
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

        public string AuthenticationString()
        {
            if (Drinks == null) return "No authentication string";

            //always sort the drink alphabetical by name
            string auth =  string.Join("", (from drink in Drinks
                                    where drink.Available
                                    orderby drink.Name.ToLower()
                                    select "["+drink.Id + ":" + drink.CurrentPrice+"]"
                        ));
            return $"({Id}){auth}";
        }
        #endregion

        public Interval()
        {

        }

        public void AddDrink(Drink drink)
        {
            if (Drinks == null)
            {
                Drinks = new Drink[] { drink };
                return;
            }
            
            List<Drink> currentDrinks = Drinks.ToList();
            currentDrinks.Add(drink);
            Drinks = currentDrinks.ToArray();

        }

        public void UpdateDrink(Drink drink)
        {
            if (Drinks == null) return;

            Drink dr = Drinks.FirstOrDefault(x => x.Id == drink.Id);
            if (dr != null)
            {
                int index = Array.IndexOf(Drinks, dr);
                Drinks[index] = drink;
            }
        }

        public void RemoveDrink(Drink drink)
        {
            if (Drinks == null) return;

            List<Drink> currentDrinks = Drinks.ToList();
            currentDrinks.Remove(currentDrinks.FirstOrDefault(x => x.Id == drink.Id));
            Drinks = currentDrinks.ToArray();

        }

        public override string ToString()
        {
            return $"[Interval]:[id = {Id},{StartTime}-{EndTime},code = {AuthenticationString()}]";
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

        public Interval Clone()
        {
            Interval clone = this.MemberwiseClone() as Interval;
            int drinkLength = this.Drinks.Length;
            Drink[] drinkClones = new Drink[drinkLength];
            for (int i = 0; i < drinkLength; i++)
            {
                drinkClones[i] = this.Drinks[i].Clone();
            }
            clone.Drinks = drinkClones;
            return clone;
        }
    }
}
