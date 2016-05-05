using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments.General
{
    public class ExampleGroup
    {
        public ExampleGroup(String[] examples)
        {
            if (examples == null) throw new ArgumentNullException();
            Examples = examples;
        }

        public ExampleGroup(String name, String[] examples)
        {
            if (examples == null) throw new ArgumentNullException();
            Name = name;
            Examples = examples;
        }

        public string Name { get; private set; }

        public string[] Examples { get; private set; }
    }
}
