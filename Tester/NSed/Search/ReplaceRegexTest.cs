using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using NSed.Search;
using System.Text.RegularExpressions;

namespace Tester.NSed.Search
{
    [TestFixture]
    public class ReplaceRegexTest
    {
        [Test]
        public void GetString1()
        {
            String line = "pl.edu.agh.cast.thirdparty;bundle-version=\"[1.0.0,1.1.0)\",";
            String findRegex = "pl.edu.agh.([^.]*).*";
            Regex regex = new Regex(findRegex);
            var m = regex.Match(line);
            ReplaceRegex replace = new ReplaceRegex(findRegex, "\\1\\r\\nlink");
            String actual = replace.GetReplaced(m, line);
            Assert.AreEqual("cast\r\nlink", actual);

            replace = new ReplaceRegex(findRegex, "\\1\\\\r\\\\nlink");
            actual = replace.GetReplaced(m, line);
            Assert.AreEqual("cast\\r\\nlink", actual);
        }

        [Test]
        public void GetString2()
        {
            String line = "pl.edu.agh.cast.thirdparty;bundle-version=\"[1.0.0,1.1.0)\",";
            String findRegx = "(cast)";
            Regex regex = new Regex(findRegx);
            var m = regex.Match(line);
            ReplaceRegex replace = new ReplaceRegex(findRegx, "link");
            String actual = replace.GetReplaced(m, line);
            Assert.AreEqual("pl.edu.agh.link.thirdparty;bundle-version=\"[1.0.0,1.1.0)\",", actual);
        }

        [Test]
        public void GetString3()
        {
            String line = "pl.edu.agh.cast.thirdparty;bundle-version=\"[1.0.0,1.1.0)\",";
            String findRegex = "(bundle-version=\"\\[)1.0.0,1.1.0(\\)\",)";
            Regex regex = new Regex(findRegex);
            var m = regex.Match(line);
            ReplaceRegex replace = new ReplaceRegex(findRegex, "\\12.0.0,3.0.0\\2");
            String actual = replace.GetReplaced(m, line);
            Assert.AreEqual("pl.edu.agh.cast.thirdparty;bundle-version=\"[2.0.0,3.0.0)\",", actual);
        }

        [Test]
        public void GetStringMulti()
        {
            String line = "10a12b13ast";
            String findRegex = "(\\d+)";
            Regex regex = new Regex(findRegex);
            var m = regex.Match(line);
            ReplaceRegex replace = new ReplaceRegex(findRegex, "--");
            String actual = replace.GetReplaced(m, line);
            Assert.AreEqual("--a--b--ast", actual);

            line = "";
            m = regex.Match(line);
            actual = replace.GetReplaced(m, "");
            Assert.AreEqual("", actual);
        }

        [Test]
        public void GetStringMultiline()
        {
            String line = "\r\nversion=1\r\nversion=2\r\nversion=3";
            String findRegex = "^(version=)(\\d)";
            Regex regex = new Regex(findRegex, RegexOptions.Multiline);
            var m = regex.Match(line);

            ReplaceRegex replace = new ReplaceRegex(findRegex, "\\1\\2.0");
            String actual = replace.GetReplaced(m, line);
            Assert.AreEqual("\r\nversion=1.0\r\nversion=2.0\r\nversion=3.0", actual);
        }

        [Test]
        public void GetStringMultiline2()
        {
            String line = "\r\nversion=1\r\n   \r\n   version=2";
            String findRegex = "\\s+^\\s*$\\s+";
            Regex regex = new Regex(findRegex, RegexOptions.Multiline);
            var m = regex.Match(line);

            ReplaceRegex replace = new ReplaceRegex(findRegex, "\r\n");
            String actual = replace.GetReplaced(m, line);
            Assert.AreEqual("\r\nversion=1\r\nversion=2", actual);
        }

        [Test]
        public void MultilineRegex()
        {
            String line = "10a12b13ast \r\nversion=1\\.0";
            String findRegex = "^version";
            Regex regex = new Regex(findRegex, RegexOptions.Multiline);
            var m = regex.Match(line);
            Assert.IsTrue(m.Success);

            regex = new Regex(findRegex);
            m = regex.Match(line);
            // SingleLine mode is by default
            Assert.IsFalse(m.Success);
        }

        [Test]
        public void GetStringNoMatch()
        {
            String line = "10a12b13ast";
            String findRegex = "(\\d{10,})";
            Regex regex = new Regex(findRegex);
            var m = regex.Match(line);
            ReplaceRegex replace = new ReplaceRegex(findRegex, "--");
            String actual = replace.GetReplaced(m, line);
            Assert.AreEqual(line, actual);
        }

        [Test]
        public void GetGroupCount()
        {
            int actual = ReplaceRegex.GetGroupCount("");
            Assert.AreEqual(0, actual);

            actual = ReplaceRegex.GetGroupCount("()");
            Assert.AreEqual(1, actual);

            actual = ReplaceRegex.GetGroupCount("(.(x))");
            Assert.AreEqual(2, actual);

            actual = ReplaceRegex.GetGroupCount("\\(.(x)\\)");
            Assert.AreEqual(1, actual);

            actual = ReplaceRegex.GetGroupCount("\\\\(.(x)\\\\)");
            Assert.AreEqual(2, actual);

            Assert.Throws(typeof(ArgumentException), () => { ReplaceRegex.GetGroupCount("(.(x)"); });
            Assert.Throws(typeof(ArgumentException), () => { ReplaceRegex.GetGroupCount("(.(x)))"); });
        }
        [Test]
        public void GroupCount()
        {
            Regex regex = new Regex("(a)?(b)");
            Match m = regex.Match("ab");
            Console.WriteLine("Group count: {0}, g1: {1}, g2: {2}", m.Groups.Count, m.Groups[1].Value, m.Groups[2].Value);
            Assert.AreEqual(3, m.Groups.Count);

            m = regex.Match("b");
            Console.WriteLine("Group count: {0}, g1: {1}, g2: {2}", m.Groups.Count, m.Groups[1].Value, m.Groups[2].Value);
            Assert.AreEqual(3, m.Groups.Count);
        }
    }
}