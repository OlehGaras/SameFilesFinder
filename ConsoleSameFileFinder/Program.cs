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
            var checkedGroups = f.FindGroupOfSameFiles(@"D:\folder2");
            if (checkedGroups.Count == 0)
            {
                Console.WriteLine("There arent same files at this folder");
                return;
            }
            for (int i = 0; i < checkedGroups.Count; i++)
            {
                Console.WriteLine("Group " + (i + 1).ToString() + ":");
                Console.WriteLine(checkedGroups[i].ToString());
            }

        }
    }
}
