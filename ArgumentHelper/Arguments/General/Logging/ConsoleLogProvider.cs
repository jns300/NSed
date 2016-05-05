using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments.General.Logging
{
    public class ConsoleLogProvider : ILogProvider
    {

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            Console.Error.WriteLine(message);
        }

        public void Error(string message, Exception ex)
        {
            Console.Error.WriteLine("{0}{2}Exception details: {1}", message.TrimEnd(), ex, Environment.NewLine);
        }


        public void Debug(string message)
        {
            Error(message);
        }

        public void Debug(string message, Exception ex)
        {
            Error(message, ex);
        }
    }
}
