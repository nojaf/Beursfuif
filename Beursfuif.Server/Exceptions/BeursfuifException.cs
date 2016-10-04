using System;

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
