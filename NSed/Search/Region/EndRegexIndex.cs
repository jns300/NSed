using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSed.Search.Region
{
    public class EndRegexIndex : AbstractEndIndex
    {
        private string endLineRegexStr;
        private bool useLastLine;

        private Regex endLineRegex;

        public EndRegexIndex(int? endLineOffset, string endLineRegexStr, bool useLastLine, bool caseSensitive)
            : base(endLineOffset)
        {
            this.endLineRegexStr = endLineRegexStr;
            this.useLastLine = useLastLine;


            RegexOptions options = !caseSensitive ? RegexOptions.IgnoreCase : 0;
            if (endLineRegexStr != null)
            {
                endLineRegex = new Regex(endLineRegexStr, options);
            }
        }
        protected override void FetchEndIndex(string[] lines, int? startIndex)
        {

            if (startIndex.HasValue)
            {
                if (!CanHaveEndIndex)
                {
                    // Offset can not be specified in this case
                    SetExactEndIndex(lines.Length - 1);
                }
                else
                {
                    int lcount = lines.Length;
                    for (int i = startIndex.Value; i < lcount; i++)
                    {
                        if (IsEndLine(lines, i))
                        {
                            SetEndIndex(lines, i);
                            break;
                        }
                    }
                }
            }
            if (startIndex.HasValue && !IsEndIndexFound && useLastLine)
            {
                // Offset here cannot be used
                SetExactEndIndex(lines.Length - 1);
            }
        }

        private bool IsEndLine(String[] lines, int index)
        {
            if (endLineRegex != null)
            {
                return endLineRegex.IsMatch(lines[index]);
            }
            return false;
        }

        private bool CanHaveEndIndex
        {
            get
            {
                return endLineRegex != null;
            }
        }

    }
}
