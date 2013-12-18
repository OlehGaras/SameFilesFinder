using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SameFileFinder;

namespace ConsoleSameFileFinder
{
    public class ConsoleManager
    {
        public string[] Args { get; set; }
        public Logger Logger { get; set; }

        public ConsoleManager(string[] args,Logger logger)
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
                if (fileGroup.Group.Count > 1)
                {
                    Console.WriteLine("Group " + (i + 1).ToString() + ":");
                    Console.WriteLine(fileGroup.ToString());
                    i++;
                }
            }
        }
    }
}
