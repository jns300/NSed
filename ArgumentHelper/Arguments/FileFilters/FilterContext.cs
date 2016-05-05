using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ArgumentHelper.Arguments.General.Operators;

namespace ArgumentHelper.Arguments.FileFilters
{
    public class FilterContext : ITestContext
    {
        public String FileName { get; private set; }
        public String FilePath { get; private set; }
        public DateTime LastModificationTime
        {
            get
            {
                if (provider != null)
                {
                    return provider.GetModificationTime(fileInfo);
                }
                return modificationTime;
            }
        }
        public int Depth { get; private set; }

        private DateTime modificationTime;

        private static IContextDataProvider provider;
        private FileInfo fileInfo;

        public FilterContext(FileInfo fileInfo, string filePath, int depth)
        {
            FileName = Path.GetFileName(filePath);
            FilePath = filePath;
            Depth = depth;
            this.fileInfo = fileInfo;
            modificationTime = fileInfo.LastWriteTime;
        }

        public static void SetDataProvider(IContextDataProvider newProvider)
        {
            provider = newProvider;
        }
    }
}
