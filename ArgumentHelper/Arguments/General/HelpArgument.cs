using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments.General
{
    /// <summary>
    /// Represents an argument which displays help.
    /// </summary>
    public class HelpArgument : Argument
    {
        private const String DEFAULT_DESCRIPTION = "Displays this help";

        public HelpArgument(String name, String description = DEFAULT_DESCRIPTION)
            : this(new String[] { name }, description)
        {
        }
        public HelpArgument(String[] name, String description = DEFAULT_DESCRIPTION)
            : base(name, description, false, 1, false)
        {
        }
    }
}
