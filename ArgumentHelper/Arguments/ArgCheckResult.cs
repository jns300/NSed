using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments
{
    /// <summary>
    /// Represents a result of argument checking process.
    /// </summary>
    public enum ArgCheckResult
    {
        /// <summary>
        /// The program can be continues.
        /// </summary>
        Continue,
        /// <summary>
        /// The program has to be stopped, but no error was found.
        /// </summary>
        Stop,
        /// <summary>
        /// The program has to be stopped, since an error was found.
        /// </summary>
        ErrorFound,
    }
}
