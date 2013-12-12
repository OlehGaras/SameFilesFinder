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
        public List<FileInfo> m_Group = new List<FileInfo>();

        public void print()
        {
            foreach (var file in m_Group)
            {
                Console.WriteLine("\t"+file.Name);
            }
        }
    }
}
