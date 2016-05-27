using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSed.Search.Region
{
    public class RegexRegion : SearchRegion
    {
        public RegexRegion(String[] lines, int? startLineOffset, int? endLineOffset,
            string startLineRegexStr, string endLineRegexStr, bool useLastLine, bool caseSensitive, bool isLastCharNewLine)
            : base(lines, new StartRegexIndex(startLineOffset, startLineRegexStr, caseSensitive),
            new EndRegexIndex(endLineOffset, endLineRegexStr, useLastLine, caseSensitive), isLastCharNewLine)
        {
        }
    }
}
