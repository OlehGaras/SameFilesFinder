using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SameFileFinder
{
    public interface IFinder
    {
        List<FileGroup> FindGroupOfSameFiles(string path, ILogger logger,IFileManager fileManagers);
    }
}
