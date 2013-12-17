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
            FileManager fileManager = new FileManager();
            Finder finder = new Finder();
            manager.Execute(fileManager,finder);
        }
    }
}
