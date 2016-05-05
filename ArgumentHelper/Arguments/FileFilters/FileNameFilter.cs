using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArgumentHelper.Arguments.General.Operators;
using System.Text.RegularExpressions;
using ArgumentHelper.Arguments.FileFilters;

namespace ArgumentHelper.Arguments.FileFilters
{
    public class FileNameFilter : AbstractArgTreeItem
    {
        private Regex regex;

        public FileNameFilter()
        {
        }
        public FileNameFilter(String namePattern)
        {
            CreateRegex(namePattern);
        }

        private void CreateRegex(String namePattern)
        {
            // Converting pattern with chars * and ? to regular expression
            StringBuilder sb = new StringBuilder();
            sb.Append("^");
            int len = namePattern.Length;
            int lastEscapeChar = 0;
            for (int i = 0; i < len; i++)
            {
                char ch = namePattern[i];
                String toReplace = null;
                if (ch == '*')
                {
                    toReplace = ".*";
                }
                else if (ch == '?')
                {
                    toReplace = ".";
                }
                if (toReplace != null)
                {
                    if (i - lastEscapeChar > 0)
                    {
                        sb.Append(Regex.Escape(namePattern.Substring(lastEscapeChar, i - lastEscapeChar)));
                    }
                    lastEscapeChar = i + 1;
                    sb.Append(toReplace);
                }
            }
            if (len - lastEscapeChar > 0)
            {
                sb.Append(Regex.Escape(namePattern.Substring(lastEscapeChar, len - lastEscapeChar)));
            }
            sb.Append("$");
            regex = new Regex(sb.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public override bool Test(ITestContext context)
        {
            return regex.IsMatch(((FilterContext)context).FileName);
        }
        public override bool Validate()
        {
            return regex != null;
        }
        public override bool IsValueRequired()
        {
            return true;
        }

        public override void SetValue(String value)
        {
            CreateRegex(value);
        }
        public override string ItemName
        {
            get { return "name"; }
        }
    }
}
