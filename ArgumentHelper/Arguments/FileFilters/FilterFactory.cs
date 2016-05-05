using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArgumentHelper.Arguments.General.Operators;
using ArgumentHelper.Arguments.FileFilters;
using ArgumentHelper.Arguments.General;

namespace ArgumentHelper.Arguments.FileFilters
{
    public class FilterFactory : IItemFactory
    {
        public IArgTreeItem CreateItem(string argName)
        {
            switch (argName)
            {
                case "-mtime" :
                    return new MTimeFilter();
                case "-mmin":
                    return new MMinFilter();
                case "-name":
                    return new FileNameFilter();
                case "-path-regex":
                    return new RegexPathFilter();
                case "-regex":
                    return new RegexNameFilter();
                default:
                    throw new ArgumentValidationException("unexpected argument name: " + argName);
            }
        }
    }
}
