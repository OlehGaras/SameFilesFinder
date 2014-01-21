using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace SameFileFinder
{
    public interface IFinder
    {
        List<FileGroup> FindGroupOfSameFiles(string path, ILogger logger, IFileManager fileManagers, CancellationToken e);
    }
}
