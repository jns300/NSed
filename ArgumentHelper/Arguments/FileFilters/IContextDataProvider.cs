using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ArgumentHelper.Arguments.FileFilters
{
    public interface IContextDataProvider
    {
        DateTime GetModificationTime(FileInfo file);
    }
}
