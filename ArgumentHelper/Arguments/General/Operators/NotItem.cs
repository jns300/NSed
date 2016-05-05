using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgumentHelper.Arguments.General.Operators
{
    public class NotItem : AbstractArgTreeItem
    {
        public override bool Test(ITestContext context)
        {
            return !children[0].Test(context);
        }

        public override bool Validate()
        {
            return children.Count == 1;
        }
        public override string ItemName
        {
            get { return "not"; }
        }
    }
}
