using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArgumentHelper.Arguments.General.Operators;

namespace ArgumentHelper.Arguments.FileFilters
{
    public class MTimeFilter : AbstractArgTreeItem
    {
        private int? n;
        private TimeEnum? timeEnum;

        public MTimeFilter()
        {
        }
        public MTimeFilter(int minDepth)
        {
            this.n = minDepth;
        }

        public override bool Test(ITestContext context)
        {
            double dayDiff = DateTime.Now.Subtract(((FilterContext)context).LastModificationTime).TotalDays;
            return TimeHelper.TestDateDiff(timeEnum, dayDiff, n);
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
            get { return "mtime"; }
        }
    }
}
