using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beursfuif.Server.DataAccess
{
    public interface IIOManager
    {
        T Load<T>(string path) where T:class;

        void Save<T>(string path, T entity) where T : class;

    }
}
