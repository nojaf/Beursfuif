using Beursfuif.Server.DataAccess;
using Beursfuif.Server.Messages;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.ViewModel
{
    public class IntervalViewModel : BeursfuifViewModelBase
    {
        private IOManager _iomanager;

        public IntervalViewModel(IOManager iomanager):base()
        {
            
        }

    }
}
