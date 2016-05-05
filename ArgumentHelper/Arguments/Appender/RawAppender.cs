using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments.Appender
{
    /// <summary>
    /// Appends text to the string builder without modifications.
    /// </summary>
    public class RawAppender : ITextAppender
    {
        public void AppendFormattedText(StringBuilder builder, string rawText, int maxNameLen, int minRawTextLineLen, int nextLinesIndent = 0)
        {
            builder.Append(rawText).Append(Environment.NewLine);
        }
    }
}
