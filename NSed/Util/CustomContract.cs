using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSed.Util
{
    public class CustomContract
    {

        public static void Requires(bool predicate, string message = null)
        {
            if (!predicate)
            {
                if (message == null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    throw new ArgumentException(message);
                }
            }
        }

        public static void Requires<TException>(bool predicate, string message = null)
            where TException : Exception, new()
        {
            if (!predicate)
            {
                if (message == null || !HasStringConstructor<TException>())
                {
                    throw new TException();
                }
                else
                {
                    var exception = (TException)Activator.CreateInstance(typeof(TException), message);
                    throw exception;
                }
            }
        }

        private static bool HasStringConstructor<T>()
        {
            var ctors = typeof(T).GetConstructors();
            var found = ctors.Where(c =>
            {
                var parameters = c.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType.Equals(typeof(String));
            }).FirstOrDefault();
            return found != null;
        }
    }

}
