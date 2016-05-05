using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using NSed;
using NSed.Search;
using System.IO;
using System.Text.RegularExpressions;
using NSed.Arguments.NSed;
using System.Threading;

namespace Tester.NSed.Search
{
    [TestFixture]
    public class SearchTest
    {
        private const String MANIFEST_TEST_FILE = "ManifestForTests.txt";

        private const String TEMP_TEST_FILE = "TmpTestFile.txt";

        [SetUp]
        public void SetUp()
        {
            DeleteTempFile();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTempFile();
        }

        private static void DeleteTempFile()
        {
            if (File.Exists(TEMP_TEST_FILE))
            {
                File.Delete(TEMP_TEST_FILE);
            }
            if (File.Exists(MANIFEST_TEST_FILE))
            {
                File.Delete(MANIFEST_TEST_FILE);
            }
        }

        private void WriteManifestFile()
        {
            File.WriteAllText(MANIFEST_TEST_FILE, TestResources.ManifestForTests);
        }

        [Test]
        public void Construct()
        {
            WriteManifestFile();
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(MANIFEST_TEST_FILE, 1);
            args.Find.SetFoundValues("bundle-version=\"[^\"]*\"", 1);
            SearchAndReplace search = new SearchAndReplace(args, null);
            Assert.IsFalse(search.DeleteFilePermanently);

            args.DeletePermanently.SetFoundValues(String.Empty, 1);
            search = new SearchAndReplace(args, null);
            Assert.IsTrue(search.DeleteFilePermanently);
        }

        [Test]
        public void Find()
        {
            WriteManifestFile();
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(MANIFEST_TEST_FILE, 1);
            args.Find.SetFoundValues("bundle-version=\"[^\"]*\"", 1);
            SearchAndReplace search = new SearchAndReplace(args, null);
            search.Perform();
            Assert.AreEqual(11, search.MatchCount);
        }

        [Test]
        public void Replace1()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("(Bundle-version=)[^,]*(,)?", 1);
            args.Replace.SetFoundValues("\\1[1.0.0,1.0.2)]\\2", 1);
            String content = "bundle-version=1.0.0,\nbundle-version=2.0.0,";
            String replacedContent = "bundle-version=[1.0.0,1.0.2)]," + Environment.NewLine
                + "bundle-version=[1.0.0,1.0.2)]," + Environment.NewLine;
            ReplaceInFile(args, content, replacedContent, true);

            // Case-sesitive check
            args.CaseSensitive.SetFoundValues(String.Empty, 1);
            ReplaceInFile(args, content, replacedContent, false, false);
        }

        [Test]
        public void Replace2()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("(^\\s+version)=(1.0.0)", 1);
            args.Replace.SetFoundValues("\\1=1.0.3", 1);
            args.Multiline.SetFoundValues(String.Empty, 1);
            String content = "\r\n\tversion=1.0.0\r\n";
            String replacedContent = "\r\n\tversion=1.0.3\r\n";
            ReplaceInFile(args, content, replacedContent, true);

            // singleline 
            args.Multiline.SetFoundValues(String.Empty, 0);
            ReplaceInFile(args, content, replacedContent, true);
        }

        [Test]
        public void FindStartAndEndIndex()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("\\s+", 1);
            args.StartLineIndex.SetFoundValues("0", 1);
            args.EndLineIndex.SetFoundValues("5", 1);

            String fileContent = "1  \r\n2  \r\n3 ";
            File.WriteAllText(TEMP_TEST_FILE, fileContent);

            SearchAndReplace search = new SearchAndReplace(args, null);
            search.Perform();
            Assert.AreEqual(3, search.MatchCount);
        }

        [Test]
        public void FindStartLineRegex()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("\\s+", 1);
            args.StartLineRegex.SetFoundValues("version=1\\.0\\.3", 1);
            args.StartLineOffset.SetFoundValues("1", 1);
            args.EndLineIndex.SetFoundValues("-1", 1);

            String fileContent =
    @" line0
    <parent version=1.0.3>
        <name>money</name>
        <description>Money description</description>
    </parent>";
            File.WriteAllText(TEMP_TEST_FILE, fileContent);

            SearchAndReplace search = new SearchAndReplace(args, null);
            search.Perform();
            Assert.AreEqual(4, search.MatchCount);
        }

        [Test]
        public void FindSingleLineManyMatches()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("name>", 1);

            String fileContent =
    @"<parent version=1.0.3>
        <name>money</name>
        <description>Money description</description>
    </parent>";
            File.WriteAllText(TEMP_TEST_FILE, fileContent);

            SearchAndReplace search = new SearchAndReplace(args, null);
            search.Perform();
            Assert.AreEqual(2, search.MatchCount);
        }

        private static NSedAllowedArgs GetAllowedArgs()
        {
            var args = new NSedAllowedArgs();
            args.Initialize();
            return args;
        }

        [Test]
        public void FindNotExistingRegex()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("bundle-version(=1\\.0\\.3)", 1);
            args.Replace.SetFoundValues("version\\2", 1);
            args.Multiline.SetFoundValues(String.Empty, 1);

            String fileContent =
    @"<parent version=1.0.3>
        <name>money</name>
        <description>Money description</description>
    </parent>
