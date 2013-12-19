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
        public FileInfo Information { get; set; }
        public Int64 Hash { get; set; }

        public MyFileInfo(FileInfo inform, Int64 hash)
        {
            Information = inform;
            Hash = hash;
        }   
    }
}
