using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using NSed.Search.Region;
using ArgumentHelper.Arguments.General;
using NSed.Arguments.NSed;
using System.Diagnostics.Contracts;
using NSed.Util;

namespace NSed.Search
{
    public class SearchAndReplace
    {
        private NSedAllowedArgs arguments;
        private readonly Regex findRegex;
        private int matchCount;
        private readonly string outFilePath;
        private ReplaceRegex replaceRegex;
        private bool? isLastCharNewLine;

        public SearchAndReplace(NSedAllowedArgs args, String filePath)
        {
            CustomContract.Requires(args != null);
            this.arguments = args;
            FilePath = filePath == null ? args.FilePath.StringValue : filePath;
            if (FilePath == null)
            {
                throw new ArgumentException("file path was not specified");
            }
            DeleteFilePermanently = args.DeletePermanently.FoundCount > 0;
            findRegex = BuildFindRegex();
            if (CanReplace)
            {
                replaceRegex = new ReplaceRegex(args.Find.StringValue, args.Replace.StringValue);
                FileInfo info = new FileInfo(FilePath);
                outFilePath = Path.Combine(info.DirectoryName, Path.GetFileNameWithoutExtension(FilePath) + ".~nsedtmp");
            }
        }

        public void Perform()
        {
            matchCount = 0;
            if (!IsMultiline && !HasLineIndicators)
            {
                SimpleFind();
            }
            else
            {
                ComplexFind();
            }
        }

        private void ComplexFind()
        {
            String allText = File.ReadAllText(FilePath);
            isLastCharNewLine = allText.Length > 0 && allText[allText.Length - 1] == '\n';
            String[] lines = GetLines(allText);
            SearchRegion region = SearchRegion.GetSearchRegion(lines,
                GetInt(arguments.StartLineIndex), GetInt(arguments.EndLineIndex),
                arguments.StartLineRegex.StringValue, arguments.EndLineRegex.StringValue,
                GetInt(arguments.StartLineOffset), GetInt(arguments.EndLineOffset),
                arguments.UseLastLine.FoundCount > 0, IsCaseSensitive, isLastCharNewLine.Value);
            bool canReplace = CanReplace;
            FileStream writeStream = null;
            StreamWriter sw = null;
            try
            {
                GetOutputStreams(out writeStream, out sw);
                if (canReplace)
                {
                    foreach (LineData line in region.GetBeforeLines())
                    {
                        WriteNotMatched(sw, line.Line, line.AppendNewLine);
                    }
                }

                if (region.SelectedLineCount > 0)
                {
                    if (IsMultiline)
                    {
                        String selected = region.GetSelectedLines();

                        Match m = findRegex.Match(selected);
                        if (m.Success)
                        {
                            ReportMatchesAndWrite(sw, m, selected, false);
                        }
                        else
                        {
                            WriteNotMatched(sw, selected, false);
                        }
                    }
                    else
                    {
                        foreach (LineData line in region.GetSelectedLinesEnumerator())
                        {
                            HandleSingleLine(sw, line);
                        }
                    }
                }

                if (canReplace)
                {
                    foreach (LineData line in region.GetAfterLines())
                    {
                        WriteNotMatched(sw, line.Line, line.AppendNewLine);
                    }
                }
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
            }
            ReplaceOriginal();
        }

        private string[] GetLines(string input)
        {
            LinkedList<String> lines = new LinkedList<string>();
            using (System.IO.StringReader reader = new System.IO.StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.AddLast(line);
                }
            }
            return lines.ToArray();
        }

        private int? GetInt(Argument arg)
        {
            if (arg.FoundCount == 0)
            {
                return null;
            }
            else
            {
                return int.Parse(arg.StringValue);
            }
        }

        private void SimpleFind()
        {
            FileStream inputStream = null;
            StreamReader sr = null;
            FileStream writeStream = null;
            StreamWriter sw = null;
            try
            {
                GetOutputStreams(out writeStream, out sw);
                inputStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                bool isLastCharNL = IsLastCharNewLine(inputStream);
                inputStream.Seek(0, SeekOrigin.Begin);
                sr = new StreamReader(inputStream, Encoding.GetEncoding(FileEncoding));
                inputStream = null;
                String line1 = sr.ReadLine();
                if (line1 != null)
                {
                    String line2;
                    while ((line2 = sr.ReadLine()) != null)
                    {
                        HandleSingleLine(sw, new LineData(line1, false, true));
                        line1 = line2;
                    }
                    HandleSingleLine(sw, new LineData(line1, true, isLastCharNL));
                }
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
                if (sr != null)
                {
                    sr.Close();
                }
                if (inputStream != null)
                {
                    inputStream.Close();
                }
            }
            ReplaceOriginal();
        }

