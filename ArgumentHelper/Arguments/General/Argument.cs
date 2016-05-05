using ArgumentHelper.Arguments.General.Operators;
using ArgumentHelper.Arguments.FileFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgumentHelper.Arguments.General
{
    public class Argument
    {
        public class RawArgData
        {
            public String Name { get; private set; }
            public String Value { get; private set; }

            public RawArgData(string name, string value)
            {
                Name = name;
                Value = value;
            }

        }

        public IReadOnlyCollection<String> Names { get; private set; }
        public string Description { get; private set; }
        public bool HasValue { get; private set; }
        public int Count { get; private set; }
        public bool Mandatory { get; private set; }
        public String StringValue { get; set; }
        public int FoundCount { get; set; }
        public IList<Argument> AllowedArgs { get; set; }
        public IList<Argument> Options { get; set; }
        public Argument AllowedAfter { get; set; }

        private List<RawArgData> argGroup = new List<RawArgData>();

        private IArgTreeItem filterTree;

        public Argument(string name, string description, bool hasValue, int count, bool mandatory)
            : this(new String[] { name }, description, hasValue, count, mandatory, null)
        {
        }
        public Argument(string[] names, string description, bool hasValue, int count, bool mandatory)
            : this(names, description, hasValue, count, mandatory, null)
        {
        }
        public Argument(string name, string description, bool hasValue, int count, bool mandatory, Argument allowedAfter)
            : this(new String[] { name }, description, hasValue, count, mandatory, allowedAfter)
        {
        }

        public Argument(string[] names, string description, bool hasValue, int count, bool mandatory, Argument allowedAfter)
        {
            if (names == null) throw new ArgumentNullException();
            if (names.Length == 0) throw new ArgumentException();
            this.Names = names.ToList();
            this.Description = description;
            this.HasValue = hasValue;
            this.Count = count;
            this.Mandatory = mandatory;
            this.AllowedAfter = allowedAfter;
            Indent = true;
        }

        public List<String> UsageNames
        {
            get
            {
                List<String> list = new List<string>(Names.Count);
                int index = 0;
                int count = Names.Count;
                foreach (var name in Names)
                {
                    String usage = GetUsageName(name);
                    if (count > 1 && index < count - 1)
                        usage += ",";
                    list.Add(usage);
                    index++;
                }
                return list;
            }
        }

        private String GetUsageName(String name)
        {
            String msg;
            if (HasValue)
            {
                msg = String.Format("{0}{1} <value>", IndentString, name);
            }
            else
            {
                msg = String.Format("{0}{1}", IndentString, name);
            }
            return msg;
        }

        public bool Indent
        {
            get;
            set;
        }

        private String IndentString
        {
            get
            {
                if (Indent)
                    return " ";
                return String.Empty;
            }
        }

        public String UsageDescription
        {
            get
            {
                String msg = String.Format("{0} (mandatory: {1}, allowed count: {2}", Description,
                        GetYesNoString(Mandatory), Count);
                if (AllowedAfter != null)
                {
                    msg += String.Format(", allowed after: {0}", AllowedAfter.NameString);
                }
                msg += ")";
                return msg;
            }
        }

        private String GetYesNoString(bool value)
        {
            if (value)
                return "yes";
            else
                return "no";
        }

        public override string ToString()
        {
            return GetUsageName(NameString) + UsageDescription;
        }

        public void SetFoundValues(string value, int foundCount)
        {
            StringValue = value;
            FoundCount = foundCount;
        }

        public void AddGroupArg(string name, string value)
        {
            argGroup.Add(new RawArgData(name, value));
        }

        internal IArgTreeItem BuildTree()
        {
            TreeBuilder bracktes = new TreeBuilder(argGroup, new FilterFactory());
            IArgTreeItem root = bracktes.GetItemTree();
            root.ValidateTree();
            return root;
        }


        public IArgTreeItem GetFilteringTree()
        {
            if (filterTree == null)
            {
                filterTree = BuildTree();
            }
            return filterTree;
        }

        internal bool IsOption(IEnumerable<String> argNames)
        {
            foreach (var name in argNames)
            {
                if (IsOption(name))
                    return true;
            }
            return false;
        }
        internal bool IsOption(string argName)
        {
            if (Options != null && Options.Count > 0)
            {
                return Options.FirstOrDefault(arg => { return arg.Names.Contains(argName); }) != null;
            }
            return false;
        }

        public int ArgGroupCount
        {
            get
            {
                return argGroup == null ? 0 : argGroup.Count;
            }
        }

        public void CheckNameValid(string name)
        {
            if (!Names.Contains(name)) throw new ArgumentException("argument name is invalid: " + name);
        }

        public String NameString
        {
            get
            {
                return String.Join(", ", Names.ToArray());
            }
        }
    }
}
