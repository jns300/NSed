using ArgumentHelper.Arguments.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ArgumentHelper.Arguments.General
{
    public interface IAllowedArgs
    {
        bool IsArgAllowed(String name);

        Argument GetArgument(String name);

        IReadOnlyDictionary<String, Argument> GetAllowedMap();

        string GetUsage(ITextAppender appender);

        string HelpSwitch { get; }

        void Initialize();
    }
}
