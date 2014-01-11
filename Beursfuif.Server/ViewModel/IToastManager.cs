using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.ViewModel
{
    public interface IToastManager
    {
        void ShowToast(string title, string message = "");
    }
}
