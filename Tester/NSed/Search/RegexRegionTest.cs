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
    public class RegexRegionTest : AbstractRegionSearchTestBase
    {

        private String[] lines = new String[] { 
            "1", "2", "3", "4"
        };
        [Test]
        public void FindIndex()
        {
            RegexRegion region = new RegexRegion(new String[0], null, null, "1", "2", false, false);
            AssertRegionState(region, new string[0], new string[0], new string[0]);

            region = new RegexRegion(new String[] { "line1" }, null, null, "line2", "line3", false, false);
            AssertRegionState(region, new string[] { "line1" }, new string[0], new string[0]);

            region = new RegexRegion(new String[] { "line1" }, null, null, "line1", "line1", false, false);
            AssertRegionState(region, new string[0], new string[0], new string[] { "line1" });

            region = new RegexRegion(lines, null, null, "2", "10", true, false);
            AssertRegionState(region, new string[] { "1" }, new string[0], new string[] { "2", "3", "4" });

            region = new RegexRegion(lines, null, null, "-5", "1", false, false);
            AssertRegionState(region, new string[] { "1", "2", "3", "4" }, new string[] { }, new string[] { });

            region = new RegexRegion(lines, null, null, "2", "2", false, false);
            AssertRegionState(region, new string[] { "1" }, new string[] { "3", "4" }, new string[] { "2" });

            region = new RegexRegion(lines, 1, null, "2", "2", false, false);
            AssertRegionState(region, new string[] { "1", "2", "3", "4" }, new string[] { }, new string[] { });

            region = new RegexRegion(lines, 1, null, "2", "2", true, false);
            AssertRegionState(region, new string[] { "1", "2" }, new string[] { }, new string[] { "3", "4" });

            region = new RegexRegion(lines, 2, 3, "2", "4", false, false);
            AssertRegionState(region, new string[] { "1", "2", "3" }, new string[] { }, new string[] { "4" });

            region = new RegexRegion(lines, -1, -2, "2", "3", false, false);
            AssertRegionState(region, new string[] { }, new string[] { "2", "3", "4" }, new string[] { "1" });

            region = new RegexRegion(lines, -1, -2, "3", null, false, false);
            AssertRegionState(region, new string[] { "1" }, new string[] { }, new string[] { "2", "3", "4" });

            region = new RegexRegion(lines, -1, -2, null, "3", false, false);
            AssertRegionState(region, new string[] { }, new string[] { "2", "3", "4" }, new string[] { "1" });

            region = new RegexRegion(new String[] { "a", "b", "c" }, null, null, "B", "C", false, false);
            AssertRegionState(region, new string[] { "a" }, new string[] { }, new string[] { "b", "c" });

            region = new RegexRegion(new String[] { "a", "b", "c" }, null, null, "B", "C", false, true);
            AssertRegionState(region, new string[] { "a", "b", "c" }, new string[] { }, new string[] { });
        }

    }
}