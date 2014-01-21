using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using SameFileFinder;

namespace ConsoleSameFileFinder
{
    public class ConsoleManager
    {
        public string[] Args { get; private set; }

        public ConsoleManager(string[] args)
        {
            Args = args;
        }

        public void Execute(IFileManager fileManager, IFinder finder,ILogger logger)
        {
            logger.Write("APPLICATION STARTED");  
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
                var groups = finder.FindGroupOfSameFiles(path, logger, fileManager,new CancellationToken());
                
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
            logger.Write("APPLICATION ENDED");
        }

        public void PrintGroups(List<FileGroup> groups)
        {
            int i = 0;
            foreach (var fileGroup in groups)
            {
                if (fileGroup.Files.Count > 1)
                {
                    Console.WriteLine("Group " + (i + 1).ToString(CultureInfo.InvariantCulture) + ":");
                    Console.WriteLine(fileGroup.ToString());
                    i++;
                }
            }
        }
    }
}
