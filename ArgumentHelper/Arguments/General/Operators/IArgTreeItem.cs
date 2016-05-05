using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgumentHelper.Arguments.General.Operators
{
    public interface IArgTreeItem
    {
        bool Test(ITestContext context);

        bool HasChildren { get; }

        bool Validate();

        bool IsValueRequired();

        void SetValue(String value);

        void ToString(StringBuilder builder);

        void ValidateTree();

        String ItemName { get; }
    }
}
