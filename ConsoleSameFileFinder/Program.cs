using System;
using SameFileFinder;

namespace ConsoleSameFileFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new SameFileFinder.Logger("log.txt");
            args = new string[2];
            args[0] = "--dir";
            args[1] = "C:\\Users";
            var manager = new ConsoleManager(args, logger);
            var fileManager = new FileManager();
            var finder = new Finder();
            //var groups = finder.FindGroupOfSameFiles("D:\\SameLenDifcontent", logger, fileManager);
            //var groups = finder.FindGroupOfSameFiles("C:\\Users", logger, fileManager);
            manager.Execute(fileManager,finder,logger);        
      }
    }
}
