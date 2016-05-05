using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgumentHelper.Arguments.General.Operators
{
    public class TreeBuilder
    {
        private IItemFactory factory;
        private List<Argument.RawArgData> argGroup;
        private List<IArgTreeItem> itemList;

        public TreeBuilder(List<Argument.RawArgData> argGroup, IItemFactory factory)
        {
            this.factory = factory;
            while (argGroup.Count > 1)
            {
                if (argGroup[0].Name.Equals("(") && argGroup[argGroup.Count - 1].Name.Equals(")"))
                {
                    argGroup.RemoveAt(argGroup.Count - 1);
                    argGroup.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
            this.argGroup = argGroup;
            itemList = new List<IArgTreeItem>();
            int len = argGroup.Count;
            for (int i = 0; i < len; i++)
            {
                Argument.RawArgData rawArg = argGroup[i];
                String argName = rawArg.Name;
                if (argName == "(")
                {
                    TreeBuilder subBracket = new TreeBuilder(FindSubGroup(ref i), factory);
                    itemList.Add(subBracket.GetItemTree());
                }
                else if (argName == ")")
                {
                    if (i + 1 < len)
                    {
                        throw new ArgumentValidationException("Brackets are incorrectly paired");
                    }
                }
                else
                {
                    switch (argName)
                    {
                        case "-and":
                            itemList.Add(new AndItem());
                            break;
                        case "-or":
                            itemList.Add(new OrItem());
                            break;
                        case "-not":
                            itemList.Add(new NotItem());
                            break;
                        default:
                            IArgTreeItem item = factory.CreateItem(argName);
                            if (item.IsValueRequired())
                            {
                                String argValue = rawArg.Value;
                                if (argValue.Length == 0)
                                {
                                    throw new ArgumentValidationException(String.Format("argument '{0}' does not have a value", argName));
                                }
                                try
                                {
                                    item.SetValue(argValue);
                                }
                                catch (Exception ex)
                                {
                                    throw new ArgumentValidationException(String.Format("value '{0}' for argument '{1}' is invalid: {2}",
                                        argValue, argName, ex.Message), ex);
                                }
                            }
                            itemList.Add(item);
                            break;
                    }
                }

            }
        }

        public IArgTreeItem GetItemTree()
        {
            List<IArgTreeItem> newItemList = MergeNotItems(itemList);
            newItemList = MergeAllItems(newItemList, (item) => { return item as AndItem; });
            newItemList = MergeAllItems(newItemList, (item) => { return item as OrItem; });
            if (newItemList.Count > 1)
            {
                throw new ArgumentValidationException("Between some arguments operators (and/or) are missing.");
            }
            if (newItemList.Count == 0)
                return new EmptyItem();
            else
                return newItemList[0];
        }
        private List<IArgTreeItem> MergeAllItems(List<IArgTreeItem> currentItemList, Func<IArgTreeItem, AbstractArgTreeItem> itemFunc)
        {
            int beforeCount;
            List<IArgTreeItem> newItemList = currentItemList;
            do
            {
                beforeCount = newItemList.Count;
                newItemList = MergeOperatorItems(newItemList, itemFunc);
            } while (beforeCount > newItemList.Count);
            return newItemList;
        }
        private List<IArgTreeItem> MergeOperatorItems(List<IArgTreeItem> currentItemList, Func<IArgTreeItem, AbstractArgTreeItem> itemFunc)
        {
            List<IArgTreeItem> newItemList = new List<IArgTreeItem>(currentItemList.Count);
            int i = 0;
            while (i < currentItemList.Count)
            {
                IArgTreeItem item = currentItemList[i];
                AbstractArgTreeItem castedItem = itemFunc.Invoke(item);
                FillItem(currentItemList, newItemList, ref i, castedItem, item);
            }
            return newItemList;
        }

        private static void FillItem(List<IArgTreeItem> currentItemList, List<IArgTreeItem> newItemList, ref int i,
            AbstractArgTreeItem operatorItem, IArgTreeItem item)
        {
            if (operatorItem != null && !operatorItem.HasChildren)
            {
                if (i + 1 >= currentItemList.Count)
                {
                    throw new ArgumentValidationException("And operator has no right argument");
                }
                else if (i == 0)
                {
                    throw new ArgumentValidationException("And operator has no left argument");
                }
                else
                {
                    IArgTreeItem previous = newItemList[newItemList.Count - 1];
                    IArgTreeItem next = currentItemList[i + 1];
                    if (IsEmptyOperator(next))
                    {
                        throw new ArgumentValidationException("And operator is before empty operator 'and' or 'or'");
                    }
                    else if (IsEmptyOperator(previous))
                    {
                        throw new ArgumentValidationException("And operator is after empty operator 'and' or 'or'");
                    }
                    operatorItem.AddChildren(previous, next);
                    newItemList.RemoveAt(newItemList.Count - 1);
                    newItemList.Add(operatorItem);
                    i++;
                }
            }
            else
            {
                newItemList.Add(item);
            }
            i++;
        }

        private static bool IsEmptyOperator(IArgTreeItem item)
        {
            return item is AndItem && !((AndItem)item).HasChildren || item is OrItem && !((OrItem)item).HasChildren;
        }
        private List<IArgTreeItem> MergeNotItems(List<IArgTreeItem> currentItemList)
        {
            List<IArgTreeItem> newItemList = new List<IArgTreeItem>(currentItemList.Count);
            int i = 0;
            while (i < currentItemList.Count)
            {
                IArgTreeItem item = currentItemList[i];
                NotItem notItem = item as NotItem;
                if (notItem != null)
                {
                    if (i < currentItemList.Count - 1)
                    {
                        IArgTreeItem next = currentItemList[i + 1];
                        if (IsEmptyOperator(next))
                        {
                            throw new ArgumentValidationException("Not operator is before empty operator 'and' or 'or'");
                        }
                        notItem.AddChildren(next);
                        newItemList.Add(notItem);
                        i++;
                    }
                    else
                    {
                        throw new ArgumentValidationException("Not operator has not argument");
                    }
                }
                else
                {
                    newItemList.Add(item);
                }
                i++;
            }
            return newItemList;
        }

        private List<Argument.RawArgData> FindSubGroup(ref int i)
        {
            if (!argGroup[i].Name.Equals("("))
            {
                throw new ArgumentValidationException("Open bracket was expected");
            }
            List<Argument.RawArgData> subGrup = new List<Argument.RawArgData>();
            int len = argGroup.Count;
            int count = 0;
            while (i < len)
            {
                subGrup.Add(argGroup[i]);
                if (argGroup[i].Name.Equals("("))
                {
                    count++;
                }
                if (argGroup[i].Name.Equals(")"))
                {
                    count--;
                    if (count == 0)
                    {
                        break;
                    }
                }
                i++;
            }
            if (count != 0)
            {
                throw new ArgumentValidationException("Brackets are incorrectly paired");
            }
            return subGrup;
        }

        private string eatString(string toParse, ref int i)
        {
            StringBuilder sb = new StringBuilder();
            int len = toParse.Length;
            // Skip white
            while (i < len && Char.IsWhiteSpace(toParse[i])) i++;
            // Find arg name
            while (i < len)
            {
                char ch = toParse[i];
                if (ch == '(' || ch == ')' || Char.IsWhiteSpace(ch))
                {
                    break;
                }
                else
                {
                    sb.Append(ch);
                }
                i++;
            }
            return sb.ToString();
        }
    }
}
