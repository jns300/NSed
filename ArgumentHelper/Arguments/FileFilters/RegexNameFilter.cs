using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArgumentHelper.Arguments.General.Operators;
using System.Text.RegularExpressions;

namespace ArgumentHelper.Arguments.FileFilters
{
    public class RegexNameFilter : AbstractArgTreeItem
    {
        private Regex regex;

        public RegexNameFilter()
        {
        }
        public RegexNameFilter(String pattern)
        {
            CreateRegex(pattern);
        }

        private void CreateRegex(String pattern)
        {
            regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public override bool Test(ITestContext context)
        {
            return regex.IsMatch(((FilterContext)context).FileName);
        }

        public override bool Validate()
        {
            return regex != null;
        }
        public override bool IsValueRequired()
        {
            return true;
        }
        public override void SetValue(String value)
        {
            CreateRegex(value);
        }

        public override string ItemName
        {
            get { return "regex"; }
        }
    }
}
