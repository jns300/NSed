using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSed.Search.Region
{
    public class IndexedRegion : SearchRegion
    {

        public IndexedRegion(String[] lines, int? startLineOffset, int? endLineOffset,
            int? startLineIndex, int? endLineIndex)
            : base(lines, new StartIntIndex(startLineOffset, startLineIndex),
            new EndIntIndex(endLineOffset, endLineIndex))
        {
        }
    }
}
