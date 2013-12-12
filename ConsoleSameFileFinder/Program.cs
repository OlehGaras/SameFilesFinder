using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SameFileFinder;

namespace ConsoleSameFileFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            IFinder f = new Finder();
            f.FindGroupOfSameFiles(@"D:\downloads");
        }
    }
}