        private void HandleSingleLine(StreamWriter writer, LineData line)
        {
            Match m = findRegex.Match(line.Line);
            if (m.Success)
            {
                ReportMatchesAndWrite(writer, m, line.Line, line.AppendNewLine);
            }
            else
            {
                WriteNotMatched(writer, line.Line, line.AppendNewLine);
            }
        }

        private void ReplaceOriginal()
        {
            if (CanReplace)
            {
                if (matchCount > 0)
                {
                    if (DeleteFilePermanently)
                    {
                        File.Delete(FilePath);
                    }
                    else
                    {
                        // Sending to recycle bin
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(FilePath,
                            Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                            Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                    }
                    File.Move(outFilePath, FilePath);
                    IsFileReplaced = true;
                }
                else
                {
                    File.Delete(outFilePath);
                }
            }
        }


        private void WriteNotMatched(StreamWriter writer, string line, bool addNewLine)
        {
            if (writer != null)
            {
                if (addNewLine)
                {
                    writer.WriteLine(line);
                }
                else
                {
                    writer.Write(line);
                }
            }
        }

        private void GetOutputStreams(out FileStream writeStream, out StreamWriter writer)
        {
            if (CanReplace)
            {
                writeStream = new FileStream(outFilePath, FileMode.OpenOrCreate);
                writer = new StreamWriter(writeStream, Encoding.GetEncoding(FileEncoding));
            }
            else
            {
                writeStream = null;
                writer = null;
            }
        }

        private void ReportMatchesAndWrite(StreamWriter writer, Match m, String line, bool addNewLine)
        {

            if (writer != null)
            {
                int replaceCount;
                String replaced = replaceRegex.GetReplaced(m, line, out replaceCount);
                matchCount += replaceCount;
                if (addNewLine)
                {
                    writer.WriteLine(replaced);
                }
                else
                {
                    writer.Write(replaced);
                }
            }
            else
            {
                while (m.Success)
                {
                    matchCount++;
                    Console.WriteLine("Match {0}: '{1}'", matchCount, m.Groups[0]);
                    m = m.NextMatch();
                }
            }
        }

        private Regex BuildFindRegex()
        {
            Regex regex = new Regex(arguments.Find.StringValue, RegexOptions.Compiled | RegexOptions.CultureInvariant |
                (IsMultiline ? RegexOptions.Multiline : RegexOptions.Singleline) |
                (!IsCaseSensitive ? RegexOptions.IgnoreCase : 0));
            return regex;
        }

        public bool IsMultiline
        {
            get
            {
                return arguments.Multiline.FoundCount > 0;
            }
        }

        public bool IsCaseSensitive
        {
            get
            {
                return arguments.CaseSensitive.FoundCount > 0;
            }
        }

        public bool CanReplace
        {
            get
            {
                return arguments.Replace.FoundCount > 0;
            }
        }
        public String FilePath
        {
            get;
            private set;
        }
        public String FileEncoding
        {
            get
            {
                if (arguments.FileEncoding.FoundCount == 0)
                {
                    return "Windows-1250";
                }
                return arguments.FileEncoding.StringValue;
            }
        }
        public bool HasLineIndicators
        {
            get
            {
                return arguments.StartLineIndex.FoundCount > 0 || arguments.EndLineIndex.FoundCount > 0
                    || arguments.StartLineRegex.FoundCount > 0 || arguments.EndLineRegex.FoundCount > 0;
            }
        }
        public int MatchCount
        {
            get { return matchCount; }
        }

        public bool DeleteFilePermanently { get; set; }

        public bool IsFileReplaced { get; private set; }

        /// <summary>
        /// Returns whether the last char of the stream is the new line character.
        /// </summary>
        /// <param name="fileStream">the open file stream</param>
        /// <returns>whether the last char of the stream is the new line character</returns>
        public bool IsLastCharNewLine(FileStream fileStream)
        {
            if (!isLastCharNewLine.HasValue)
            {
                bool lastNewLineValue;
                if (CanReplace)
                {
                    if (fileStream.Length > 0)
                    {
                        fileStream.Seek(-1, SeekOrigin.End);
                        int last = fileStream.ReadByte();
                        lastNewLineValue = last == '\n';
                    }
                    else
                    {
                        lastNewLineValue = false;
                    }
                }
                else
                {
                    lastNewLineValue = true;
                }
                isLastCharNewLine = lastNewLineValue;
            }
            return isLastCharNewLine.Value;
        }

        /// <summary>
        /// Opens the input file and checks whether the last char of the file stream is the new line character.
        /// </summary>
        /// <returns>whether the last char of the file stream is the new line character</returns>
        public bool CheckIsLastCharNewLine()
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                return IsLastCharNewLine(fs);
            }
        }

    }
}
