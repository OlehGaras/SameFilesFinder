using System;
using SameFileFinder;

namespace ConsoleSameFileFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            //byte[] mas = new byte[40];
            //for (int i = 0; i < 38; i++)
            //{
            //    mas[i] = 0;
            //}
            //mas[38] = 1;
            //mas[39] = 1;
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(mas);

            //Int64 b = BitConverter.ToInt64(mas, 0);

            //Int64 a = BitConverter.ToInt64(mas, 0);


            var logger = new SameFileFinder.Logger("log.txt");
            var manager = new ConsoleManager(args, logger);
            FileManager fileManager = new FileManager();
            Finder finder = new Finder();
            finder.FindGroupOfSameFiles("D:\\folder", fileManager, logger);
            manager.Execute(fileManager,finder);        
        }
    }
}
