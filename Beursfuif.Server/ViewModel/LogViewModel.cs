using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using Beursfuif.Server.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Beursfuif.Server.Entity;

namespace Beursfuif.Server.ViewModel
{
    public class LogViewModel:BeursfuifViewModelBase
    {
        #region Properties
        /// <summary>
        /// The <see cref="AllLogMessages" /> property's name.
        /// </summary>
        public const string AllLogMessagesPropertyName = "AllLogMessages";

        private List<LogMessage> _allLogMessages = new List<LogMessage>();

        /// <summary>
        /// Sets and gets the AllLogMessages property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public List<LogMessage> AllLogMessages
        {
            get
            {
                return _allLogMessages;
            }

            set
            {
                if (_allLogMessages == value)
                {
                    return;
                }

                RaisePropertyChanging(AllLogMessagesPropertyName);
                _allLogMessages = value;
                RaisePropertyChanged(AllLogMessagesPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedLogMessages" /> property's name.
        /// </summary>
        public const string SelectedLogMessagesPropertyName = "SelectedLogMessages";

        private ObservableCollection<LogMessage> _selectedLogMessages = new ObservableCollection<LogMessage>();

        /// <summary>
        /// Sets and gets the SelectedLogMessages property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<LogMessage> SelectedLogMessages
        {
            get
            {
                return _selectedLogMessages;
            }

            set
            {
                if (_selectedLogMessages == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedLogMessagesPropertyName);
                _selectedLogMessages = value;
                RaisePropertyChanged(SelectedLogMessagesPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedLogType" /> property's name.
        /// </summary>
        public const string SelectedLogTypePropertyName = "SelectedLogType";

        private LogType _selectedLogType = LogType.ALL;

        /// <summary>
        /// Sets and gets the SelectedLogType property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public LogType SelectedLogType
        {
            get
            {
                return _selectedLogType;
            }

            set
            {
                if (_selectedLogType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedLogTypePropertyName);
                _selectedLogType = value;
                UpdateSelectedLogItems();
                RaisePropertyChanged(SelectedLogTypePropertyName);
            }
        }

        #endregion

        public LogViewModel(IBeursfuifData data) : base(data)
        {
            if (!IsInDesignMode)
            {
                MessengerInstance.Register<LogMessage>(this, LogMessageReceived);
                _beursfuifData.BeursfuifDataImported += BeursfuifData_BeursfuifDataImported;
                _beursfuifData.DataReset += BeursfuifData_DataReset;
            }
        }

        void BeursfuifData_DataReset(object sender, bool e)
        {
            RaisePropertyChanged(AllLogMessagesPropertyName);
            RaisePropertyChanged(SelectedLogTypePropertyName);
            RaisePropertyChanged(SelectedLogMessagesPropertyName);
        }

        void BeursfuifData_BeursfuifDataImported(object sender, EventArgs e)
        {
            RaisePropertyChanged(AllLogMessagesPropertyName);
            RaisePropertyChanged(SelectedLogTypePropertyName);
            RaisePropertyChanged(SelectedLogMessagesPropertyName);
        }

        private void UpdateSelectedLogItems()
        {
            var query = AllLogMessages.Where(x => x.Type.HasFlag(SelectedLogType));
            SelectedLogMessages.Clear();
            foreach (var item in query)
            {
                SelectedLogMessages.Add(item);
            }
        }

        private void LogMessageReceived(LogMessage logMessage)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                AllLogMessages.Add(logMessage);
                if (logMessage.Type.HasFlag(SelectedLogType))
                {
                    SelectedLogMessages.Add(logMessage);
                }
            }));
            LogManager.AppendToLog(logMessage);
        }
    }
}
