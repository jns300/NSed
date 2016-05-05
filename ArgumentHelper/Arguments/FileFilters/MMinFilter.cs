using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArgumentHelper.Arguments.General.Operators;

namespace ArgumentHelper.Arguments.FileFilters
{
    class MMinFilter : AbstractArgTreeItem
    {
        private int? n;

        private TimeEnum? timeEnum;

        public MMinFilter()
        {
        }
        public MMinFilter(int maxDepth)
        {
            this.n = maxDepth;
        }
        public override bool Test(ITestContext context)
        {
            double minuteDiff = DateTime.Now.Subtract(((FilterContext)context).LastModificationTime).TotalMinutes;
            return TimeHelper.TestDateDiff(timeEnum, minuteDiff, n);
        }
        public override bool Validate()
        {
            return n.HasValue && timeEnum.HasValue;
        }
        public override bool IsValueRequired()
        {
            return true;
        }

        public override void SetValue(String value)
        {
            timeEnum = TimeHelper.ParserTime(ref value);
            n = int.Parse(value);
        }

        public override string ItemName
        {
            get { return "mmin"; }
        }
    }
}
