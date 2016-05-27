using ArgumentHelper.Arguments.General.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ArgumentHelper.Arguments.Appender;

namespace ArgumentHelper.Arguments.General
{
    public abstract class AbstractArgumentsChecker
    {
        protected string[] args;

        private Dictionary<String, int> countMap = new Dictionary<string, int>();

        private ILogProvider log;

        private ITextAppender textAppender;

        public AbstractArgumentsChecker(string[] args, IAllowedArgs allowedArgs, ILogProvider log)
        {
            this.args = args;
            this.AllowedArgs = allowedArgs;
            this.log = log;
            if (this.log == null)
                this.log = new ConsoleLogProvider();
            allowedArgs.Initialize();
        }

        public virtual void PerformCheck()
        {
            try
            {
                int len = args.Length;
                int i = 0;
                List<Argument> argsWithGroups = new List<Argument>();
                Argument restrictionArg = null;
                while (i < len)
                {
                    String argName = args[i];
                    if (argName.Equals(";"))
                    {
                        restrictionArg = null;
                    }
                    else if (restrictionArg != null && (argName.Equals("(") || argName.Equals(")")))
                    {
                        restrictionArg.AddGroupArg(argName, null);
                    }
                    else
                    {
                        if (!AllowedArgs.IsArgAllowed(argName))
                        {
                            throw new ArgumentValidationException("argument '" + argName + "' is not supported");
                        }
                        if (restrictionArg != null && !IsArgAllowed(restrictionArg.AllowedArgs, argName))
                        {
                            throw new ArgumentValidationException(String.Format("argument '{0}' is not allowed after argument(s) '{1}'", argName, restrictionArg.NameString));
                        }
                        Argument argObj = AllowedArgs.GetArgument(argName);
                        if (argObj.AllowedAfter != null && (restrictionArg == null || !argObj.AllowedAfter.Equals(restrictionArg)))
                        {
                            throw new ArgumentValidationException(String.Format("argument '{0}' is allowed only after argument(s) '{1}'", argName, argObj.AllowedAfter.NameString));
                        }
                        IncrementCountMap(argObj, argName);
                        if (argObj.Count != -1 && GetArgCount(argObj) > argObj.Count)
                        {
                            throw new ArgumentValidationException("count of argument '" + argName + "' is invalid, allowed: " + argObj.Count);
                        }
                        if (argObj.HasValue)
                        {
                            if (i + 1 >= len)
                            {
                                throw new ArgumentValidationException("count of argument '" + argName + "' has to have a value");
                            }
                            else
                            {
                                argObj.StringValue = args[++i];
                            }
                        }
                        if (restrictionArg != null)
                        {
                            if (restrictionArg.IsOption(argObj.Names))
                            {
                                if (restrictionArg.ArgGroupCount > 0)
                                {
                                    throw new ArgumentValidationException(String.Format("option '{0}' can be specified only just after argument(s) '{1}'", argName, restrictionArg.NameString));
                                }
                            }
                            else
                            {
                                restrictionArg.AddGroupArg(argName, argObj.StringValue);
                            }
                        }
                        if (argObj.AllowedArgs != null && argObj.AllowedArgs.Count > 0)
                        {
                            restrictionArg = argObj;
                            argsWithGroups.Add(restrictionArg);
                        }
                    }
                    i++;
                }
                CheckMandatory();
                // To finish validation trees are to be built
                foreach (Argument arg in argsWithGroups)
                {
                    arg.BuildTree();
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                log.Error("Argument checking error", ex);
                throw;
            }
        }

        private bool IsArgAllowed(IList<Argument> argList, string argName)
        {
            foreach (Argument arg in argList)
            {
                if (arg.Names.Contains(argName))
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckMandatory()
        {
            foreach (Argument argObj in AllowedArgs.GetAllowedMap().Values)
            {
                if (argObj.Mandatory && GetArgCount(argObj.Names) == 0)
                {
                    throw new ArgumentValidationException("argument '" + argObj.Names.First() + "' is mandatory");
                }
            }
        }

        private void IncrementCountMap(Argument argObj, String argName)
        {
            argObj.CheckNameValid(argName);
            if (countMap.ContainsKey(argName))
            {
                countMap[argName] = countMap[argName] + 1;
            }
            else
            {
                countMap[argName] = 1;
            }
            argObj.FoundCount = GetArgCount(argObj.Names);
        }

        public IAllowedArgs AllowedArgs { get; private set; }

        public int GetArgCount(Argument arg)
        {
            return GetArgCount(arg.Names);
        }

        public int GetArgCount(IEnumerable<String> names)
        {
            int count = 0;
            foreach (var name in names)
                count += GetArgCount(name);
            return count;
        }
        public int GetArgCount(String name)
        {
            if (countMap.ContainsKey(name))
            {
                return countMap[name];
            }
            else
            {
                return 0;
            }
        }

        public void EnsureArgumentSet(String messageText, params Argument[] arguments)
        {
            var q = arguments.SelectMany(a => a.Names);
            foreach (var entry in countMap)
            {
                if (entry.Value > 0 && q.Where(name => name == entry.Key).FirstOrDefault() == null)
                {
                    throw new ArgumentValidationException(String.Format("{0}: {1}", messageText, entry.Key));
                }
            }
        }

        /// <summary>
        /// Process the arguments.
        /// </summary>
        /// <param name="allowed">the object with allowed arguemnts</param>
        /// <returns>whether the program can continues. When false continuation is not allowed</returns>
        public bool PerformAllChecks()
        {
            return PerformAllChecksWResult() == ArgCheckResult.Continue;
        }
        public ArgCheckResult PerformAllChecksWResult()
        {
            if (this.PrintHelp())
            {
                log.Info(AllowedArgs.GetUsage(TextAppender));
                return ArgCheckResult.Stop;
            }
            try
            {
                this.PerformCheck();
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                String message = GetInvalidArgumentMessage(ex);
                TextAppender.AppendFormattedText(sb, message, 0, 40);
                if (IsHelpDisplayAllowed && PrintUsageOnError)
                {
                    sb.Append(String.Format("{0}{1}{0}{0}", Environment.NewLine, AllowedArgs.GetUsage(TextAppender)));
                }
                if (IsValidationException(ex))
                {
                    AppendHelpInfo(sb);
                    log.Error(sb.ToString());
                }
                else
                    log.Debug(sb.ToString(), ex);
                return ArgCheckResult.ErrorFound;
            }
            return ArgCheckResult.Continue;
        }

        private String GetInvalidArgumentMessage(Exception ex)
        {
            return String.Format("Argument validation error: {0}.", ex.Message);
        }

        private void AppendHelpInfo(StringBuilder builder)
        {
            String helpSwitch = AllowedArgs.HelpSwitch;
            if (helpSwitch != null)
                TextAppender.AppendFormattedText(builder, String.Format("For help specify {0} switch.", helpSwitch), 0, 40);
        }

        private bool IsValidationException(Exception ex)
        {
            return ex is ArgumentValidationException;
        }

        protected bool PrintHelp()
        {
            int diff = HasRawHelpTextArgument() ? 1 : 0;
            if (args.Length - diff == 1)
            {
                return HasHelpArgument();
            }
            return false;
        }

        private bool HasHelpArgument()
        {
            return HasArgumentOfType(typeof(HelpArgument));
        }

        private bool HasRawHelpTextArgument()
        {
            return HasArgumentOfType(typeof(RawHelpArgument));
        }

        private bool HasArgumentOfType(Type type)
        {
            return AllowedArgs.GetAllowedMap().Values.Where(argument =>
                {
                    if (argument.GetType().Equals(type))
                    {
                        return args.Where(arg => argument.Names.Contains(arg)).Count() > 0;
                    }
                    return false;
                }).Count() > 0;
        }

        protected ITextAppender TextAppender
        {
            get
            {
                if (textAppender == null)
                {
                    if (HasRawHelpTextArgument())
                    {
                        textAppender = new RawAppender();
                    }
                    else
                    {
                        textAppender = new ConsoleAppender();
                    }
                }
                return textAppender;
            }
        }

        protected virtual bool IsHelpDisplayAllowed
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Indicates whether to print help when an error occurs.
        /// </summary>
        public bool PrintUsageOnError { get; set; }

    }
}
