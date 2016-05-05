using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSed.Search;
using NSed.Arguments.NSed;
using System.Text.RegularExpressions;

namespace NSed
{
    class Program
    {
        static void Main(string[] args)
        {
            //PrintArgs(args);
            NSedAllowedArgs allowed = new NSedAllowedArgs();
            NSedArgsChecker argObj = new NSedArgsChecker(args, allowed);
            if (!argObj.PerformAllChecks())
                return;

            try
            {
                DirScan scan = new DirScan(allowed);
                scan.Scan();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Processing error:{0}{1}", Environment.NewLine, ex);
                Environment.ExitCode = 10;
            }
        }

        private static void PrintArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Console.Write(args[i]);
                Console.Write("   ");
            }
        }

    }
}
