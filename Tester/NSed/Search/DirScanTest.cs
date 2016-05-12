using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using NSed.Search;
using NSed.Arguments.NSed;
using ArgumentHelper.Arguments.FileFilters;
using System.IO;

namespace Tester.NSed.Search
{
    [TestFixture]
    public class DirScanTest
    {
        private class TestContextDataProvider : IContextDataProvider
        {
            public TestContextDataProvider(DateTime modificationTime)
            {
                ModificationTime = modificationTime;
            }

            public DateTime GetModificationTime(System.IO.FileInfo file)
            {
                return ModificationTime;
            }

            public DateTime ModificationTime { get; private set; }
        }

        private const String tempFilePath = @".\scanTest\tempFile.txt";

        private static TestData testData;

        [TestFixtureSetUp]
        public static void ClassSetUp()
        {
            testData = new TestData();
            testData.CreateFiles();
        }

        [TestFixtureTearDown]
        public static void ClassCleanUp()
        {
            testData.CleanUp();
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
        [Test]
        public void GetRelativePath()
        {
            Assert.AreEqual("./file.txt", DirScan.GetRelativePath(@"x:\a", @"x:\a\file.txt"));
            Assert.AreEqual("./a/file.txt", DirScan.GetRelativePath(@"x:\", @"x:\a\file.txt"));
            Assert.AreEqual("./c/file.txt", DirScan.GetRelativePath(@"x:\a\b", @"x:\a\b\c\file.txt"));
        }

        [Test]
        public void ScanBackslash()
        {
            File.WriteAllText(tempFilePath, "winodws\\abc\\10");
            FilterContext.SetDataProvider(new TestContextDataProvider(DateTime.Now.AddMinutes(-10.0)));
            TestScan(new String[] { "-scan", @".\scanTest", "-name", "tempFile.txt", ";", "-find", "windows\\abc\\", "-replace", "windows\\efg\\" },
                new String[] { "./tempFile.txt" });
        }

        [Test]
        public void Scan()
        {
            FilterContext.SetDataProvider(new TestContextDataProvider(DateTime.Now.AddMinutes(-10.0)));
            TestScan(new String[] { "-scan", @".\scanTest\depth", "-mindepth", "2", "-maxdepth", "4", "-mmin", "+4", ";", "-find", "value" },
                new String[] { "./a/1.txt", "./a/b/2.txt", "./a/b/c/3.txt" });

            TestNValues(true);
            TestNValues(false);

            TestScan(new String[] { "-scan", @".\scanTest\depth", "-mindepth", "1", "-not", "-path-regex", "3|4", ";", "-find", "value" },
                new String[] { "./0.txt", "./a/1.txt", "./a/b/2.txt" });

            TestScan(new String[] { "-scan", @".\scanTest\depth", "-name", "0.*", "-or", "-name", "4.*", ";", "-find", "value" },
                new String[] { "./0.txt", "./a/b/c/d/4.txt" });

            TestScan(new String[] { "-scan", @".\scanTest\depth", "-name", "0.*", "-or", "-path-regex", "/c/[^/]*$", ";", "-find", "value" },
                new String[] { "./0.txt", "./a/b/c/3.txt" });
        }

        [Test]
        public void ScanSingle()
        {
            ScanSingleWithParameters(true);
        }

        [Test]
        public void ScanSingleNoNewLine()
        {
            ScanSingleWithParameters(false);
        }

        private void ScanSingleWithParameters(bool appendNewLine)
        {
            String filePath = "singleScanFile.txt";
            try
            {
                String nl = appendNewLine ? Environment.NewLine : String.Empty;
                File.WriteAllText(filePath, "a b c " + nl);
                TestScan(new String[] { "-del", "-f", filePath, "-find", "c\\s+$*", "-replace", "" },
                    new String[] { });
                String actual = File.ReadAllText(filePath);
                Assert.AreEqual("a b " + nl, actual);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        private void TestNValues(bool bMTime)
        {

            String timeParam = bMTime ? "-mtime" : "-mmin";
            FilterContext.SetDataProvider(new TestContextDataProvider(GetDateTime(bMTime, -6.0)));
            TestScan(new String[] { "-scan", @".\scanTest\depth", "-mindepth", "1", "-maxdepth", "5", timeParam, "+5", ";", "-find", "value" },
                new String[] { "./0.txt", "./a/1.txt", "./a/b/2.txt", "./a/b/c/3.txt", "./a/b/c/d/4.txt" });

            TestScan(new String[] { "-scan", @".\scanTest\depth", "-mindepth", "1", "-maxdepth", "5", timeParam, "+7", ";", "-find", "value" },
                new String[] { });

            FilterContext.SetDataProvider(new TestContextDataProvider(GetDateTime(bMTime, -3)));
            TestScan(new String[] { "-scan", @".\scanTest\depth", "-mindepth", "1", "-maxdepth", "5", timeParam, "-5", ";", "-find", "value" },
                new String[] { "./0.txt", "./a/1.txt", "./a/b/2.txt", "./a/b/c/3.txt", "./a/b/c/d/4.txt" });

            TestScan(new String[] { "-scan", @".\scanTest\depth", "-mindepth", "1", "-maxdepth", "5", timeParam, "-2", ";", "-find", "value" },
                new String[] { });

            FilterContext.SetDataProvider(new TestContextDataProvider(GetDateTime(bMTime, -3)));
            TestScan(new String[] { "-scan", @".\scanTest\depth", "-mindepth", "1", "-maxdepth", "5", timeParam, "3", ";", "-find", "value" },
                new String[] { "./0.txt", "./a/1.txt", "./a/b/2.txt", "./a/b/c/3.txt", "./a/b/c/d/4.txt" });

            TestScan(new String[] { "-scan", @".\scanTest\depth", "-mindepth", "1", "-maxdepth", "5", timeParam, "4", ";", "-find", "value" },
                new String[] { });
        }

        private DateTime GetDateTime(bool bDays, double value)
        {
            return bDays ? DateTime.Now.AddDays(value) : DateTime.Now.AddMinutes(value);
        }


        private void TestScan(String[] args, String[] expectedFiles)
        {
            // Console.WriteLine(ConcatArgs(args));
            NSedAllowedArgs allowed = new NSedAllowedArgs();
            NSedArgsChecker checker = new NSedArgsChecker(args, allowed);
            checker.PerformCheck();
            DirScan scan = new DirScan(allowed);
            scan.Scan();
            Assert.AreEqual(expectedFiles, scan.FoundFiles);
        }

        public static String ConcatenateArgs(String[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String s in args)
            {
                sb.Append(s);
                sb.Append(" ");
            }
            return sb.ToString();
        }
    }
}