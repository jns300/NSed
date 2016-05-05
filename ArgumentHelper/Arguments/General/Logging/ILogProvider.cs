using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments.General.Logging
{
    public interface ILogProvider
    {
        void Error(string message);
        void Error(string message, Exception ex);
        void Info(string message);
        void Debug(string message);
        void Debug(string message, Exception ex);
    }
}
