using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSed.Search
{
    /// <summary>
    /// Contains data describing line from the input file.
    /// </summary>
    public struct LineData
    {
        public LineData(String line, bool isLast, bool appendNewLine)
        {
            Contract.Requires(line != null);
            Line = line;
            IsLast = isLast;
            AppendNewLine = appendNewLine;
        }
        public bool AppendNewLine { get; private set; }
        public bool IsLast { get; private set; }
        public String Line { get; private set; }
    }
}
