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
        public List<MyFileInfo> DirSearch(string dir,ILogger logger)
        {
            var files = new List<MyFileInfo>();
            try
            {
                var di = new DirectoryInfo(dir);
                var filesInCurrentDirectory = di.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList();
                var directoriesInCurrentDirectory =
                    di.GetDirectories("*.*", SearchOption.TopDirectoryOnly).ToList();
                foreach (FileInfo f in filesInCurrentDirectory)
                    files.Add(new MyFileInfo(f,0));
                foreach (DirectoryInfo d in directoriesInCurrentDirectory)
                {
                    files.AddRange(DirSearch(d.FullName,logger));
                }
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
            return files;
        }
    }
}
