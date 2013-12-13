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
            var checkedGroups = f.FindGroupOfSameFiles(@"D:\folder");
            if (checkedGroups != null)
            {
                if (checkedGroups.Count != 0)
                {
                    for (int i = 0; i < checkedGroups.Count; i++)
                    {
                        Console.WriteLine("Group " + (i + 1).ToString() + ":");
                        checkedGroups[i].print();
                    }
                }
                else
                {
                    Console.WriteLine("There arent same files at this folder");
                }
            }
        }
    }
}
