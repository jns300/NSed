using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgumentHelper.Arguments.General.Operators
{
    public interface IItemFactory
    {
        IArgTreeItem CreateItem(String argName);
    }
}
