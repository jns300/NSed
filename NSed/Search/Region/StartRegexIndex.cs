using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSed.Search.Region
{
    public class StartRegexIndex : AbstractStartIndex
    {
        private string startLineRegexStr;
        private bool useLastLine;

        private Regex startLineRegex;
        public StartRegexIndex(int? startLineOffset, string startLineRegexStr, bool useLastLine, bool caseSensitive)
            : base(startLineOffset)
        {
            this.startLineRegexStr = startLineRegexStr;
            this.useLastLine = useLastLine;

            RegexOptions options = !caseSensitive ? RegexOptions.IgnoreCase : 0;
            if (startLineRegexStr != null)
            {
                startLineRegex = new Regex(startLineRegexStr, options);
            }
        }

        protected override void FetchStartIndex(string[] lines)
        {
            if (startLineRegex == null)
            {
                SetExactStartIndex(0);
            }
            else
            {
                int lcount = lines.Length;
                for (int i = 0; i < lcount; i++)
                {
                    if (!IsStartIndexFound)
                    {
                        if (IsStartLine(i, lines))
                        {
                            SetStartIndex(i);
                            break;
                        }
                    }
                }
            }
        }

        private bool IsStartLine(int index, String[] lines)
        {
            if (startLineRegex != null)
            {
                return startLineRegex.IsMatch(lines[index]);
            }
            return false;
        }
    }
}
