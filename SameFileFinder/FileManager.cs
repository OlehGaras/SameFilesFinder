using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;

namespace SameFileFinder
{
    [Export(typeof(IFileManager))]
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
                var directoriesInCurrentDirectory = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly).ToList();
                foreach (var f in filesInCurrentDirectory)
                {
                    files.Add(new FileInfo(f.DirectoryName + @"\" + f.Name, f.Length, f.Name, null));
                }
                foreach (var d in directoriesInCurrentDirectory)
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

        public bool ByteCompare(FileInfo file1, FileInfo file2, ILogger logger,CancellationToken t)
        {
            if (file1 == null || file2 == null)
                return false;
            try
            {
                var firstFile = new FileStream(file1.Path, FileMode.Open, FileAccess.Read);
                var secondFile = new FileStream(file2.Path, FileMode.Open, FileAccess.Read);

                var byte1 = new byte[ChunkSize];
                var byte2 = new byte[ChunkSize];

                var res1 = firstFile.Read(byte1, 0, byte1.Length);
                var res2 = secondFile.Read(byte2, 0, byte2.Length);
                while (res1 != 0 && res2 != 0)
                {
                    if (!ByteByByteCompare(byte1, byte2) || t.IsCancellationRequested)
                    {
                        firstFile.Close();
                        secondFile.Close();
                        return false;
                    }
                    res1 = firstFile.Read(byte1, 0, byte1.Length);
                    res2 = secondFile.Read(byte2, 0, byte2.Length);
                }

                firstFile.Close();
                secondFile.Close();
                return true;
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
            return false;
        }

        public bool ByteByByteCompare(byte[] first, byte[] second)
        {
            if (first == null || second == null) 
                throw new ArgumentNullException();

            if (first.Length != second.Length)
                return false;

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
