using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSed.Search.Region
{
    public abstract class AbstractEndIndex : IEndRegionIndex
    {

        protected int? endLineOffset;

        private int? foundEndIndex;

        public AbstractEndIndex(int? endLineOffset)
        {
            this.endLineOffset = endLineOffset;
        }
        protected bool IsEndIndexFound { get { return foundEndIndex.HasValue; } }
        protected int FoundEndIndex
        {
            get { return foundEndIndex.Value; }
        }

        protected int? NullableFoundEndIndex
        {
            get { return foundEndIndex; }
        }

        protected void SetExactEndIndex(int i)
        {
            foundEndIndex = i;
        }

        protected void SetEndIndex(String[] lines, int i)
        {
            foundEndIndex = i;
            if (endLineOffset.HasValue)
            {
                foundEndIndex += endLineOffset.Value;
            }
            if (foundEndIndex >= lines.Length)
            {
                foundEndIndex = lines.Length - 1;
            }
        }

        public int? GetEndIndex(String[] lines, int startIndex)
        {
            FetchEndIndex(lines, startIndex);
            return NullableFoundEndIndex;
        }
        protected abstract void FetchEndIndex(String[] lines, int? startIndex);
    }
}
