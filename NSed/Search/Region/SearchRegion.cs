using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NSed.Search.Region;
using System.Diagnostics.Contracts;
using NSed.Util;

namespace NSed.Search.Region
{
    public class SearchRegion
    {

        protected string[] lines;
        private IStartRegionIndex startRegionIndex;
        private IEndRegionIndex endRegionIndex;
        private int lastLineIndex;

        private int? foundStartIndex;
        private int? foundEndIndex;
        private bool isLastCharNewLine;

        public SearchRegion(String[] lines, IStartRegionIndex startRegionIndex, IEndRegionIndex endRegionIndex, bool isLastCharNewLine)
        {
            CustomContract.Requires(lines != null);
            CustomContract.Requires(startRegionIndex != null);
            CustomContract.Requires(endRegionIndex != null);

            this.startRegionIndex = startRegionIndex;
            this.endRegionIndex = endRegionIndex;
            this.lines = lines;
            this.isLastCharNewLine = isLastCharNewLine;
            lastLineIndex = lines.Length - 1;
            FindIndexes();
        }

        protected void FindIndexes()
        {
            foundStartIndex = startRegionIndex.GetStartIndex(lines);
            if (foundStartIndex.HasValue)
            {
                foundEndIndex = endRegionIndex.GetEndIndex(lines, foundStartIndex.Value);
            }

        }


        public int SelectedLineCount
        {
            get
            {
                int selectedCount = 0;
                if (foundStartIndex.HasValue && foundEndIndex.HasValue)
                {
                    selectedCount = foundEndIndex.Value - foundStartIndex.Value + 1;
                }
                if (selectedCount < 0)
                {
                    selectedCount = 0;
                }
                return selectedCount;
            }
        }

        public IEnumerable<LineData> GetBeforeLines()
        {
            int end = lines.Length;
            if (foundStartIndex.HasValue && foundEndIndex.HasValue)
            {
                end = foundStartIndex.Value;
            }
            end = Math.Min(lines.Length, end);
            for (int i = 0; i < end; i++)
            {
                yield return CreateLineData(i);
            }
        }

        private LineData CreateLineData(int i)
        {
            return new LineData(lines[i], i == lastLineIndex, GetAppendNewLine(i));
        }

        private bool GetAppendNewLine(int index)
        {
            if(index == lastLineIndex)
            {
                return isLastCharNewLine;
            }
            return true;
        }

        public IEnumerable<LineData> GetAfterLines()
        {
            int start = lines.Length;
            if (foundEndIndex.HasValue)
            {
                if (SelectedLineCount > 0)
                {
                    start = foundEndIndex.Value + 1;
                }
                else if (foundStartIndex.HasValue)
                {
                    start = foundStartIndex.Value;
                }
            }
            int end = lines.Length;
            start = Math.Max(0, start);
            for (int i = start; i < end; i++)
            {
                yield return CreateLineData(i);
            }
        }


        public IEnumerable<LineData> GetSelectedLinesEnumerator()
        {
            if (SelectedLineCount > 0)
            {
                int end = Math.Min(lines.Length, foundEndIndex.Value + 1);
                int start = Math.Max(0, foundStartIndex.Value);
                for (int i = start; i < end; i++)
                {
                    yield return CreateLineData(i);
                }
            }
        }

        public string GetSelectedLines()
        {
            if (SelectedLineCount > 0)
            {
                String separator = Environment.NewLine;
                // Estimating capacity for the StringBuilder
                int bufferLen = 0;
                foreach (LineData line in GetSelectedLinesEnumerator())
                {
                    bufferLen += line.Line.Length + separator.Length;
                }
                
                // Creating the buffer
                StringBuilder sb = new StringBuilder(bufferLen);

                // Filling the StringBuilder
                foreach (LineData line in GetSelectedLinesEnumerator())
                {
                    sb.Append(line.Line);
                    if (line.AppendNewLine)
                    {
                        sb.Append(separator);
                    }
                }
                return sb.ToString();
            }
            return String.Empty;
        }

        public static SearchRegion GetSearchRegion(String[] lines, int? startLineIndex, int? endLineIndex,
            string startLineRegexStr, string endLineRegexStr,
            int? startLineOffset, int? endLineOffset, bool useLastLine, bool caseSensitive, bool isLastCharNewLine)
        {
            IStartRegionIndex startRegion;
            IEndRegionIndex endRegion;
            if (startLineRegexStr != null)
            {
                startRegion = new StartRegexIndex(startLineOffset, startLineRegexStr, caseSensitive);
            }
            else
            {
                startRegion = new StartIntIndex(startLineOffset, startLineIndex);
            }

            if (endLineRegexStr != null)
            {
                endRegion = new EndRegexIndex(endLineOffset, endLineRegexStr, useLastLine, caseSensitive);
            }
            else
            {
                endRegion = new EndIntIndex(endLineOffset, endLineIndex);
            }
            SearchRegion region = new SearchRegion(lines, startRegion, endRegion, isLastCharNewLine);
            return region;
        }

    }
}
