using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using ArgumentHelper.Arguments.Appender;

namespace ArgumentHelper.Arguments.General
{
    public abstract class AbstractAllowedArgs : IAllowedArgs
    {

        public abstract IReadOnlyDictionary<String, Argument> GetAllowedMap();

        private int minDescriptionLineLen = ConsoleAppender.CONSOLE_LINE_LEN - 40;

        private Assembly mainAssembly;

        public AbstractAllowedArgs(Assembly mainAssembly)
        {
            if (mainAssembly == null) throw new ArgumentNullException();
            this.mainAssembly = mainAssembly;
        }

        public String GetUsage(ITextAppender appender)
        {
            String appNameWExt = System.IO.Path.GetFileName(mainAssembly.Location);
            String name = mainAssembly.GetName().Name;
            String appName = name;
            String version = mainAssembly.GetName().Version.ToString();
            String description = GetAssemblyDescription(mainAssembly);

            StringBuilder sb = new StringBuilder();
            sb.Append("Name:");
            sb.Append(Environment.NewLine);
            sb.Append(String.Format(" {0} {1}", name, version));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Synopsis:");
            sb.Append(Environment.NewLine);
            sb.Append(String.Format(" {0} <options>", appName));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            if (!String.IsNullOrWhiteSpace(description))
            {
                sb.Append("Description:");
                sb.Append(Environment.NewLine);
                sb.Append(" ");
                appender.AppendFormattedText(sb, description, 1, minDescriptionLineLen);
                sb.Append(Environment.NewLine);
            }
            sb.Append("Options:");
            sb.Append(Environment.NewLine);
            GetOptionsString(appender, sb);
            IEnumerable<ExampleGroup> examples = GetExamples(appName);
            if (examples.Count() > 0)
            {
                sb.Append(Environment.NewLine);
                sb.Append("Examples:");
                sb.Append(Environment.NewLine);
                String examplePrefix = " ";
                int groupIndex = 1;
                int nextLinesIndent = 3;
                if (examples.Count() == 1 && String.IsNullOrWhiteSpace(examples.First().Name))
                {
                    AppendGroupExamples(appender, sb, examples.First(), examplePrefix, nextLinesIndent, 1);
                }
                else
                {
                    foreach (var exampleGroup in examples)
                    {
                        AppendExampleGroup(appender, sb, examplePrefix, groupIndex, nextLinesIndent, exampleGroup);
                        groupIndex++;
                    }
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString().TrimEnd();
        }

        private void AppendExampleGroup(ITextAppender appender, StringBuilder builder, String examplePrefix, int groupIndex, int nextLinesIndent, ExampleGroup exampleGroup)
        {
            builder.Append(Environment.NewLine);
            builder.Append(examplePrefix);
            String rawName;
            if (String.IsNullOrWhiteSpace(exampleGroup.Name))
            {
                rawName = String.Format("Example set no. {0}", groupIndex);
            }
            else
            {
                rawName = exampleGroup.Name;
            }
            String grupName = String.Format(" * {0}", rawName);
            appender.AppendFormattedText(builder, grupName, examplePrefix.Length, minDescriptionLineLen, nextLinesIndent);
            AppendGroupExamples(appender, builder, exampleGroup, examplePrefix, nextLinesIndent, 2);
        }

        private void AppendGroupExamples(ITextAppender appender, StringBuilder builder, ExampleGroup exampleGroup, String examplePrefix, int nextLinesIndent, int level)
        {
            int eindex = 1;
            int count = exampleGroup.Examples.Length;
            if (count == 1)
            {
                AppendString(builder, examplePrefix, level);
                String text = exampleGroup.Examples.First();
                appender.AppendFormattedText(builder, text, examplePrefix.Length * level, minDescriptionLineLen, 0);
            }
            else
            {
                foreach (var example in exampleGroup.Examples)
                {
                    AppendString(builder, examplePrefix, level);
                    String text = String.Format(" {0}. {1}", eindex, example);
                    appender.AppendFormattedText(builder, text, examplePrefix.Length * level, minDescriptionLineLen, nextLinesIndent);
                    eindex++;
                }
            }
        }

        private static void AppendString(StringBuilder builder, String str, int count)
        {
            for (int i = 0; i < count; i++)
            {
                builder.Append(str);
            }
        }

        private string GetAssemblyDescription(Assembly mainAssembly)
        {
            //Type of attribute that is desired
            Type type = typeof(AssemblyDescriptionAttribute);
            //Is there an attribute of this type already defined?
            if (AssemblyDescriptionAttribute.IsDefined(mainAssembly, type))
            {
                //if there is, get attribute of desired type
                AssemblyDescriptionAttribute a = (AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(mainAssembly, type);
                return a.Description;
            }
            return null;
        }

        private void GetOptionsString(ITextAppender appender, StringBuilder builder)
        {
            IEnumerable<Argument> allowed = GetOrderedArguments();
            int maxNameLen = 0;
            foreach (Argument argObj in allowed)
            {
                foreach (var usageName in argObj.UsageNames)
                {
                    maxNameLen = Math.Max(maxNameLen, usageName.TrimEnd().Length);
                }
            }
            String nameSeparator = " - ";
            foreach (Argument argObj in allowed)
            {
                var usageNameList = argObj.UsageNames;
                int usageIndex = 0;
                foreach (var usageName in usageNameList)
                {
                    if (usageIndex >= usageNameList.Count - 1)
                        break;
                    builder.Append(ConsoleAppender.CreateFullLine(usageName.TrimEnd()));
                    usageIndex++;
                }
                String name = usageNameList.Last().TrimEnd();
                builder.Append(ConsoleAppender.PadLineRight(name, maxNameLen));
                builder.Append(nameSeparator);
                int maxLen = maxNameLen + nameSeparator.Length;
                appender.AppendFormattedText(builder, argObj.UsageDescription, maxLen, minDescriptionLineLen);
            }
        }

        /// <summary>
        /// Returns list of arguments in the addition order.
        /// </summary>
        /// <returns>the list of arguments</returns>
        protected abstract IReadOnlyList<Argument> GetOrderedArguments();

        private IEnumerable<Argument> GetSortedArguments()
        {
            var list = GetAllowedMap().Values.ToList();
            list.Sort(CompareArguments);
            return list;
        }

        internal int CompareArguments(Argument a, Argument b)
        {
            if (a is HelpArgument && b is HelpArgument)
            {
                return a.Names.First().CompareTo(b.Names.First());
            }
            else if (a is HelpArgument)
            {
                return -1;
            }
            else if (b is HelpArgument)
            {
                return 1;
            }
            return a.Names.First().CompareTo(b.Names.First());
        }

        public String GetOptionsString()
        {
            StringBuilder sb = new StringBuilder();
            GetOptionsString(new ConsoleAppender(), sb);
            return sb.ToString();
        }

        public abstract IEnumerable<ExampleGroup> GetExamples(String appName);

        public abstract bool IsArgAllowed(string name);

        public abstract Argument GetArgument(string name);

        /// <summary>
        /// Gets or sets minimum line length when usage desription is fomatted.
        /// </summary>
        public int MinDescriptionLineLen
        {
            get
            {
                return minDescriptionLineLen;
            }
            set
            {
                minDescriptionLineLen = value;
            }

        }

        public String HelpSwitch
        {
            get
            {
                var help = GetAllowedMap().Values.Where(a => a is HelpArgument).FirstOrDefault();
                if (help != null)
                    return help.Names.First();
                return null;
            }
        }


        public abstract void Initialize();
    }
}