";

            ReplaceInFile(args, fileContent, fileContent, true, false);
        }


        [Test]
        public void ReplaceLineRegexAndOffset()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("word(s?)", 1);
            args.StartLineRegex.SetFoundValues("^\\s*$", 1);
            args.StartLineOffset.SetFoundValues("-1", 1);
            args.EndLineIndex.SetFoundValues("-1", 1);

            args.Replace.SetFoundValues("bird\\1", 1);

            String fileContent =
    @" M$ Word
    10 words
    
    a thousand words
    Word 2007
";

            String replacedContent =
    @" M$ Word
    10 birds
    
    a thousand birds
    bird 2007
";

            ReplaceInFile(args, fileContent, replacedContent, true);

            // Case sensitive
            args.CaseSensitive.SetFoundValues(String.Empty, 1);
            ReplaceInFile(args, fileContent, replacedContent, false);
        }


        [Test]
        public void FindMultiline()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("version=.+\r\n\\s*", 1);
            args.Multiline.SetFoundValues(String.Empty, 1);
            args.CaseSensitive.SetFoundValues(String.Empty, 1);

            String fileContent = "1  \r\nversion=1.0.3\r\n   ";
            File.WriteAllText(TEMP_TEST_FILE, fileContent);

            SearchAndReplace search = new SearchAndReplace(args, null);
            search.Perform();
            Assert.AreEqual(1, search.MatchCount);
        }

        [Test]
        public void ReplaceMultipleLineBreak()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("\r\n\\s*?\r\n", 1);
            args.Replace.SetFoundValues("\r\n", 1);
            args.Multiline.SetFoundValues(String.Empty, 1);

            String fileContent =
    @" M$ Word
    10 words

    thousand of words

    Word 2007
";

            String replacedContent =
    @" M$ Word
    10 words
    thousand of words
    Word 2007
