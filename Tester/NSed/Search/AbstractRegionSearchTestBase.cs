using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using NSed.Search;
using NSed.Search.Region;

namespace Tester.NSed.Search
{
    public class AbstractRegionSearchTestBase
    {

        protected void AssertRegionState(SearchRegion region, string[] beforeLines, string[] afterLines,
            String[] selected)
        {
            Assert.AreEqual(beforeLines, region.GetBeforeLines().ToArray());
            Assert.AreEqual(afterLines, region.GetAfterLines().ToArray());
            Assert.AreEqual(selected.Length, region.SelectedLineCount);
            StringBuilder sb = new StringBuilder();
            foreach (String s in selected)
            {
                sb.Append(s);
                sb.Append(Environment.NewLine);
            }
            Assert.AreEqual(sb.ToString(), region.GetSelectedLines());
        }
    }
}
