using System;
using System.Collections.Generic;
using SameFileFinder;

namespace ConsoleSameFileFinder
{
    public class ConsoleManager
    {
        public string[] Args { get; private set; }
        public ILogger Logger { get; private set; }

        public ConsoleManager(string[] args, ILogger logger)
        {
            Args = args;
            Logger = logger;
            Logger.Write("APPLICATION STARTED");  
        }

        public void Execute(IFileManager fileManager, IFinder finder,ILogger logger)
        {  
            if (Args.Length == 2)
            {
                switch (Args[0])
                {
                    case "--dir":
                        break;
                    default:
                        Console.WriteLine("This command is not recognized as an internal or external command, operable program or batch file.");
                        return;
                }

                string path = Args[1];
                var groups = finder.FindGroupOfSameFiles(path, Logger, fileManager);
                
                if (groups.Count == 0)
                {
                    Console.WriteLine("There arent same files at this folder or you wrote bad path");
                    return;
                }
                PrintGroups(groups);
                Console.WriteLine(groups.Count);
            }
            else
            {
                Console.WriteLine("This command is not recognized as an internal or external command, operable program or batch file.");
            }
            Logger.Write("APPLICATION ENDED");
        }

        public void PrintGroups(List<FileGroup> groups)
        {
            int i = 0;
            foreach (var fileGroup in groups)
            {
                if (fileGroup.Files.Count > 1)
                {
                    Console.WriteLine("Group " + (i + 1).ToString() + ":");
                    Console.WriteLine(fileGroup.ToString());
                    i++;
                }
            }
        }
    }
}
