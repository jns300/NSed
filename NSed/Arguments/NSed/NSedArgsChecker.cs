using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ArgumentHelper.Arguments.General;

namespace NSed.Arguments.NSed
{
    public class NSedArgsChecker : AbstractArgumentsChecker
    {
        private NSedAllowedArgs allowedArgs;

        public NSedArgsChecker(string[] args, NSedAllowedArgs allowedArgs)
            : base(args, allowedArgs, null)
        {
            this.allowedArgs = allowedArgs;
        }

        private static string[] RemoveLast(string[] args)
        {
            if (args.Length > 0)
            {
                String[] copy = new string[args.Length - 1];
                Array.Copy(args, copy, copy.Length);
                return copy;
            }
            return args;
        }
        public override void PerformCheck()
        {
            base.PerformCheck();
            if (GetArgCount(allowedArgs.FilePath) > 0 && !File.Exists(allowedArgs.FilePath.StringValue))
            {
                throw new ArgumentValidationException("file does not exist");
            }
            if (GetArgCount(allowedArgs.Scan) > 0 && !Directory.Exists(allowedArgs.Scan.StringValue))
            {
                throw new ArgumentValidationException("directory to scan does not exist");
            }
            if (GetArgCount(allowedArgs.FilePath) > 0 && GetArgCount(allowedArgs.Scan) > 0)
            {
                throw new ArgumentValidationException("when file name (-f) then dicrectory to scan should not (-scan)");
            }
            if (GetArgCount(allowedArgs.FilePath) == 0 && GetArgCount(allowedArgs.Scan) == 0)
            {
                throw new ArgumentValidationException("file name (-f) or dicrectory to scan (-scan) should be specified");
            }
            if (GetArgCount(allowedArgs.StartLineIndex) > 0 && GetArgCount(allowedArgs.StartLineRegex) > 0)
            {
                throw new ArgumentValidationException("start line index cannot be used with start line regex");
            }
            if (GetArgCount(allowedArgs.EndLineIndex) > 0 && GetArgCount(allowedArgs.EndLineRegex) > 0)
            {
                throw new ArgumentValidationException("end line index cannot be used with end line regex");
            }
            if (GetArgCount(allowedArgs.EndLineOffset) > 0
                && GetArgCount(allowedArgs.EndLineIndex) == 0 && GetArgCount(allowedArgs.EndLineRegex) == 0)
            {
                throw new ArgumentValidationException("end line index and regex are not specified but offset is specified");
            }
            if (GetArgCount(allowedArgs.UseLastLine) > 0
                && GetArgCount(allowedArgs.EndLineIndex) == 0 && GetArgCount(allowedArgs.EndLineRegex) == 0)
            {
                throw new ArgumentValidationException("It is specified that last line is to be used when end line index is not found, but end line index and regex is not specified");
            }
            if (GetArgCount(allowedArgs.StartLineOffset) > 0
                && GetArgCount(allowedArgs.StartLineIndex) == 0 && GetArgCount(allowedArgs.StartLineRegex) == 0)
            {
                throw new ArgumentValidationException("start line index is not specified but offset is specified");
            }
            if (GetArgCount(allowedArgs.EndLineIndex) > 0)
            {
                int index;
                if (!int.TryParse(allowedArgs.EndLineIndex.StringValue, out index))
                {
                    throw new ArgumentValidationException("end line index is not a valid integer");
                }
            }
            if (GetArgCount(allowedArgs.StartLineIndex) > 0)
            {
                int index;
                if (!int.TryParse(allowedArgs.StartLineIndex.StringValue, out index))
                {
                    throw new ArgumentValidationException("start line index is not a valid integer");
                }
            }
            if (GetArgCount(allowedArgs.EndLineOffset) > 0)
            {
                int index;
                if (!int.TryParse(allowedArgs.EndLineOffset.StringValue, out index))
                {
                    throw new ArgumentValidationException("end line offset is not a valid integer");
                }
            }
            if (GetArgCount(allowedArgs.StartLineOffset) > 0)
            {
                int index;
                if (!int.TryParse(allowedArgs.StartLineOffset.StringValue, out index))
                {
                    throw new ArgumentValidationException("start line offset is not a valid integer");
                }
            }
            if (GetArgCount(allowedArgs.DeletePermanently) > 0 && GetArgCount(allowedArgs.Replace) == 0)
            {
                throw new ArgumentValidationException("file cannot be deleted because replacement is not performed");
            }
        }
    }
}
