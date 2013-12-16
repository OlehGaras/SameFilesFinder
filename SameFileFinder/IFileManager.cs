using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SameFileFinder
{
    public interface IFileManager
    {
        void DirSearch(string dir,Logger logger);
    }
}
