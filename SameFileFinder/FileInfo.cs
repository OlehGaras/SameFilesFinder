using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SameFileFinder
{
    public class MyFileInfo
    {
        public string Path { get; set; }
        public long Length { get; set; }
        public string Name { get; set; }
        public FileInfo Information { get; set; }
        public Int64 Hash { get; set; }

        public MyFileInfo()
        {
        }

        public MyFileInfo(FileInfo inform, Int64 hash)
        {
            Information = inform;
            Path = Information.DirectoryName + @"\" + Information.Name;
            Length = Information.Length;
            Name = Information.Name;
            Hash = hash;
        }   
    }
}
