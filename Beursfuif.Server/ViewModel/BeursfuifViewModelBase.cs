using Beursfuif.BL;
using Beursfuif.Server.Messages;
using Beursfuif.Server.Services;
using GalaSoft.MvvmLight;
using System.ComponentModel;
using Beursfuif.Server.Entity;

namespace Beursfuif.Server.ViewModel
{
    public abstract class BeursfuifViewModelBase:ViewModelBase
    {
        //this base class
        //implements the code to change the visible state of the view
        //Using an instance of IStateChange (View)
        //Unfortunately in WPF I can't make a base control for the views

        protected IBeursfuifData _beursfuifData;
        protected IStateChange _stateChanger;
        protected const string FADE_IN = "FadeIn";
        protected const string FADE_OUT = "FadeOut";
        protected bool _visible = true;

        protected DialogMessage _dm = new Beursfuif.Server.Messages.DialogMessage();

        /// <summary>
        /// The <see cref="BeursfuifBusy" /> property's name.
        /// </summary>
        public const string BeursfuifBusyPropertyName = "BeursfuifBusy";

        /// <summary>
        /// Sets and gets the BeursfuifBusy property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool BeursfuifBusy
        {
            get
            {
                return _beursfuifData.BeursfuifBusy;
            }
        }

        public bool NotBeursfuifBusy
        {
            get { return !BeursfuifBusy; }
        }

        public BeursfuifViewModelBase(IBeursfuifData beursfuifData)
        {
            _beursfuifData = beursfuifData;
            InitHideShowMessage();
        }

        private void InitHideShowMessage()
        {
            MessengerInstance.Register<ChangeVisibilityMessage>(this, ChangeState);
        }

        private void ChangeState(ChangeVisibilityMessage message)
        {
            string currentClassName = TypeDescriptor.GetClassName(this).Split('.')[TypeDescriptor.GetClassName(this).Split('.').Length - 1];
            if (!_visible && message.ClassName == currentClassName)
            {
                _stateChanger.GoToState(FADE_IN);
                _visible = true;
                //SendLogMessage("Changed view to " + currentClassName, LogType.INFO);
            }
            else if (_visible && message.ClassName != currentClassName)
            {
                _stateChanger.GoToState(FADE_OUT);
                _visible = false;
            }

        }

        public void SetStateChanger(IStateChange drinkView)
        {
            _stateChanger = drinkView;
        }

        public virtual void SendLogMessage(string msg, LogType type){
            MessengerInstance.Send<LogMessage>(new LogMessage(msg,type));
        }

        protected virtual void SendToastMessage(string title, string message = null)
        {
            MessengerInstance.Send<ToastMessage>(new ToastMessage(title, message));
        }

        public virtual void PointInCode(string where)
        {
            SendLogMessage(where, LogType.POINT_IN_CODE);
        }
    }
}
