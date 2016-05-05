using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSed.Search.Region
{
    public interface IStartRegionIndex
    {
        int? GetStartIndex(String[] lines);
    }
}
