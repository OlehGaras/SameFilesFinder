using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SameFileFinder
{
    public class FileGroup : IFileGroup
    {
        public List<FileInfo> Files { get; private set; }

        public FileGroup()
        {
            Files = new List<FileInfo>();
        }

        public override string ToString()
        {
            var res = string.Empty;
            foreach (var file in Files)
            {
                res += "\t" + file.Path + "\n"; 
            }
            return res;
        }

        public void Add(FileInfo file)
        {
            Files.Add(file);
        }
    }
}
