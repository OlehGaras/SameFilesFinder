using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SameFileFinder
{
    public class FileManager : IFileManager
    {
        public List<FileInfo> Files { get; set; }

        public FileManager()
        {
            Files = new List<FileInfo>();
        }

        public void DirSearch(string dir,Logger logger)
        {
            try
            {
                var di = new DirectoryInfo(dir);
                var filesInCurrentDirectory = di.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList();
                var directoriesInCurrentDirectory =
                    di.GetDirectories("*.*", SearchOption.TopDirectoryOnly).ToList();
                foreach (FileInfo f in filesInCurrentDirectory)
                    Files.Add(f);
                foreach (DirectoryInfo d in directoriesInCurrentDirectory)
                {
                    DirSearch(d.FullName,logger);
                }
            }
            catch (Exception e)
            {
                logger.WritetoFile(e);
            }
        }
    }
}
