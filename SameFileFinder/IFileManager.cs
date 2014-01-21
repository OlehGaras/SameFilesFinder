using System.Collections.Generic;
using System.Threading;

namespace SameFileFinder
{
    public interface IFileManager
    {
        List<FileInfo> DirSearch(string dir,ILogger logger);
        bool ByteCompare(FileInfo file1, FileInfo file2, ILogger logger,CancellationToken t);
    }
}
