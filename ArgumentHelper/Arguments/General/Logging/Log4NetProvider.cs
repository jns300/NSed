using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments.General.Logging
{
    public class Log4NetProvider : ILogProvider
    {
        private ILog log;

        public Log4NetProvider(ILog log)
        {
            if (log == null) throw new ArgumentNullException();
            this.log = log;
        }
        public void Info(string message)
        {
            log.Info(message);
        }

        public void Error(string message)
        {
            log.Error(message);
        }

        public void Error(string message, Exception ex)
        {
            log.Error(message, ex);
        }


        public void Debug(string message)
        {
            log.Debug(message);
        }

        public void Debug(string message, Exception ex)
        {
            log.Debug(message, ex);
        }
    }
}
