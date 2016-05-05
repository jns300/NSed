using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgumentHelper.Arguments.General.Operators
{
    class AndItem : AbstractArgTreeItem
    {
        public override bool Test(ITestContext context)
        {
            foreach (var c in children)
            {
                if (!c.Test(context))
                    return false;
            }
            return true;
        }

        public override bool Validate()
        {
            return children.Count >= 2;
        }

        public override string ItemName
        {
            get { return "and"; }
        }
    }
}
