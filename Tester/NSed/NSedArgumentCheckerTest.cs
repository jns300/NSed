using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using NSed;
using System.IO;
using NSed.Arguments.NSed;
using ArgumentHelper.Arguments.General;

namespace Tester.NSed
{
    [TestFixture]
    public class NSedArgumentCheckerTest
    {
        private const String testFile = "testFile.txt";

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
        public void TearDown()
        {
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
            }
        }

        [Test]
        public void Operations()
        {
            NSedAllowedArgs args = new NSedAllowedArgs();
            args.Initialize();
            String sli = args.StartLineIndex.Names.First();
            String eli = args.EndLineIndex.Names.First();
            String slr = args.StartLineRegex.Names.First();
            String elr = args.EndLineRegex.Names.First();
            File.WriteAllText(testFile, "");
            AssertThrows(new String[] { }, true);
            AssertThrows(new String[] { "-f" }, true);
            AssertThrows(new String[] { "-f", testFile }, true);
            AssertThrows(new String[] { "-f", testFile, "-find", "value" }, false);
            AssertThrows(new String[] { "-f", testFile, "-find", }, true);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-replace", "abc", 
                "-ml", "-case", sli, "1", eli, "2"}, false);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-replace", "abc", 
                "-ml", "-case", slr, "(ab*)", elr, "\\1c"}, false);

            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-replace", "abc", 
                "-ml", "-case", slr, "(ab*)", elr, "\\1c", eli, "2"}, true);

            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-replace", "abc", 
                "-ml", "-case", slr, "(ab*)", elr, "\\1c", eli, "2"}, true);

            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-replace", "abc", 
                "-ml", "-case", slr, "(ab*)", elr, "\\1c", "-ml"}, true);

            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-replace", "abc", 
                "-ml", "-case", slr, "(ab*)", elr, "\\1c", "-case"}, true);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-slo", "-1" }, true);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-slo", "-1", "-sli", "1" }, false);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", args.UseLastLine.Names.First() }, true);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", args.UseLastLine.Names.First(), "-sli", "1" }, true);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", args.UseLastLine.Names.First(), "-eli", "1" }, false);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-slo", "1", "-elo", "1" }, true);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-slo", "1", "-elo", "1", "-sli", "1", "-eli", "1" }, false);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-slo", "b", "-elo", "1", "-sli", "1", "-eli", "1" }, true);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-slo", "1", "-elo", "c2", "-sli", "1", "-eli", "1" }, true);
            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-del" }, true);
            AssertThrows(new String[] { "-find", "value", "-scan", @".\scanTest\abc", "-mindepth", "2", "-maxdepth", "5" }, false);
            // Simulteniesly File and Scan
            AssertThrows(new String[] { "-f", testFile, "-find", "value", "-scan", @".\scanTest\abc", "-mindepth", "2" }, true);
            // Arg Regex in wrong place
            AssertThrows(new String[] { "-regex", "../[^/]*/abc/", "-find", "value", "-scan", @".\scanTest\abc", "-mindepth", "2" }, true);
            AssertThrows(new String[] { "-path-regex", "../[^/]*/abc/", "-find", "value", "-scan", @".\scanTest\abc", "-mindepth", "2" }, true);
            // Arg Sli in wron place
            AssertThrows(new String[] { "-find", "value", "-scan", @".\scanTest\abc", "-mindepth", "2", "-sli", "1" }, true);
            AssertThrows(new String[] { "-scan", @".\scanTest\abc", "-mindepth", "2", ";", "-find", "value" }, false);
            // Arg Mindepth not after -scan
            AssertThrows(new String[] { "-scan", @".\scanTest\abc", "-name", "*.txt", "-mindepth", "2", ";", "-find", "value" }, true);
        }

        [Test]
        public void OperatorParsing()
        {
            AssertThrows(new String[] { "-scan", @".\scanTest\abc", "-mindepth", "2", "-and", ";", "-find", "value" }, true);
            AssertThrows(new String[] { "-scan", @".\scanTest\abc", "-mindepth", "2", "-or", "-and", ";", "-find", "value" }, true);

            CheckOperators(new String[] { "-scan", @".\scanTest\abc", "-mindepth", "2", ";", "-find", "value" }, false,
                "-scan", "empty()");
            CheckOperators(new String[] { "-find", "v", "-scan", @".\scanTest\abc", "-mindepth", "2", "-maxdepth", "4", "-mtime", "4", "-and", "-mmin", "10", "-or", "-mtime", "1" }, false,
                "-scan", "or(and(mtime(), mmin()), mtime())");

            CheckOperators(new String[] { "-find", "v", "-scan", @".\scanTest\abc", "-mtime", "2", "-and", "(", "-mmin", "-4", "-or", "-mmin", "+1", ")" }, false,
                "-scan", "and(mtime(), or(mmin(), mmin()))");
            CheckOperators(new String[] { "-find", "v", "-scan", @".\scanTest\abc", "-mtime", "+2", "-and", "-not", "(", "-mmin", "-4", "-or", "-not", "-mtime", "1", ")" }, false,
                "-scan", "and(mtime(), not(or(mmin(), not(mtime()))))");
            // Missing and/or
            CheckOperators(new String[] { "-find", "v", "-scan", @".\scanTest\abc", "-mindepth", "2",
                "-and", "(", "(", "-name", "*.txt", "-or", "-regex", "\\d+", ")", 
                    "-mmin", "4", "-or", "-mtime", "1", ")" }, true, "-scan", null);
            // Invalid bracketes
            CheckOperators(new String[] { "-find", "v", "-scan", @".\scanTest\abc", 
                "(", "(", "-name", "*.txt", "-or", "-path-regex", "\\d+", ")", 
                    "-and", "-mmin", "4", "-or", "-mtime", "1"}, true, "-scan", null);
            // Invalid bracketes
            CheckOperators(new String[] { "-find", "v", "-scan", @".\scanTest\abc", 
                "(", "(", "-name", "*.txt", "-or", "-path-regex", "\\d+", ")", ")",
                    "-and", "-mmin", "4", "-or", "-mtime", "1", ")"}, true, "-scan", null);
            // Invalid bracketes
            CheckOperators(new String[] { "-find", "v", "-scan", @".\scanTest\abc", 
                "(", "(", "-name", "*.txt", "-or", "-regex", "\\d+", ")", 
                    "-and", "-mmin", "4", "-or", "-mtime", "1", ")", ")" }, true, "-scan", null);
            CheckOperators(new String[] { "-find", "v", "-scan", @".\scanTest\abc", "-mtime", "2",
                "-and", "(", "(", "-name", "*.txt", "-or", "-path-regex", "\\d+", "-or", "-regex", "\\d{2}", ")", "-and", 
                    "-mmin", "4", "-or", "-mmin", "1", ")" }, false,
                "-scan", "and(mtime(), or(and(or(or(name(), path-regex()), regex()), mmin()), mmin()))");
        }

        private void CheckOperators(string[] args, bool bThrow, string mainArgName, string expectedTree)
        {
            NSedAllowedArgs allowed = AssertThrows(args, bThrow);
            if (!bThrow)
            {
                String actualTree = allowed.Scan.GetFilteringTree().ToString();
                Assert.AreEqual(expectedTree, actualTree);
            }
        }
        private NSedAllowedArgs AssertThrows(string[] args, bool bThrow)
        {
            NSedAllowedArgs allowed = new NSedAllowedArgs();
            NSedArgsChecker checker = new NSedArgsChecker(args, allowed);
            if (bThrow)
            {
                Assert.Throws(typeof(ArgumentValidationException), delegate { checker.PerformCheck(); });
            }
            else
            {
                Assert.DoesNotThrow(delegate
                {
                    try
                    {
                        checker.PerformCheck();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                });
            }
            return allowed;
        }
    }
}