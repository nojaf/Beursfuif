using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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

        private IOManager _ioManager;

        public LogViewModel(IOManager ioManager)
        {
            if (!IsInDesignMode)
            {
                _ioManager = ioManager;

                MessengerInstance.Register<LogMessage>(this, LogMessageReceived);
            }
        }

        private void LogMessageReceived(LogMessage lm)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                AllLogMessages.Add(lm);
                if (lm.Type.HasFlag(SelectedLogType))
                {
                    SelectedLogMessages.Add(lm);
                }
            }));
            LogManager.AppendToLog(lm);
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
    }
}
