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
        List<MyFileInfo> DirSearch(string dir,ILogger logger);
        bool ByteCompare(MyFileInfo file1, MyFileInfo file2, ILogger logger);
    }
}
