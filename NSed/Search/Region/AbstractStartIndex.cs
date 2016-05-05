using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSed.Search.Region
{
    public abstract class AbstractStartIndex : IStartRegionIndex
    {
        protected int? startLineOffset;

        private int? foundStartIndex;

        public AbstractStartIndex(int? startLineOffset)
        {
            this.startLineOffset = startLineOffset;
        }
        protected void SetExactStartIndex(int i)
        {
            foundStartIndex = i;
        }

        protected void SetStartIndex(int i)
        {
            foundStartIndex = i;
            if (startLineOffset.HasValue)
            {
                foundStartIndex += startLineOffset.Value;
            }
            if (foundStartIndex < 0)
            {
                foundStartIndex = 0;
            }
        }


        protected bool IsStartIndexFound { get { return foundStartIndex.HasValue; } }
        protected int FoundStartIndex
        {
            get { return foundStartIndex.Value; }
        }
        protected int? NullableFoundStartIndex
        {
            get { return foundStartIndex; }
        }
        public int? GetStartIndex(String[] lines)
        {
            FetchStartIndex(lines);
            return NullableFoundStartIndex;
        }
        protected abstract void FetchStartIndex(String[] lines);
    }
}
