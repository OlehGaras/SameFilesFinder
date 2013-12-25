using System;
using SameFileFinder;

namespace ConsoleSameFileFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new SameFileFinder.Logger("log.txt");
            //args[1] = "D:\\DifferentGroups";
            args[1] = @"C:\Users";
            var manager = new ConsoleManager(args, logger);
            var fileManager = new FileManager();
            var finder = new Finder();
            manager.Execute(fileManager,finder,logger);        
      }
    }
}
