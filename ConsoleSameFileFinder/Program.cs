﻿using System;
using SameFileFinder;

namespace ConsoleSameFileFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger("log.txt");
            var manager = new ConsoleManager(args, logger);
            var fileManager = new FileManager();
            var finder = new Finder();
            manager.Execute(fileManager, finder, logger);
        }
    }

    public class FakeLogger : ILogger
    {
        public void Write(Exception e)
        {
        }

        public void Write(string message)
        {
        }
    }
}
