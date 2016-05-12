using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using NSed.Search.Region;

namespace Tester.NSed.Search
{
    [TestFixture]
    public class SearchRegionMixedModeTest : AbstractRegionSearchTestBase
    {
        private String[] lines = new String[] { 
            "1", "2", "3", "4"
        };
        [Test]
        public void FindIndexMixedMode()
        {
            String[] emptyLines = new String[0];
            SearchRegion region = new SearchRegion(emptyLines,
                new StartIntIndex(null, 0),
                new EndRegexIndex(null, "a*", false, false), false);
            AssertRegionState(region, new string[0], new string[0], new string[0]);


            region = new SearchRegion(lines,
                new StartIntIndex(null, -4),
                new EndRegexIndex(null, "3", false, false), false);
            AssertRegionState(region, new string[] { }, new string[] { "4" }, new string[] { "1", "2", "3" });

            region = new SearchRegion(lines,
                new StartIntIndex(null, null),
                new EndRegexIndex(null, "3", false, false), false);
            AssertRegionState(region, new string[] { }, new string[] { "4" }, new string[] { "1", "2", "3" });

            region = new SearchRegion(lines,
                new StartRegexIndex(null, "2", false, false),
                new EndIntIndex(null, -2), false);
            AssertRegionState(region, new string[] { "1" }, new string[] { "4" }, new string[] { "2", "3" });

            region = new SearchRegion(lines,
                new StartRegexIndex(-1, "2", false, false),
                new EndIntIndex(1, -3), false);
            AssertRegionState(region, new string[] { }, new string[] { "4" }, new string[] { "1", "2", "3" });
        }
    }
}