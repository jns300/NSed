using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments.Appender
{
    /// <summary>
    /// Interface for classes which append text to console.
    /// </summary>
    public interface ITextAppender
    {
        void AppendFormattedText(StringBuilder builder, string rawText, int maxNameLen, 
            int minRawTextLineLen, int nextLinesIndent = 0);
    }
}
