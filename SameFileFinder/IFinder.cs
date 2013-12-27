using System.Collections.Generic;

namespace SameFileFinder
{
    public interface IFinder
    {
        List<FileGroup> FindGroupOfSameFiles(string path, ILogger logger,IFileManager fileManagers);
    }
}
