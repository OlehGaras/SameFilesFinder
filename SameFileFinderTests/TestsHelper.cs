using System;
using System.IO;
using System.Text;

namespace SameFileFinderTests
{
    public static class TestsHelper
    {
        public static string TestsFolderPath = string.Format(@"{0}\Tests", Path.GetTempPath());

        private static void EnsureTestDirectoryExists()
        {
            if (!Directory.Exists(TestsFolderPath))
            {
                Directory.CreateDirectory(TestsFolderPath);
            }
        }

        public static string CreateTestFile(string fileName, string content)
        {
            EnsureTestDirectoryExists();
            var filePath = TestsFolderPath + @"\" + fileName;

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                File.WriteAllText(filePath, content);
            }

            Console.WriteLine(string.Format(@"Creating file: {0}\{1}",TestsFolderPath, fileName));

            return filePath;
        }

        public static string CreateTestFile(string fileName, byte[] content)
        {
            var strContent = new StringBuilder();
            foreach (var b in content)
            {
                strContent.Append(b);
            }
            return CreateTestFile(fileName, strContent.ToString());          
        }

        public static void CleanUpTestDirectory(string path)
        {
            EnsureTestDirectoryExists();
            var dir = new DirectoryInfo(path);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.IsReadOnly = false;
                fi.Delete();
            }

            foreach (var di in dir.GetDirectories())
            {
                CleanUpTestDirectory(di.FullName);
                di.Delete();
            }
        }
    }
}
