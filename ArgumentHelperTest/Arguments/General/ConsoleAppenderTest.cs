using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArgumentHelper.Arguments.General;
using ArgumentHelper.Arguments.General.Attributes;
using System.Text;
using System.Collections.Generic;
using ArgumentHelper.Arguments.Appender;


namespace ArgumentHelperTest.Arguments.General
{
    [TestClass]
    public class ConsoleAppenderTest
    {
        class DummyAllowedArgsTest : AbstractAllowedArgsBase
        {
            public DummyAllowedArgsTest()
                : base(System.Reflection.Assembly.GetExecutingAssembly())
            {

            }

            protected override void ArgumentsCreated()
            {
                UnsetIndent();
            }
            public override IEnumerable<ExampleGroup> GetExamples(String appName)
            {
                return new ExampleGroup[] { new ExampleGroup(new string[] { String.Format("{0} -help", appName) }) };
            }

            protected override IEnumerable<HelpArgument> GetHelpArguments()
            {
                return new HelpArgument[0];
            }

            protected override IEnumerable<RawHelpArgument> GetRawHelpArguments()
            {
                return new RawHelpArgument[0];
            }

            protected override void InstantiateArguments()
            {
                SkipNullArguments = true;
                Arg1 = Arg1ToUse;
                Arg2 = Arg2ToUse;
            }

            public static Argument Arg1ToUse { get; set; }

            public static Argument Arg2ToUse { get; set; }

            public Argument Arg1 { get; private set; }

            public Argument Arg2 { get; private set; }

            internal void UnsetIndent()
            {
                foreach (var arg in GetAllowedMap().Values)
                {
                    arg.Indent = false;
                }
            }
        }


        [TestMethod]
        public void InvalidCharacterCaretReturn()
        {
            // An exception should not be thrown
            StringBuilder sb = new StringBuilder();
            new ConsoleAppender().AppendFormattedText(sb, "Displays this help \rDisplays this help", 5, 40);
        }

        [TestMethod]
        public void InvalidCharacterNewLine()
        {
            // An exception should not be thrown
            StringBuilder sb = new StringBuilder();
            new ConsoleAppender().AppendFormattedText(sb, "Displays this help \nDisplays this help", 5, 40);
        }

        [TestMethod]
        public void AppendFormattedTextIndention()
        {
            StringBuilder sb = new StringBuilder();
            String name = "".PadLeft(40);
            sb.Append(name);
            new ConsoleAppender().AppendFormattedText(sb, "Displays this help Displays this help Displays this help Displays this help Displays this help", name.Length, 39, 3);
            String[] actualLines = GetUsageLines(sb.ToString());
            String[] expected = new String[] { name + "Displays this help Displays this help   ",
                "                                           Displays this help Displays this     ",
                "                                           help Displays this help              "};
            expected = CorrectExpectedLines(expected);
            CollectionAssert.AreEqual(expected, actualLines);
        }

        [TestMethod]
        public void GetValidSubstring()
        {
            int substringLen;
            String str = ConsoleAppender.GetValidSubstring("Displays this help Displays this help", 0, 5, out substringLen);
            Assert.AreEqual("Displ", str);

            str = ConsoleAppender.GetValidSubstring("Displays this help Displays this help", 0, 15, out substringLen);
            Assert.AreEqual("Displays this  ", str);

            str = ConsoleAppender.GetValidSubstring("Displays thishelp Displays this help", 0, 15, out substringLen);
            Assert.AreEqual("Displays       ", str);

            str = ConsoleAppender.GetValidSubstring("Displays thishelp Displays this help", 0, 15, out substringLen);
            Assert.AreEqual("Displays       ", str);

            str = ConsoleAppender.GetValidSubstring("Displays this help Displays this help", 0, 37, out substringLen);
            Assert.AreEqual("Displays this help Displays this help", str);

            str = ConsoleAppender.GetValidSubstring("Displays this help Displays this help", 9, 12, out substringLen);
            Assert.AreEqual("this help   ", str);

            str = ConsoleAppender.GetValidSubstring("Displays this help\r\n Displays this help", 0, 35, out substringLen);
            Assert.AreEqual("Displays this help".PadRight(35), str);

            str = ConsoleAppender.GetValidSubstring("Displays this help\r\n Displays this help", substringLen, 20, out substringLen);
            Assert.AreEqual(" Displays this help ", str);
        }

