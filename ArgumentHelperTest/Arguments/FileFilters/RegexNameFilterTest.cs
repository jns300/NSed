using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArgumentHelper.Arguments.General;
using ArgumentHelper.Arguments.General.Attributes;
using System.IO;
using ArgumentHelper.Arguments.FileFilters;

namespace ArgumentHelperTest.Arguments.FileFilters
{
    [TestClass]
    public class RegexNameFilterTest
    {
        [TestMethod]
        public void Test()
        {
            String path = "x:/doc/document.txt";
            FileInfo info = new FileInfo(path);
            FilterContext context = new FilterContext(info, path, 2);
            Assert.IsTrue(new RegexNameFilter(".*").Test(context));
            Assert.IsTrue(new RegexNameFilter(".*\\..*t").Test(context));
            Assert.IsTrue(new RegexNameFilter("^docu.*\\..*txt$").Test(context));
            Assert.IsFalse(new RegexNameFilter("^docu.*\\..*pdf$").Test(context));
        }
    }
}
