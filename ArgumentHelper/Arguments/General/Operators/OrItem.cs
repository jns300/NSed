using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgumentHelper.Arguments.General.Operators
{
    public class OrItem : AbstractArgTreeItem
    {
        public override bool Test(ITestContext context)
        {
            foreach (var c in children)
            {
                if (c.Test(context))
                    return true;
            }
            return false;
        }
        public override bool Validate()
        {
            return children.Count >= 2;
        }

        public override string ItemName
        {
            get { return "or"; }
        }
    }
}
