using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using Beursfuif.Server.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.ViewModel
{
    public class IntervalViewModel : BeursfuifViewModelBase
    {
        private TimeSpan[] _intervalChoices = new TimeSpan[]{
            new TimeSpan(0,10,0),
            new TimeSpan(0,15,0),
            new TimeSpan(0,20,0),
            new TimeSpan(0,30,0),
            new TimeSpan(0,45,0),
            new TimeSpan(1,0,0)
        };
        public TimeSpan[] IntervalChoices
        {
            get
            {
                return _intervalChoices;
            }
        }

        /// <summary>
        /// The <see cref="BeginTime" /> property's name.
        /// </summary>
        public const string BeginTimePropertyName = "BeginTime";

        private DateTime _beginTime = new DateTime(1970, 1, 1, 21, 0, 0);

        /// <summary>
        /// Sets and gets the BeginTime property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime BeginTime
        {
            get
            {
                return _beginTime;
            }

            set
            {
                if (_beginTime == value)
                {
                    return;
                }

                RaisePropertyChanging(BeginTimePropertyName);
                _beginTime = value;
                RaisePropertyChanged(BeginTimePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="EndTime" /> property's name.
        /// </summary>
        public const string EndTimePropertyName = "EndTime";

        private DateTime _endTime = new DateTime(1970, 1, 2, 5, 0, 0);

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

        /// <summary>
        /// The <see cref="Intervals" /> property's name.
        /// </summary>
        public const string IntervalsPropertyName = "Intervals";

        private Interval[] _intervals = null;

        /// <summary>
        /// Sets and gets the Intervals property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Interval[] Intervals
        {
            get
            {
                return _intervals;
            }

            set
            {
                if (_intervals == value)
                {
                    return;
                }

                RaisePropertyChanging(IntervalsPropertyName);
                _intervals = value;
                RaisePropertyChanged(IntervalsPropertyName);
            }
        }

        public RelayCommand GenerateIntervalsCommand
        {
            get;
            set;
        }

        private IOManager _iomanager;

        public IntervalViewModel(IOManager iomanager)
            : base()
        {
            _iomanager = iomanager;
            if (IsInDesignMode)
            {
                Intervals = new Interval[]{
                    new Interval(){
                        Id = 1,
                        StartTime = new DateTime(1970,1,1,21,0,0),
                        EndTime = new DateTime(1970,1,1,21,15,0)
                    },
                    new Interval(){
                        Id = 2,
                        StartTime = new DateTime(1970,1,1,21,15,0),
                        EndTime = new DateTime(1970,1,1,21,30,0)
                    },
                    new Interval(){
                        Id = 3,
                        StartTime = new DateTime(1970,1,1,21,30,0),
                        EndTime = new DateTime(1970,1,1,21,45,0)
                    },
                    new Interval(){
                        Id = 4,
                        StartTime = new DateTime(1970,1,1,21,45,0),
                        EndTime = new DateTime(1970,1,1,22,00,0)
                    }
                };
            }

            InitCommands();
        }

        private void InitCommands()
        {
            GenerateIntervalsCommand = new RelayCommand(GenerateIntervals, () => { return !BeursfuifBusy; });
        }

        private void GenerateIntervals()
        {
            //Validate if the hours and interval match
        }

    }
}