        [TestMethod]
        public void GetUsage()
        {
            DummyAllowedArgsTest.Arg1ToUse = new Argument("-hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh",
                    "Displays this help Displays this help Displays this Displays this help",
                    false, 1, false);
            DummyAllowedArgsTest.Arg2ToUse = null;
            DummyAllowedArgsTest dummy = GetAllowedArgs();
            String[] actualLines = GetUsageLines(dummy.GetOptionsString());
            String[] expected = new String[]{
                "-hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh - Displays this     ",
                "                                        help Displays this help Displays this   ",
                "                                        Displays this help (mandatory: no,      ",
                "                                        allowed count: 1)                       ",
            };
            expected = CorrectExpectedLines(expected);
            CollectionAssert.AreEqual(expected, actualLines);

            DummyAllowedArgsTest.Arg1ToUse = new Argument(new String[] { "-h", "-hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh" },
                    "Displays this help Displays this help Displays this Displays this help",
                    false, 1, false);
            DummyAllowedArgsTest.Arg2ToUse = null;
            dummy = GetAllowedArgs();
            actualLines = GetUsageLines(dummy.GetOptionsString());
            expected = new String[]{
                "-h,",
                "-hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh - Displays this     ",
                "                                        help Displays this help Displays this   ",
                "                                        Displays this help (mandatory: no,      ",
                "                                        allowed count: 1)                       ",
            };
            expected = CorrectExpectedLines(expected);
            CollectionAssert.AreEqual(expected, actualLines);

            DummyAllowedArgsTest.Arg1ToUse = new Argument(GetFullLine("-hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh", 'h'),
                    "Displays this help Displays this help Displays this Displays this help",
                    false, 1, false);
            DummyAllowedArgsTest.Arg2ToUse = new Argument("-help",
                    "Displays this help Displays this help Displays this Displays this help",
                    false, 1, false);
            dummy = GetAllowedArgs();
            actualLines = GetUsageLines(dummy.GetOptionsString());
            expected = new String[]{
                GetFullLine("-hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh", 'h'),
                " - Displays this help Displays this help Displays this Displays this help       ",
                "   (mandatory: no, allowed count: 1)                                            ",
                "-help                                                                           ",
                " - Displays this help Displays this help Displays this Displays this help       ",
                "   (mandatory: no, allowed count: 1)                                            "
            };
            expected = CorrectExpectedLines(expected);
            CollectionAssert.AreEqual(expected, actualLines);

            DummyAllowedArgsTest.Arg1ToUse = new Argument(GetFullLine("-hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh", 'h') + "-help",
                    "Displays this help Displays this help Displays this Displays this help",
                    false, 1, false);
            dummy = GetAllowedArgs();
            actualLines = GetUsageLines(dummy.GetOptionsString());
            expected = new String[]{
                   GetFullLine("-hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh", 'h'),
                   "-help - Displays this help Displays this help Displays this Displays this help  ",
                   "        (mandatory: no, allowed count: 1)                                       ",
                   "-help                                                                           ",
                   "      - Displays this help Displays this help Displays this Displays this help  ",
                   "        (mandatory: no, allowed count: 1)                                       "
            };
            expected = CorrectExpectedLines(expected);
            CollectionAssert.AreEqual(expected, actualLines);

            DummyAllowedArgsTest.Arg1ToUse = new Argument(GetFullLine("-hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh", 'h') + "-help",
                    "Displays this help \r\nDisplays this help Displays this\n Displays this help",
                    false, 1, false);
            dummy = GetAllowedArgs();
            actualLines = GetUsageLines(dummy.GetOptionsString());
            expected = new String[]{
                   GetFullLine("-hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh", 'h'),
                   "-help - Displays this help                                                      ",
                   "        Displays this help Displays this                                        ",
                   "        Displays this help (mandatory: no, allowed count: 1)                    ",
                   "-help                                                                           ",
                   "      - Displays this help Displays this help Displays this Displays this help  ",
                   "        (mandatory: no, allowed count: 1)                                       "
            };
            expected = CorrectExpectedLines(expected);
            CollectionAssert.AreEqual(expected, actualLines);
        }

        private string GetFullLine(string line, char ch)
        {
            return line.PadRight(ConsoleAppender.CONSOLE_LINE_LEN, ch);
        }

        private string[] CorrectExpectedLines(string[] expected)
        {
            if (!ConsoleAppender.PAD_LINES)
            {
                return expected.Select(l => l.TrimEnd()).ToArray();
            }
            return expected;
        }

        private DummyAllowedArgsTest GetAllowedArgs()
        {
            var args = new DummyAllowedArgsTest();
            args.Initialize();
            return args;
        }

        private string[] GetUsageLines(string usage)
        {
            var lines = usage.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            List<String> lineList = new List<string>();
            foreach (var line in lines)
            {
                lineList.AddRange(GetLines(line));
            }
            return lineList.ToArray();

        }
        private string[] GetLines(string usage)
        {
            int lineLen = ConsoleAppender.CONSOLE_LINE_LEN;
            List<String> lineList = new List<string>();
            int processedCount = 0;
            int toPrcess = usage.Length;
            int startPos = 0;
            while (processedCount < toPrcess)
            {
                int len = Math.Min(lineLen, toPrcess - startPos);
                String part = usage.Substring(startPos, len);
                lineList.Add(part);
                startPos += len;
                processedCount += part.Length;
            }
            return lineList.ToArray();
        }
    }
}
