using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSed.Search.Region
{
    public class FullRegion : SearchRegion
    {
        private class ConstStartIndex : IStartRegionIndex
        {
            public int? GetStartIndex(String[] lines)
            {
                return 0;
            }
        }
        private class ConstEndIndex : IEndRegionIndex
        {

            public ConstEndIndex()
            {
            }
            public int? GetEndIndex(String[] lines, int startIndex)
            {
                return lines.Length - 1;
            }
        }
        public FullRegion(String[] lines)
            :base(lines, new ConstStartIndex(), new ConstEndIndex())
        {
        }
    }
}
