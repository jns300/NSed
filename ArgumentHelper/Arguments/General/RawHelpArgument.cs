using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentHelper.Arguments.General
{
    /// <summary>
    /// Represents an argument which controls whether the help text is raw or formatted.
    /// </summary>
    public class RawHelpArgument : Argument
    {
        private const String DEFAULT_DESCRIPTION = "Displays raw help text";

        public RawHelpArgument(String name, String description = DEFAULT_DESCRIPTION)
            : this(new String[] { name }, description)
        {
        }
        public RawHelpArgument(String[] name, String description = DEFAULT_DESCRIPTION)
            : base(name, description, false, 1, false)
        {
        }
    }
}
