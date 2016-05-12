using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using NSed.Search;
using NSed.Search.Region;

namespace Tester.NSed.Search
{
    [TestFixture]
    public class IndexedRegionTest : AbstractRegionSearchTestBase
    {
        private String[] lines = new String[] { 
            "1", "2", "3", "4"
        };
        [Test]
        public void FindIndex()
        {
            IndexedRegion region = new IndexedRegion(new String[0], null, null, 1, 2, false);
            AssertRegionState(region, new string[0], new string[0], new string[0]);

            region = new IndexedRegion(new String[] { "line1" }, null, null, 1, 2, false);
            AssertRegionState(region, new string[] { "line1" }, new string[0], new string[0]);

            region = new IndexedRegion(new String[] { "line1" }, null, null, 0, 0, true);
            AssertRegionState(region, new string[0], new string[0], new string[] { "line1" });

            region = new IndexedRegion(new String[] { "line1" }, null, null, 0, 0, false);
            AssertRegionState(region, new string[0], new string[0], new string[] { "line1" }, false);

            region = new IndexedRegion(lines, null, null, 1, 10, true);
            AssertRegionState(region, new string[] { "1" }, new string[0], new string[] { "2", "3", "4" });

            region = new IndexedRegion(lines, null, null, -5, 1, true);
            AssertRegionState(region, new string[] { }, new string[] { "3", "4" }, new string[] { "1", "2" });

            region = new IndexedRegion(lines, null, null, -2, -2, true);
            AssertRegionState(region, new string[] { "1", "2" }, new string[] { "4" }, new string[] { "3" });

            region = new IndexedRegion(lines, 1, null, -2, -2, true);
            AssertRegionState(region, new string[] { "1", "2", "3" }, new string[] { "4" }, new string[] { });

            region = new IndexedRegion(lines, 2, null, -2, -2, true);
            AssertRegionState(region, new string[] { "1", "2", "3", "4" }, new string[] { }, new string[] { });

            region = new IndexedRegion(lines, 2, 3, -3, -3, true);
            AssertRegionState(region, new string[] { "1", "2", "3" }, new string[] { }, new string[] { "4" });

            region = new IndexedRegion(lines, -1, -2, -2, -1, true);
            AssertRegionState(region, new string[] { "1" }, new string[] { "3", "4" }, new string[] { "2" });

            region = new IndexedRegion(lines, -1, -2, -2, null, true);
            AssertRegionState(region, new string[] { "1" }, new string[] { }, new string[] { "2", "3", "4" });

            region = new IndexedRegion(lines, null, null, null, null, true);
            AssertRegionState(region, new string[] { }, new string[] { }, new string[] { "1", "2", "3", "4" });
        }

    }
}