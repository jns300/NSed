using ArgumentHelper.Arguments.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgumentHelper.Arguments.FileFilters
{
    class TimeHelper
    {
        public static TimeEnum ParserTime(ref String value)
        {
            TimeEnum timeEnum;
            value = value.Trim();
            if (value.StartsWith("+"))
            {
                timeEnum = TimeEnum.Plus;
                value = value.Substring(1);
            }
            else if (value.StartsWith("-"))
            {
                timeEnum = TimeEnum.Minus;
                value = value.Substring(1);
            }
            else
            {
                timeEnum = TimeEnum.Exact;
            }
            return timeEnum;
        }

        public static bool TestDateDiff(TimeEnum? timeEnum, double dateDiff, int? n)
        {
            if (!timeEnum.HasValue)
                throw new ArgumentValidationException("time enum has no value");
            if (!n.HasValue)
                throw new ArgumentValidationException("n has no value");
            switch (timeEnum)
            {
                case TimeEnum.Plus:
                    return dateDiff > n.Value;
                case TimeEnum.Minus:
                    return dateDiff < n.Value;
                case TimeEnum.Exact:
                    return Math.Abs(Math.Round(dateDiff) - n.Value) < 1E-5;
                default:
                    throw new InvalidOperationException("enexpected time type: " + timeEnum);
            }
        }
    }
}
