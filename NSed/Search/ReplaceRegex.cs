using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSed.Search
{
    public class ReplaceRegex
    {
        private interface IReplacePart
        {
            String GetString(Match m);
        }

        private class ConstStringPart : IReplacePart
        {
            private String str;
            public ConstStringPart(String str)
            {
                this.str = str;
            }

            public string GetString(Match m)
            {
                return str;
            }
        }

        private class MatchGroupPart : IReplacePart
        {
            private int groupIndex;
            public MatchGroupPart(int groupIndex)
            {
                this.groupIndex = groupIndex;
            }

            public string GetString(Match m)
            {
                if (groupIndex < 1 || groupIndex >= m.Groups.Count)
                {
                    throw new ArgumentException("invalid group of index: " + groupIndex);
                }
                return m.Groups[groupIndex].Value;
            }
        }

        private string replaceRegex;

        private List<IReplacePart> replaceParts = new List<IReplacePart>();
        private int groupCount;
        private int maxGroupNumberLen;

        public ReplaceRegex(String findRegex, string replaceRegex)
        {
            Contract.Requires(findRegex != null);
            Contract.Requires(replaceRegex != null);

            this.replaceRegex = replaceRegex;
            this.groupCount = GetGroupCount(findRegex);
            maxGroupNumberLen = groupCount.ToString().Length;
            Initialize();
        }

        public static int GetGroupCount(string findRegex)
        {
            Contract.Requires(findRegex != null);
            int openCount = 0;
            int closeCount = 0;
            int len = findRegex.Length;
            int i = 0;
            StringBuilder sb = new StringBuilder();
            while (i < len)
            {
                char ch = findRegex[i];
                if (ch == '\\')
                {
                    char la = LA(findRegex, i + 1);
                    if (la == '\\')
                    {
                        i++;
                    }
                    else if (la == '(' || la == ')')
                    {
                        i++;
                    }
                }
                else if (ch == '(')
                {
                    openCount++;
                }
                else if (ch == ')')
                {
                    char l2 = LA(findRegex, i + 2);
                    // ? has no effect on group count
                    if (l2 == '*' || l2 == '+' || l2 == '{')
                    {
                        WarningMessage.WarnWildcardsForGroup();
                    }
                    closeCount++;
                }
                i++;
            }
            if (openCount != closeCount)
            {
                throw new ArgumentException("round brackets are incorrectly paired");
            }
            return openCount;
        }

        private void Initialize()
        {
            int len = replaceRegex.Length;
            int i = 0;
            StringBuilder sb = new StringBuilder();
            while (i < len)
            {
                char ch = replaceRegex[i];
                if (ch == '\\')
                {
                    char la = LA(i + 1);
                    char la2 = LA(i + 2);
                    char la3 = LA(i + 3);
                    if (la == '\\')
                    {
                        sb.Append('\\');
                        i++;
                    }
                    else if (la == 'r' && la2 == '\\' && la3 == 'n' || la == 'n')
                    {
                        sb.Append(Environment.NewLine);
                        i++;
                        if (la == 'r' && la2 == '\\' && la3 == 'n')
                        {
                            i += 2;
                        }
                    }
                    else if (Char.IsDigit(la))
                    {
                        if (sb.Length > 0)
                        {
                            replaceParts.Add(new ConstStringPart(sb.ToString()));
                            sb.Clear();
                        }
                        int newIndex;
                        int number = EatNumber(i + 1, out newIndex, maxGroupNumberLen);
                        i = newIndex;
                        replaceParts.Add(new MatchGroupPart(number));
                    }
                }
                else
                {
                    sb.Append(ch);
                }
                i++;
            } // while
            if (sb.Length > 0)
            {
                replaceParts.Add(new ConstStringPart(sb.ToString()));
            }
        }

        private int EatNumber(int startIndex, out int newIndex, int maxLen)
        {
            int len = replaceRegex.Length;
            int i = startIndex;
            int numLen = 0;
            while (i < len && Char.IsDigit(replaceRegex[i]) && numLen < maxLen)
            {
                i++;
                numLen++;
            }
            String strNum = replaceRegex.Substring(startIndex, i - startIndex);
            int parsed = int.Parse(strNum);
            newIndex = i - 1;
            return parsed;
        }

        private char LA(int indexOfChar)
        {
            return LA(replaceRegex, indexOfChar);
        }
        private static char LA(String str, int indexOfChar)
        {
            if (indexOfChar < str.Length)
            {
                return str[indexOfChar];
            }
            return '\0';
        }

        public String GetReplaced(Match inMatch, String str)
        {
            int matchCount;
            return GetReplaced(inMatch, str, out matchCount);
        }

        public String GetReplaced(Match inMatch, String str, out int matchCount)
        {
            matchCount = 0;
            Match m = inMatch;
            StringBuilder sb = new StringBuilder();
            int lastIndex = 0;
            while (m.Success)
            {
                matchCount++;
                sb.Append(str.Substring(lastIndex, m.Index - lastIndex));
                foreach (IReplacePart part in replaceParts)
                {
                    String partStr = part.GetString(m);
                    sb.Append(partStr);
                }
                lastIndex = m.Index + m.Length;
                m = m.NextMatch();
            }
            sb.Append(str.Substring(lastIndex));
            return sb.ToString();
        }
    }
}
