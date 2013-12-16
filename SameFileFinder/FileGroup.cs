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
        public List<FileInfo> Group { get; set; }

        public FileGroup()
        {
            Group = new List<FileInfo>();
        }

        public override string ToString()
        {
            var res = string.Empty;
            foreach (var file in Group)
            {
                res += "\t" + file.Name + "\n"; 
            }
            return res;
        }

        public void Add(FileInfo file)
        {
            Group.Add(file);
        }
    }
}
