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
            AssertRegionState(region, beforeLines, afterLines, selected, true);
        }
        protected void AssertRegionState(SearchRegion region, string[] beforeLines, string[] afterLines,
            String[] selected, bool isLatsCharNewLine)
        {
            Assert.AreEqual(beforeLines, region.GetBeforeLines().Select(ld => ld.Line).ToArray());
            Assert.AreEqual(afterLines, region.GetAfterLines().Select(ld => ld.Line).ToArray());
            Assert.AreEqual(selected.Length, region.SelectedLineCount);
            StringBuilder sb = new StringBuilder();
            foreach (String s in selected)
            {
                sb.Append(s);
                sb.Append(Environment.NewLine);
            }
            if (!isLatsCharNewLine && sb.Length > 0)
            {
                sb.Remove(sb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            }
            Assert.AreEqual(sb.ToString(), region.GetSelectedLines());
        }
    }
}
