using System;
using SameFileFinder;

namespace ConsoleSameFileFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new SameFileFinder.Logger("log.txt");
            var manager = new ConsoleManager(args, logger);
            var fileManager = new FileManager();
            var finder = new Finder();
            //var groups = finder.FindGroupOfSameFiles("C:\\Users", logger, fileManager);
            manager.Execute(fileManager,finder,logger);        
        }
    }
}
