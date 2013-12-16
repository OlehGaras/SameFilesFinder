using System;
using SameFileFinder;

namespace ConsoleSameFileFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new SameFileFinder.Logger("log.txt");
            logger.AppStart();
            //logger.Write(string.Format("{0},{1}","123","234","345"));
            var f = new Finder();
            var groups = f.FindGroupOfSameFiles(@"D:\",logger);
            if (groups.Count == 0)
            {
                Console.WriteLine("There arent same files at this folder");
                return;
            }
            int i = 0;
            foreach (var fileGroup in groups)
            {
                if (fileGroup.Value.Group.Count > 1)
                {
                    Console.WriteLine("Group " + (i + 1).ToString() + ":");
                    Console.WriteLine(fileGroup.Value.ToString());
                    i++;
                }
            }
            logger.AppEnd();
        }
    }
}
