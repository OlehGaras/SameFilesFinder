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
        public List<MyFileInfo> Group { get; set; }

        public FileGroup()
        {
            Group = new List<MyFileInfo>();
        }

        public override string ToString()
        {
            var res = string.Empty;
            foreach (var file in Group)
            {
                res += "\t" + file.Information.Name + "\n"; 
            }
            return res;
        }

        public void Add(MyFileInfo file)
        {
            Group.Add(file);
        }
    }
}
