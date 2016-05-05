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
    public class FullRegionTest : AbstractRegionSearchTestBase
    {
        private String[] lines = new String[] { 
            "1", "2", "3", "4"
        };
        [Test]
        public void FindIndex()
        {
            FullRegion region = new FullRegion(new String[0]);
            AssertRegionState(region, new string[0], new string[0], new string[0]);

            region = new FullRegion(new String[] { "line1" });
            AssertRegionState(region, new string[0], new string[0], new string[] { "line1" });

            region = new FullRegion(lines);
            AssertRegionState(region, new string[0], new string[0], lines);
        }

    }
}