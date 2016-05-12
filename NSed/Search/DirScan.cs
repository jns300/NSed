using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSed.Arguments.NSed;
using System.IO;
using ArgumentHelper.Arguments.General.Operators;
using ArgumentHelper.Arguments.FileFilters;
using System.Diagnostics.Contracts;

namespace NSed.Search
{
    public class DirScan
    {
        private NSedAllowedArgs args;
        private IArgTreeItem treeRoot = new EmptyItem();
        private LinkedList<String> foundFiles = new LinkedList<String>();
        private int minDepth = -1;
        private int maxDepth = -1;

        public DirScan(NSedAllowedArgs args)
        {
            Contract.Requires(args != null);
            this.args = args;
            if (args.Mindepth.FoundCount > 0)
            {
                if (!int.TryParse(args.Mindepth.StringValue, out minDepth))
                {
                    throw new ArgumentException("min depth is not a valid integer number");
                }
            }
            if (args.Maxdepth.FoundCount > 0)
            {
                if (!int.TryParse(args.Maxdepth.StringValue, out maxDepth))
                {
                    throw new ArgumentException("max depth is not a valid integer number");
                }
            }
        }

        public void Scan()
        {
            if (args.Scan.FoundCount > 0)
            {
                treeRoot = args.Scan.GetFilteringTree();
                StartScan(1, new DirectoryInfo(args.Scan.StringValue));
                Console.WriteLine("Number of files with matches: {0}", MatchedFileCount);
                Console.WriteLine("Number of found files: {0}", AllFileCount);
            }
            else
            {
                HandleFile(null);
            }
        }

        private void StartScan(int depth, DirectoryInfo dir)
        {
            if ((minDepth < 0 || depth >= minDepth) && (maxDepth < 0 || depth <= maxDepth))
            {
                foreach (var fileInfo in dir.EnumerateFiles())
                {
                    String relPath = GetRelativePath(fileInfo.FullName);
                    if (treeRoot.Test(new FilterContext(fileInfo, relPath, depth)))
                    {
                        HandleFile(fileInfo.FullName);
                        foundFiles.AddLast(relPath);
                    }
                }
            }
            if (maxDepth < 0 || depth < maxDepth)
            {
                foreach (var dirInfo in dir.EnumerateDirectories())
                {
                    StartScan(depth + 1, dirInfo);
                }
            }
        }

        private string GetRelativePath(string filePath)
        {
            if (args.Scan.FoundCount > 0)
            {
                return GetRelativePath(args.Scan.StringValue, filePath);
            }
            else
            {
                if (Path.GetFullPath(filePath).StartsWith(Environment.CurrentDirectory))
                {
                    return GetRelativePath(Environment.CurrentDirectory, filePath);
                }
            }
            return filePath;
        }

        public static string GetRelativePath(string startPath, string filePath)
        {
            String startFull = Path.GetFullPath(startPath);
            String fileFull = Path.GetFullPath(filePath);
            if (!fileFull.StartsWith(startFull))
            {
                throw new InvalidOperationException(String.Format("file path '{0}' does not start from path '{1}'", fileFull, startPath));
            }
            String relative = fileFull.Substring(startFull.Length).Replace('\\', '/');
            if (relative.StartsWith("/"))
            {
                relative = relative.Insert(0, ".");
            }
            else
            {
                relative = relative.Insert(0, "./");
            }
            return relative;
        }

        private void HandleFile(String filePath)
        {
            try
            {
                SearchAndReplace search = new SearchAndReplace(args, filePath);
                search.Perform();
                if (search.MatchCount > 0)
                {
                    MatchedFileCount++;
                    Console.WriteLine("File: {0}, matches: {1}", GetRelativePath(search.FilePath), search.MatchCount);
                }
                AllFileCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to perform search/replace on file: " + filePath);
                Console.WriteLine("Reason: " + ex.Message);
                Console.WriteLine("Stack trace: " + ex.ToString());
            }
        }

        public List<String> FoundFiles { get { return new List<String>(foundFiles); } }

        public int MatchedFileCount { get; private set; }
        public int AllFileCount { get; private set; }
    }
}
