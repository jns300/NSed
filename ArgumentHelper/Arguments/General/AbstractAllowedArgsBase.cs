using ArgumentHelper.Arguments.General.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments.General
{
    public abstract class AbstractAllowedArgsBase : AbstractAllowedArgs
    {
        private Dictionary<String, Argument> allowedArgsMap = new Dictionary<String, Argument>(StringComparer.InvariantCultureIgnoreCase);

        private List<Argument> allowedArgsList = new List<Argument>();

        private bool isInitialized;

        public AbstractAllowedArgsBase(Assembly mainAssembly)
            : base(mainAssembly)
        {

        }

        public override void Initialize()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                AddHelpArguments();
                InstantiateArguments();
                FillArgumentMap();
                ArgumentsCreated();
            }
        }

        protected virtual void ArgumentsCreated()
        {

        }

        private void AddHelpArguments()
        {
            foreach (var h in GetHelpArguments())
            {
                AddArg(h);
            }

            foreach (var h in GetRawHelpArguments())
            {
                AddArg(h);
            }
        }

        protected override IReadOnlyList<Argument> GetOrderedArguments()
        {
            return allowedArgsList;
        }

        protected virtual IEnumerable<HelpArgument> GetHelpArguments()
        {
            return new HelpArgument[] { new HelpArgument(new String[] { "-h", "-help" }) };
        }

        protected virtual IEnumerable<RawHelpArgument> GetRawHelpArguments()
        {
            return new RawHelpArgument[] { new RawHelpArgument(new String[] { "-raw-help" }) };
        }

        private void FillArgumentMap()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(Argument) && prop.CanRead && !prop.GetGetMethod().IsStatic
                    && !prop.CustomAttributes.Select(a => a.AttributeType).Contains(typeof(NonArgumentAttribute)))
                {
                    Argument argument = (Argument)prop.GetMethod.Invoke(this, null);
                    if (argument == null && !SkipNullArguments)
                    {
                        throw new InvalidOperationException("property returned null argument: " + prop.Name);
                    }
                    if (argument != null)
                    {
                        AddArg(argument);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether null arguments can be ommited and generated no error.
        /// </summary>
        protected bool SkipNullArguments { get; set; }

        protected abstract void InstantiateArguments();

        public override bool IsArgAllowed(String name)
        {
            return allowedArgsMap.ContainsKey(name);
        }

        public override Argument GetArgument(string name)
        {
            return allowedArgsMap[name];
        }

        public override IReadOnlyDictionary<string, Argument> GetAllowedMap()
        {
            return new System.Collections.ObjectModel.ReadOnlyDictionary<String, Argument>(allowedArgsMap);
        }

        private void AddArg(Argument arg)
        {
            foreach (var name in arg.Names)
            {
                allowedArgsMap.Add(name, arg);
            }
            allowedArgsList.Add(arg);
        }

        public static float? ParseFloat(Argument arg)
        {
            float num;
            if (!float.TryParse(arg.StringValue, NumberStyles.Float, new CultureInfo("en-US"), out num))
            {
                arg.StringValue = arg.StringValue.Replace(',', '.');
                if (!float.TryParse(arg.StringValue, NumberStyles.Float, new CultureInfo("en-US"), out num))
                    return null;
            }
            return num;
        }
    }
}
