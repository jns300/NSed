using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArgumentHelper.Arguments.General;
using System.Reflection;

namespace NSed.Arguments.NSed
{
    public class NSedAllowedArgs : AbstractAllowedArgsBase
    {

        public NSedAllowedArgs()
            :base(Assembly.GetExecutingAssembly())
        {
        }
        protected override void InstantiateArguments()
        {
            FilePath = new Argument("-f", "File to process", true, 1, false);
            Find = new Argument("-find", "Find regular expression", true, 1, true);
            Replace = new Argument("-replace", "Replace regular expression", true, 1, false);
            Multiline = new Argument("-ml", "Multi-line regex", false, 1, false);
            CaseSensitive = new Argument("-case", "Case sensitive", false, 1, false);
            StartLineIndex = new Argument("-sli", "Start line index (inclusive, zero based), first line: 0, last line: -1, when negative counting starts from the end", true, 1, false);
            EndLineIndex = new Argument("-eli", "End line index (inclusive, zero based), first line: 0, last line: -1, when negative counting starts from the end", true, 1, false);
            StartLineRegex = new Argument("-slr", "Start line regex (inclusive)", true, 1, false);
            EndLineRegex = new Argument("-elr", "End line regex (inclusive)", true, 1, false);
            StartLineOffset = new Argument("-slo", "Start line offset (0 - none, 1 - next line, -1 - previous line)", true, 1, false);
            EndLineOffset = new Argument("-elo", "End line offset (0 - none, 1 - next line, -1 - previous line)", true, 1, false);
            UseLastLine = new Argument("-ul", "Use last line when end line index for regex is not found", false, 1, false);
            FileEncoding = new Argument("-enc", "File encoding", true, 1, false);
            DeletePermanently = new Argument("-del", "Deletes an old file when content is replaced - not recommended", false, 1, false);

            Scan = new Argument("-scan", "Directory to scan", true, 1, false);
            Name = new Argument("-name", "File name", true, -1, false, Scan);
            Regex = new Argument("-regex", "Regeular expression for file name", true, -1, false, Scan);
            PathRegex = new Argument("-path-regex", "Regeular expression for file path", true, -1, false, Scan);
            Mindepth = new Argument("-mindepth", "Minimum depth of files to return", true, 1, false, Scan);
            Maxdepth = new Argument("-maxdepth", "Maximum depth of files to return", true, 1, false, Scan);
            MMin = new Argument("-mmin", "File's data was last modified n minutes ago. (+n - for greater than n, -n - for less than n, n - for exactly n.)", true, -1, false, Scan);
            MTime = new Argument("-mtime", "File's  data was last modified n*24 hours ago. (+n - for greater than n, -n - for less than n, n - for exactly n.)", true, -1, false, Scan);
            And = new Argument("-and", "And operator", false, -1, false, Scan);
            Or = new Argument("-or", "Or operator", false, -1, false, Scan);
            Not = new Argument("-not", "Not operator", false, -1, false, Scan);
            Scan.AllowedArgs = new Argument[] { Name, PathRegex, Regex, Mindepth, Maxdepth, MMin, MTime, And, Or, Not };
            Scan.Options = new Argument[] { Mindepth, Maxdepth };

        }

        public Argument FilePath { get; private set; }

        public Argument Find { get; private set; }

        public Argument Replace { get; private set; }

        public Argument Multiline { get; private set; }

        public Argument CaseSensitive { get; private set; }

        public Argument StartLineIndex { get; private set; }

        public Argument EndLineIndex { get; private set; }

        public Argument StartLineRegex { get; private set; }

        public Argument EndLineRegex { get; private set; }

        public Argument Scan { get; private set; }

        public Argument UseLastLine { get; private set; }

        public Argument StartLineOffset { get; private set; }

        public Argument EndLineOffset { get; private set; }

        public Argument FileEncoding { get; private set; }

        public Argument DeletePermanently { get; private set; }

        public Argument Mindepth { get; private set; }

        public Argument Maxdepth { get; private set; }

