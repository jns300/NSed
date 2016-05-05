using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NSed.Search.Region;

namespace NSed.Search.Region
{
    public class SearchRegion
    {

        protected string[] lines;
        private IStartRegionIndex startRegionIndex;
        private IEndRegionIndex endRegionIndex;

        private int? foundStartIndex;
        private int? foundEndIndex;

        public SearchRegion(String[] lines, IStartRegionIndex startRegionIndex, IEndRegionIndex endRegionIndex)
        {
            this.startRegionIndex = startRegionIndex;
            this.endRegionIndex = endRegionIndex;
            this.lines = lines;
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

        public IEnumerable<string> GetBeforeLines()
        {
            int end = lines.Length;
            if (foundStartIndex.HasValue && foundEndIndex.HasValue)
            {
                end = foundStartIndex.Value;
            }
            end = Math.Min(lines.Length, end);
            for (int i = 0; i < end; i++)
            {
                yield return lines[i];
            }
        }

        public IEnumerable<string> GetAfterLines()
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
                yield return lines[i];
            }
        }


        public IEnumerable<string> GetSelectedLinesEnumerator()
        {
            if (SelectedLineCount > 0)
            {
                int end = Math.Min(lines.Length, foundEndIndex.Value + 1);
                int start = Math.Max(0, foundStartIndex.Value);
                for (int i = start; i < end; i++)
                {
                    yield return lines[i];
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
                foreach (String line in GetSelectedLinesEnumerator())
                {
                    bufferLen += line.Length + separator.Length;
                }
                
                // Creating the buffer
                StringBuilder sb = new StringBuilder(bufferLen);

                // Filling the StringBuilder
                foreach (String line in GetSelectedLinesEnumerator())
                {
                    sb.Append(line);
                    sb.Append(separator);
                }
                return sb.ToString();
            }
            return String.Empty;
        }

        public static SearchRegion GetSearchRegion(String[] lines, int? startLineIndex, int? endLineIndex,
            string startLineRegexStr, string endLineRegexStr,
            int? startLineOffset, int? endLineOffset, bool useLastLine, bool caseSensitive)
        {
            IStartRegionIndex startRegion;
            IEndRegionIndex endRegion;
            if (startLineRegexStr != null)
            {
                startRegion = new StartRegexIndex(startLineOffset, startLineRegexStr, useLastLine, caseSensitive);
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
            SearchRegion region = new SearchRegion(lines, startRegion, endRegion);
            return region;
        }

    }
}
