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
        private const int ChunkSize = 4096;

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
                    files.Add(new MyFileInfo(f,null));
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

        public  bool ByteCompare(MyFileInfo file1, MyFileInfo file2, ILogger logger)
        {
            if (file1 == null || file2 == null)
                return false;
            try
            {
                FileStream firstFile = new FileStream(file1.Information.DirectoryName + @"\" + file1.Information.Name,
                                                      FileMode.Open, FileAccess.Read),
                           secondFile = new FileStream(file2.Information.DirectoryName + @"\" + file2.Information.Name,
                                                       FileMode.Open, FileAccess.Read);
                byte[] byte1 = new byte[ChunkSize];
                byte[] byte2 = new byte[ChunkSize];

                int res1, res2;
                do
                {
                    res1 = firstFile.Read(byte1, 0, byte1.Length);
                    res2 = secondFile.Read(byte2, 0, byte2.Length);

                    for (int i = 0; i < byte1.Length; i++)
                    {
                        if (byte1[i] != byte2[i])
                        {
                            return false;
                        }

                    }
                }
                while (res1 != 0 && res2 != 0);

                return true;
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
            return false;
        }
    }
}
