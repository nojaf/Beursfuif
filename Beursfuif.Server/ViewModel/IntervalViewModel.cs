using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using Beursfuif.Server.Messages;
using Beursfuif.Server.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beursfuif.Server.ViewModel
{
    public class IntervalViewModel : BeursfuifViewModelBase
    {
        #region properties and fields
        private IBeursfuifData _beursfuifData;

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


        /// <summary>
        /// Sets and gets the BeginTime property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime BeginTime
        {
            get
            {
                return _beursfuifData.BeginTime;
            }

            set
            {
                if (_beursfuifData.BeginTime == value)
                {
                    return;
                }

                RaisePropertyChanging(BeginTimePropertyName);
                _beursfuifData.BeginTime = value;
                RaisePropertyChanged(BeginTimePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="EndTime" /> property's name.
        /// </summary>
        public const string EndTimePropertyName = "EndTime";



        /// <summary>
        /// Sets and gets the EndTime property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return _beursfuifData.EndTime;
            }

            set
            {
                if (_beursfuifData.EndTime == value)
                {
                    return;
                }

                RaisePropertyChanging(EndTimePropertyName);
                _beursfuifData.EndTime = value;
                RaisePropertyChanged(EndTimePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Intervals" /> property's name.
        /// </summary>
        public const string IntervalsPropertyName = "Intervals";


        /// <summary>
        /// Sets and gets the Intervals property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Interval[] Intervals
        {
            get
            {
                return _beursfuifData.Intervals;
            }

            set
            {
                if (_beursfuifData.Intervals == value)
                {
                    return;
                }

                RaisePropertyChanging(IntervalsPropertyName);
                _beursfuifData.Intervals = value;
                RaisePropertyChanged(IntervalsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CanModify" /> property's name.
        /// </summary>
        public const string CantModifyPropertyName = "CanModify";

        private bool _canModify = true;

        /// <summary>
        /// Sets and gets the CantModify property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool CanModify
        {
            get
            {
                return _canModify;
            }

            set
            {
                if (_canModify == value)
                {
                    return;
                }

                RaisePropertyChanging(CantModifyPropertyName);
                _canModify = value;
                RaisePropertyChanged(CantModifyPropertyName);
            }
        }

        public RelayCommand GenerateIntervalsCommand
        {
            get;
            set;
        }
        #endregion



        public IntervalViewModel(IBeursfuifData beursfuifData): base(beursfuifData)
        {
            if (IsInDesignMode)
            {
                #region DummyData
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
                #endregion
            }
            else
            {
                PointInCode("IntervalViewModel: Ctor");

                _beursfuifData = beursfuifData;

                if (Intervals != null && Intervals.Any())
                {
                    BeginTime = Intervals[0].StartTime;
                    EndTime = Intervals[Intervals.Length - 1].EndTime;
                    ChosenInterval = IntervalChoices.FirstOrDefault(x => x.TotalMinutes == Intervals[0].Duration.TotalMinutes);
                    SendLogMessage("Intervals found in data folder", LogType.INTERVAL_VM);
                }

                InitCommands();
                CheckCanEdit();

                _beursfuifData.BeursfuifBusyChanged += BeursfuifData_BeursfuifBusyChanged;
                _beursfuifData.DataReset += BeursfuifData_DataReset;
                _beursfuifData.BeursfuifDataImported += BeursfuifData_BeursfuifDataImported;
            }
        }

        void BeursfuifData_BeursfuifDataImported(object sender, EventArgs e)
        {
            RaisePropertyChanged(IntervalsPropertyName);
            RaisePropertyChanged(BeginTimePropertyName);
            RaisePropertyChanged(EndTimePropertyName);
            CheckCanEdit();
        }

        void BeursfuifData_DataReset(object sender, bool e)
        {
            RaisePropertyChanged(IntervalsPropertyName);
            RaisePropertyChanged(ChosenIntervalPropertyName);
            RaisePropertyChanged(BeginTimePropertyName);
            RaisePropertyChanged(EndTimePropertyName);
            CheckCanEdit();
        }

        void BeursfuifData_BeursfuifBusyChanged(object sender, bool e)
        {
            CheckCanEdit();
        }

        #region InitCommands
        private void InitCommands()
        {
            PointInCode("IntervalViewModel: InitCommands");

            GenerateIntervalsCommand = new RelayCommand(GenerateIntervals, () => { return !BeursfuifBusy; });
        }

        private void GenerateIntervals()
        {
            PointInCode("IntervalViewModel: GenerateIntervals");

            if (BeginTime.CompareTo(EndTime) == 1)
            {
                _dm = new DialogMessage("Begin ligt na einde");
                _dm.Nay = System.Windows.Visibility.Collapsed;
                _dm.Errors.Add("De gewenste instelling is onmogelijk");
                _dm.Errors.Add("Het begintijdstip moet plaats vinden voor het eindtijdstip");
                MessengerInstance.Send<DialogMessage>(_dm);
                SendLogMessage("Error creating Intervals, begin is greater than end", LogType.INTERVAL_VM | LogType.USER_ERROR);
                return;
            }

            //Validate if the hours and interval match
            TimeSpan period = EndTime.Subtract(BeginTime);
            int numberOfIntervals = 0;
            if(int.TryParse(""+(period.TotalMinutes / ChosenInterval.TotalMinutes),out numberOfIntervals))
            {
                Task.Factory.StartNew(CreateIntervals, numberOfIntervals);
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
            SendLogMessage("Error creating Intervals, " + string.Join(";", _dm.Errors.ToArray()), LogType.INTERVAL_VM | LogType.USER_ERROR);
        }
        #endregion

        private void CreateIntervals(object state)
        {
            PointInCode("IntervalViewModel: CreateIntervals");


            int numberOfIntervals = Convert.ToInt32(state);
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

            SendLogMessage("New intervals created", LogType.INTERVAL_VM);

            SaveIntervals();
        }

        public void SaveIntervals()
        {
            PointInCode("IntervalViewModel: SaveIntervals");


            ThreadPool.QueueUserWorkItem(new WaitCallback(new Action<object>((object state) => {
                _beursfuifData.SaveIntervals();
            })));
            SendToastMessage("Intervallen saved");
            SendLogMessage("Intervallen saved", LogType.INTERVAL_VM);
        }

        private void CheckCanEdit()
        {
            CanModify = !_beursfuifData.BeursfuifEverStarted;
            RaisePropertyChanged(CantModifyPropertyName);
        }

    }
}