";

            ReplaceInFile(args, fileContent, replacedContent, true);
        }

        [Test]
        public void RealReplaceFeatureVersion()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("(version=)\".*?\"", 1);
            args.Replace.SetFoundValues("\\1\"1.0.4.qualifier\"", 1);
            args.StartLineRegex.SetFoundValues("^<feature", 1);
            args.EndLineRegex.SetFoundValues("^\\s*provider-name", 1);

            String fileContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine
                + "<feature" + Environment.NewLine
                + "id=\"pl.edu.agh.link.money.feature\"" + Environment.NewLine
                + "label=\"%featureName\"" + Environment.NewLine
                + "version=\"1.0.3.qualifier\"" + Environment.NewLine
                + "provider-name=\"%providerName\">" + Environment.NewLine;

            String replacedContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine
                + "<feature" + Environment.NewLine
                + "id=\"pl.edu.agh.link.money.feature\"" + Environment.NewLine
                + "label=\"%featureName\"" + Environment.NewLine
                + "version=\"1.0.4.qualifier\"" + Environment.NewLine
                + "provider-name=\"%providerName\">" + Environment.NewLine;

            ReplaceInFile(args, fileContent, replacedContent, true);
        }

        [Test]
        public void RealReplacePomVersion()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("(\\s*<version>)1.0.3-SNAPSHOT(</version>)", 1);
            args.Replace.SetFoundValues("\\11.0.4-SNAPSHOT\\2", 1);

            String fileContent = "  <parent>" + Environment.NewLine
                + "    <groupId>pl.edu.agh.link</groupId>" + Environment.NewLine
                + "    <artifactId>money-features</artifactId>" + Environment.NewLine
                + "    <version>1.0.3-SNAPSHOT</version>" + Environment.NewLine
                + "  </parent>" + Environment.NewLine;

            String replacedContent = "  <parent>" + Environment.NewLine
                + "    <groupId>pl.edu.agh.link</groupId>" + Environment.NewLine
                + "    <artifactId>money-features</artifactId>" + Environment.NewLine
                + "    <version>1.0.4-SNAPSHOT</version>" + Environment.NewLine
                + "  </parent>" + Environment.NewLine;

            ReplaceInFile(args, fileContent, replacedContent, true);
        }

        [Test]
        public void RealFindWrongIndention()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("^\\s{3,}<version>", 1);
            args.StartLineIndex.SetFoundValues("0", 1);
            args.EndLineRegex.SetFoundValues("^\\s*</parent>", 1);

            String fileContent = "\t<parent>" + Environment.NewLine
                + "\t\t<groupId>pl.edu.agh.link</groupId>" + Environment.NewLine
                + "\t\t<artifactId>money-features</artifactId>" + Environment.NewLine
                + "\t\t\t<version>1.0.3-SNAPSHOT</version>" + Environment.NewLine
                + "\t\t<packaging>jar</packaging>" + Environment.NewLine;


            File.WriteAllText(TEMP_TEST_FILE, fileContent);

            SearchAndReplace search = new SearchAndReplace(args, null);
            search.Perform();
            Assert.AreEqual(0, search.MatchCount);
        }

        [Test]
        public void ReplaceMatchCount()
        {
            NSedAllowedArgs args = GetAllowedArgs();
            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);
            args.Find.SetFoundValues("^\\s{3,}<version>", 1);
            args.Replace.SetFoundValues("^\t\t<version>", 1);
            args.StartLineIndex.SetFoundValues("0", 1);
            args.EndLineRegex.SetFoundValues("^\\s*</parent>", 1);

            String fileContent = "\t<parent>" + Environment.NewLine
                + "\t\t<groupId>pl.edu.agh.link</groupId>" + Environment.NewLine
                + "\t\t<artifactId>money-features</artifactId>" + Environment.NewLine
                + "\t\t\t<version>1.0.3-SNAPSHOT</version>" + Environment.NewLine
                + "\t\t\t<version>1.0.4-SNAPSHOT</version>" + Environment.NewLine
                + "\t\t</parent>" + Environment.NewLine;


            File.WriteAllText(TEMP_TEST_FILE, fileContent);

            SearchAndReplace search = new SearchAndReplace(args, null);
            search.DeleteFilePermanently = true;
            search.Perform();
            Assert.AreEqual(2, search.MatchCount);
        }

        private void ReplaceInFile(NSedAllowedArgs args, String fileContent, String replacedFileContent,
            bool equals, bool shouldBeReplaced = true)
        {
            File.WriteAllText(TEMP_TEST_FILE, fileContent);
            var fileInfo = new FileInfo(TEMP_TEST_FILE);
            DateTime writeTime = fileInfo.LastWriteTime;
            // sleep in order to ensure that the last write time will change
            Thread.Sleep(1);

            args.FilePath.SetFoundValues(TEMP_TEST_FILE, 1);

            SearchAndReplace search = new SearchAndReplace(args, null);
            search.DeleteFilePermanently = true;
            search.Perform();

            Assert.AreEqual(shouldBeReplaced, search.IsFileReplaced);
            fileInfo.Refresh();
            if (search.IsFileReplaced)
            {
                Assert.AreNotEqual(writeTime, fileInfo.LastWriteTime);
            }
            else
            {
                Assert.AreEqual(writeTime, fileInfo.LastWriteTime);
            }

            String actual = File.ReadAllText(TEMP_TEST_FILE);
            if (equals)
            {
                Assert.AreEqual(replacedFileContent, actual);
            }
            else
            {
                Assert.AreNotEqual(replacedFileContent, actual);
            }
        }
    }
}