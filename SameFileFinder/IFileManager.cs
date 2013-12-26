using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SameFileFinder
{
    public interface IFileManager
    {
        List<FileInfo> DirSearch(string dir,ILogger logger);
        bool ByteCompare(FileInfo file1, FileInfo file2, ILogger logger);
    }
}
