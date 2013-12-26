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

        public List<FileInfo> DirSearch(string dir, ILogger logger)
        {
            var files = new List<FileInfo>();
            try
            {
                var di = new DirectoryInfo(dir);
                var filesInCurrentDirectory = di.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList();
                var directoriesInCurrentDirectory =
                    di.GetDirectories("*.*", SearchOption.TopDirectoryOnly).ToList();
                foreach (System.IO.FileInfo f in filesInCurrentDirectory)
                    files.Add(new FileInfo(f, null));
                foreach (DirectoryInfo d in directoriesInCurrentDirectory)
                {
                    files.AddRange(DirSearch(d.FullName, logger));
                }
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
            return files;
        }

        //public bool ByteCompare(FileInfo file1, FileInfo file2, ILogger logger)
        public bool ByteCompare(FileInfo file1, FileInfo file2, ILogger logger)
        {
            if (file1 == null || file2 == null)
                return false;
            try
            {
                FileStream firstFile = new FileStream(file1.Path,
                                                      FileMode.Open, FileAccess.Read),
                           secondFile = new FileStream(file2.Path,
                                                       FileMode.Open, FileAccess.Read);
                byte[] byte1 = new byte[4096];
                byte[] byte2 = new byte[4096];

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

        public bool ByteByByteCompare(byte[] array1, byte[] array2)
        {
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
