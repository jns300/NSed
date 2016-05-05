using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgumentHelper.Arguments.General.Operators
{
    public class EmptyItem : AbstractArgTreeItem
    {
        public override bool Test(ITestContext context)
        {
            return true;
        }

        public override bool Validate()
        {
            return true;
        }

        public override string ItemName
        {
            get { return "empty"; }
        }
    }
}
