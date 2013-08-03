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
        protected IStateChange _stateChanger;
        protected const string FADE_IN = "FadeIn";
        protected const string FADE_OUT = "FadeOut";
        protected bool _visible = true;
        protected DialogMessage _dm = new Beursfuif.Server.Messages.DialogMessage();

        public BeursfuifViewModelBase()
        {
            InitHideShowMessage();
        }

        private void InitHideShowMessage()
        {
            MessengerInstance.Register<ChangeVisibilityMessage>(this, ChangeState);
        }

        private void ChangeState(ChangeVisibilityMessage message)
        {
            string currentClassName = TypeDescriptor.GetClassName(this).Split('.')[TypeDescriptor.GetClassName(this).Split('.').Length - 1];
            Console.WriteLine(currentClassName);
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
