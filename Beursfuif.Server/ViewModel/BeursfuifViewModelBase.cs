using Beursfuif.Server.Messages;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.ViewModel
{
    public class BeursfuifViewModelBase:ViewModelBase
    {
        //this base class
        //implements the code to change the visible state of the view
        //Using an instance of IStateChange (View)
        //Unfortunately in WPF I can't make a base control for the views

        protected IStateChange _stateChanger;
        protected const string FADE_IN = "FadeIn";
        protected const string FADE_OUT = "FadeOut";
        protected bool _visible = true;

        public bool BeursfuifBusy { get; set; }

        public BeursfuifViewModelBase()
        {
            InitHideShowMessage();
        }

        private void InitHideShowMessage()
        {
            MessengerInstance.Register<ChangeVisibilityMessage>(this, ChangeState);
            MessengerInstance.Register<BeursfuifBusyMessage>(this, ChangePartyBusy);
        }

        private void ChangePartyBusy(BeursfuifBusyMessage obj)
        {
            BeursfuifBusy = obj.Value;
        }

        private void ChangeState(ChangeVisibilityMessage message)
        {
            string currentClassName = TypeDescriptor.GetClassName(this).Split('.')[TypeDescriptor.GetClassName(this).Split('.').Length - 1];
            if (!_visible && message.ClassName == currentClassName)
            {
                _stateChanger.GoToState(FADE_IN);
                _visible = true;
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


    }
}
