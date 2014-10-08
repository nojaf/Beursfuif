using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beursfuif.BL.Exceptions
{
    public class BeursfuifException : Exception
    {
        public BeursfuifException():base()
        {

        }

        public BeursfuifException(string mesage):base(mesage)
        {

        }

        public BeursfuifException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
