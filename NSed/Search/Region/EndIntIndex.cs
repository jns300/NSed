using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSed.Search.Region
{
    public class EndIntIndex : AbstractEndIndex
    {

        private int? endLineIndex;

        public EndIntIndex(int? endLineOffset, int? endLineIndex)
            : base(endLineOffset)
        {

            this.endLineIndex = endLineIndex;
        }

        protected override void FetchEndIndex(string[] lines, int? startIndex)
        {
            if (endLineIndex.HasValue)
            {
                int endIndex = endLineIndex.Value;
                if (endIndex < 0)
                {
                    // Counting from the end
                    endIndex = lines.Length + endIndex;
                }
                SetEndIndex(lines, endIndex);
            }
            else
            {
                // No end index
                SetExactEndIndex(lines.Length - 1);
            }
        }
    }
}
