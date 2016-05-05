using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgumentHelper.Arguments.General.Operators
{
    public abstract class AbstractArgTreeItem : IArgTreeItem
    {
        protected List<IArgTreeItem> children = new List<IArgTreeItem>();

        public void AddChildren(params IArgTreeItem[] toAdd)
        {
            children.AddRange(toAdd);
        }

        public bool HasChildren { get { return children.Count > 0; } }

        public abstract bool Test(ITestContext context);

        public abstract bool Validate();

        public virtual bool IsValueRequired()
        {
            return false;
        }
        public virtual void SetValue(String value)
        {
            // by default do nothing
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            ToString(sb);
            return sb.ToString();
        }

        public abstract String ItemName { get; }


        public void ToString(StringBuilder builder)
        {
            builder.Append(ItemName);
            builder.Append("(");
            int index = 0;
            foreach (IArgTreeItem item in children)
            {
                if (index++ > 0)
                    builder.Append(", ");
                item.ToString(builder);
            }

            builder.Append(")");
            builder.ToString();
        }


        public void ValidateTree()
        {
            foreach (IArgTreeItem item in children)
            {
                if (!item.Validate())
                {
                    throw new InvalidOperationException(String.Format("tree item is invalid, name: {0}, tree: {1}", item.ItemName, item.ToString()));
                }
                item.ValidateTree();
            }   
        }
    }
}
