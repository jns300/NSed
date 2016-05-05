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
    public class FileNameFilterTest
    {
        [TestMethod]
        public void Test()
        {
            String path = "x:/doc/document.txt";
            FileInfo info = new FileInfo(path);
            FilterContext context = new FilterContext(info, path, 2);
            Assert.IsTrue(new FileNameFilter("*.*").Test(context));
            Assert.IsTrue(new FileNameFilter("*.*t").Test(context));
            Assert.IsTrue(new FileNameFilter("doc*.*t").Test(context));
            Assert.IsTrue(new FileNameFilter("doc*t.*").Test(context));
            Assert.IsFalse(new FileNameFilter("*.pdf").Test(context));
        }

        [TestMethod]
        public void TestNameOnly()
        {
            String path = "x:/doc/document.txt.cs.csproj";
            FileInfo info = new FileInfo(path);
            FilterContext context = new FilterContext(info, path, 2);
            Assert.IsFalse(new FileNameFilter("*.cs").Test(context));
            Assert.IsFalse(new FileNameFilter("ment*.csproj").Test(context));
            Assert.IsTrue(new FileNameFilter("*ment*.csproj").Test(context));
            Assert.IsTrue(new FileNameFilter("*.csproj").Test(context));
        }
    }
}