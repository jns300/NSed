using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments.General
{
    /// <summary>
    /// Represents an exception thrown when command line arguments are validate.
    /// </summary>
    [Serializable]
    public class ArgumentValidationException : Exception
    {
        public ArgumentValidationException(String message)
            :base(message)
        {
        }

        public ArgumentValidationException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
