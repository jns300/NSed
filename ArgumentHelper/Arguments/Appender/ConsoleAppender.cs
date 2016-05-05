using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments.Appender
{
    /// <summary>
    /// Helper class which allows to append messages to a buffer in the way in which words printed on a console.
    /// wiil be correctly broken.
    /// </summary>
    public class ConsoleAppender : ITextAppender
    {
        public const int CONSOLE_LINE_LEN = 79;

        public static readonly bool PAD_LINES = false;

        /// <summary>
        /// Appends text the the string builder.
        /// </summary>
        /// <param name="builder">the string builder</param>
        /// <param name="rawText">the text to be appended</param>
        /// <param name="maxNameLen">the maximum length of names which precede the raw text</param>
        /// <param name="minRawTextLineLen">the minimum line length for the raw text</param>
        /// <param name="nextLinesIndent">the indention of the next lines</param>
        public void AppendFormattedText(StringBuilder builder, string rawText, int maxNameLen, int minRawTextLineLen, int nextLinesIndent = 0)
        {
            String text = rawText.Replace("\t", "".PadLeft(9, ' '));
            int start = 0;
            int realMaxNameLen = maxNameLen % CONSOLE_LINE_LEN;
            // Desired length of the line without padding
            int desiredLen = Math.Max(CONSOLE_LINE_LEN - realMaxNameLen, minRawTextLineLen);
            int allChars = text.Length;
            // Used to in the stop condition
            int printed = 0;
            int firstPartLen = Math.Min(desiredLen, CONSOLE_LINE_LEN - realMaxNameLen);
            int partLen = firstPartLen;
            int lineIndex = 0;
            while (printed < allChars)
            {
                int diff = allChars - (start + partLen);
                int validLen = diff > 0 ? partLen : partLen + diff;
                int substringLen;
                // Returns substring which do not breake lines
                var part = GetValidSubstring(text, start, validLen, out substringLen);
                part = part.TrimStart().PadRight(partLen);
                int tmpPartLen = part.Length;
                printed += substringLen;
                if (printed < allChars)
                    // this is not the last part
                    part = part.TrimStart().PadRight(tmpPartLen, ' ');
                // adding padding
                String padded;
                if (lineIndex > 0)
                    padded = part.PadLeft(part.Length + (CONSOLE_LINE_LEN - desiredLen), ' ');
                else
                    padded = part;
                if (!PAD_LINES)
                {
                    padded = padded.TrimEnd() + Environment.NewLine;
                }
                builder.Append(padded);
                start += substringLen;
                if (lineIndex == 0)
                    desiredLen -= nextLinesIndent;
                partLen = desiredLen;
                lineIndex++;
            }
        }

        /// <summary>
        /// Returns substring which tries to do not divide words.
        /// </summary>
        /// <param name="value">the input string</param>
        /// <param name="start">the start index</param>
        /// <param name="length">the substring max length</param>
        /// <returns>the substring without divided words</returns>
        public static String GetValidSubstring(string value, int start, int length, out int selectedLength)
        {
            if (length == 0)
            {
                selectedLength = 0;
                return String.Empty;
            }
            StringBuilder sb = new StringBuilder(length);
            int newWordLen;
            int lastWordEnd = start;
            bool isNewLine = false;
            int ommitedChars = 0;
            do
            {
                newWordLen = 0;
                for (int i = lastWordEnd; i < value.Length; i++)
                {
                    newWordLen++;
                    if (value[i] == '\n')
                    {
                        isNewLine = true;
                        break;
                    }
                    if (Char.IsWhiteSpace(value[i]))
                    {
                        break;
                    }
                }
                if (sb.Length + newWordLen > length)
                {
                    if (sb.Length == 0)
                    {
                        ommitedChars += AppendToBuffer(sb, value.Substring(start, length));
                    }
                    break;
                }
                else if (newWordLen > 0)
                {
                    ommitedChars += AppendToBuffer(sb, value.Substring(lastWordEnd, newWordLen));
                    lastWordEnd += newWordLen;
                }
            } while (newWordLen > 0 && !isNewLine);
            selectedLength = sb.Length + ommitedChars;
            if (sb.Length < length)
            {
                sb.Append(String.Empty.PadRight(length - sb.Length, ' '));
            }
            return sb.ToString();
        }

        private static int AppendToBuffer(StringBuilder builder, string text)
        {
            int ommitedChars = 0;
            // Append word to the buffer
            foreach (char c in text)
            {
                if (c != '\r' && c != '\n')
                    builder.Append(c);
                else
                    ommitedChars++;
            }
            return ommitedChars;
        }

        /// <summary>
        /// Appneds new line character to the text. When the line length is equal to console line len
        /// then does nothing.
        /// </summary>
        /// <param name="text">the text to modify</param>
        /// <returns>the full line created from the text</returns>
        public static String CreateFullLine(string text)
        {
            if (text.Length % CONSOLE_LINE_LEN == 0 && PAD_LINES)
            {
                return text;
            }
            else
            {
                return text += Environment.NewLine;
            }
        }

        /// <summary>
        /// Appends spaces at the end of the text in order.
        /// </summary>
        /// <param name="text">the inputtext</param>
        /// <param name="totalWidth">the width of the output line</param>
        /// <returns>the padded line</returns>
        public static String PadLineRight(string text, int totalWidth)
        {
            if (totalWidth >= CONSOLE_LINE_LEN)
            {
                if (PAD_LINES)
                {
                    text = text.PadRight(totalWidth);
                }
                else
                {
                    if (text.Length <= CONSOLE_LINE_LEN)
                    {
                        text += Environment.NewLine;
                        text += String.Empty.PadRight(totalWidth % CONSOLE_LINE_LEN);
                    }
                }
            }
            else
            {
                text = text.PadRight(totalWidth);
            }
            return text;
        }
    }
}
