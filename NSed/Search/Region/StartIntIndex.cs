using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSed.Search.Region
{
    public class StartIntIndex : AbstractStartIndex
    {
        private int? startLineIndex;
        
        public StartIntIndex(int? startLineOffset, int? startLineIndex)
            : base(startLineOffset)
        {
            this.startLineIndex = startLineIndex;
        }

        protected override void FetchStartIndex(string[] lines)
        {
            if (startLineIndex.HasValue)
            {
                int startIndex = startLineIndex.Value;
                if (startIndex < 0)
                {
                    // Counting from the end
                    startIndex = lines.Length + startIndex;
                }
                SetStartIndex(startIndex);
            }
            else
            {
                SetExactStartIndex(0);
            }
        }


    }
}
