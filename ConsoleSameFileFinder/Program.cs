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
            manager.Execute();
        }
    }
}