        public Argument Name { get; private set; }

        public Argument PathRegex { get; private set; }

        public Argument Regex { get; private set; }

        public Argument MMin { get; private set; }

        public Argument MTime { get; private set; }

        public Argument And { get; private set; }

        public Argument Or { get; private set; }

        public Argument Not { get; private set; }

        public override IEnumerable<ExampleGroup> GetExamples(String appName)
        {
            List<ExampleGroup> groupList = new List<ExampleGroup>();
            List<String> list = new List<string>();
            list.Add(String.Format("{0} -f file.txt -find version=.+\n - finds string \"version=xxx\", search is case-insensitive and single lines are examined.", appName));
            list.Add(String.Format("{0} -f file.txt -find (^\\s+version)=(1.0.0) -replace \\1=1.0.3\n - finds string with version and replaces with new one.", appName));
            list.Add(String.Format("{0} -f file.txt -find \\s+ -sli 0 -eli 5\n - finds at least white space between lines 0-5 (lines 0 and 5 - inclusive).", appName));
            list.Add(String.Format("{0} -f file.txt -find \\s+ -slr version=1\\.0\\.3 -slo 1 -eli -1\n - finds at least one white space between line with version +1 and the last line.", appName));
            list.Add(String.Format("{0} -f file.txt -find word(s?) -slr ^\\s*$ -slo -1 -eli -1 -replace bird\\1\n - finds \"word\" or \"words\" string between line with space -1 and the last line. Then replaces \"word\" with \"bird\".", appName));
            list.Add(String.Format("{0} -f file.txt -find version=.+\\r\\n\\s* -ml -case\n - finds string \"version=xxx\" and white spaces in next line, search is case-sensitive and multiline.", appName));
            groupList.Add(new ExampleGroup("General examples:", list.ToArray()));

            list = new List<string>();
            list.Add(String.Format("{0} -find version=.+\\r\\n\\s* -scan x:\\dir -name *.txt\n - finds string \"version=xxx\" in all files in directory x:\\dir and in its all sub directories.", appName));
            list.Add(String.Format("{0} -scan x:\\dir -name *.txt ; -find version=.+\\r\\n\\s*\n - the same like above but at the beginning scanning is configured.", appName));
            list.Add(String.Format("{0} -find \"text to find\" -scan x:\\dir -mindepth 2 -maxdepth 3 ( -name *.htm -or -name *.txt ) -and -mtime +1\n - " +
                "finds string \"text to find\" in all htm and txt files in directory x:\\dir in the depth in the rage [2, 3]. The provided files were modified 1 day ago or more.", appName));
            list.Add(String.Format("{0} -find \"text to find\" -scan x:\\dir -name *.txt -and -mmin -50\n - " +
                "finds string \"text to find\" in all txt files in directory x:\\dir which were modified 50 minutes ago or less.", appName));
            list.Add(String.Format("{0} -find \"text to find\" -scan x:\\dir -name *.htm -and -not -path-regex \"_pliki/[^/]*.*$\"\n - " + 
                "finds string \"text to find\" in all htm files in directory x:\\dir. Folders whose names ends with '_pliki' are skipped.", appName));
            list.Add(String.Format("{0} -find \"text to find\" -scan x:\\dir -name *.htm ; -replace replaced\n - " + 
                "finds string \"text to find\" in all htm files in directory x:\\dir and replaces this string to \"replaced\".", appName));
            groupList.Add(new ExampleGroup("Directory scanning examples:", list.ToArray()));

            list = new List<string>();
            list.Add("Regex should be specified in double quotes. When it contains caret (^) without quotes then this char is not added to arguments.");
            list.Add("Min depth 1 means that the current directory is considered.");
            list.Add("Regular expression from argument -path-regex is examined on entire relative path (e.g. ./a/1.txt). Matching differs from Unix find where entire patch (from beginning to end) must match.");
            groupList.Add(new ExampleGroup("Notices:", list.ToArray()));

            return groupList;
        }

    }
}
