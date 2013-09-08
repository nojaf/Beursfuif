using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using Beursfuif.Server.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
        /// The <see cref="ChosenInterval" /> property's name.
        /// </summary>
        public const string ChosenIntervalPropertyName = "ChosenInterval";

        private TimeSpan _chosenInterval = TimeSpan.Zero;

        /// <summary>
        /// Sets and gets the ChosenInterval property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public TimeSpan ChosenInterval
        {
            get
            {
                return _chosenInterval;
            }

            set
            {
                if (_chosenInterval == value)
                {
                    return;
                }

                RaisePropertyChanging(ChosenIntervalPropertyName);
                _chosenInterval = value;
                RaisePropertyChanged(ChosenIntervalPropertyName);
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

            Intervals = _iomanager.LoadArrayFromBinary<Interval>(PathManager.INTERVAL_BINARY_PATH);
            if (Intervals != null)
            {
                BeginTime = Intervals[0].StartTime;
                EndTime = Intervals[Intervals.Length - 1].EndTime;
                ChosenInterval = IntervalChoices.FirstOrDefault(x => x.TotalMinutes == Intervals[0].Duration.TotalMinutes);
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
            TimeSpan period = EndTime.Subtract(BeginTime);
            int numberOfIntervals = 0;
            if(int.TryParse(""+(period.TotalMinutes / ChosenInterval.TotalMinutes),out numberOfIntervals))
            {
                CreateIntervals(numberOfIntervals);
                SaveIntervals();
                return;
            }

            //else throw a message that the settings aren't correct
            Console.WriteLine("Error");
            _dm = new DialogMessage("Het is onmogelijk om intervallen aan te maken.");
            _dm.Nay = System.Windows.Visibility.Collapsed;
            _dm.Errors.Add("Het is onmogelijk om een geheel aantal intervallen van \n\r"
                + ChosenInterval.TotalMinutes + " min aan te maken tussen "+
                BeginTime.ToShortTimeString() + " en " + EndTime.ToShortTimeString() + ".");
            MessengerInstance.Send<DialogMessage>(_dm);
        }


        private void CreateIntervals(int numberOfIntervals)
        {
            Intervals = new Interval[numberOfIntervals];
            for (int i = 0; i < numberOfIntervals; i++)
            {
                Intervals[i] = new Interval()
                {
                    StartTime = BeginTime.AddMinutes(i * ChosenInterval.TotalMinutes),
                    EndTime = BeginTime.AddMinutes((i+1) * ChosenInterval.TotalMinutes),
                    Id = i + 1
                };
            }
        }

        public void SaveIntervals()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(new Action<object>((object state) => {
                _iomanager.SaveArrayToBinary<Interval>(PathManager.INTERVAL_BINARY_PATH, Intervals);
            })));
            MessengerInstance.Send<ToastMessage>(new ToastMessage("Intervallen saved"));
        }

    }
}
