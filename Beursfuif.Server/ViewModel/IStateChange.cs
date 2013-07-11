using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.ViewModel
{
    public interface IStateChange
    {
        void GoToState(string name, bool transition = true, string who = null);
    }
}
