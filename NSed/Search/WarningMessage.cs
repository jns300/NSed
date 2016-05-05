using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSed.Search
{
    public class WarningMessage
    {
        static WarningMessage()
        {
        }
        public static bool Warn { get; set; }

        internal static void WarnWildcardsForGroup()
        {
            if (Warn)
            {
                Console.Error.WriteLine("WARNING: wildcards (*, +, ?, {xx}), are applied on a group. This is may cause problem in group numbering, which is Java like but not .NET like.");
            }
        }
    }
}
