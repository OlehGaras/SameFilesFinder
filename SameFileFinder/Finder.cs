using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SameFileFinder
{
    public class Finder : IFinder
    {


        private void Sort(FileInfo[] mas)
        {
            for (int i = 0; i < mas.Length; i++)
            {
                for (int j = 0; j < mas.Length - i - 1; j++)
                {
                    if (mas[j].Length > mas[j+1].Length)
                    {
                        FileInfo temp = mas[j];
                        mas[i] = mas[j+1];
                        mas[j+1] = temp;
                    }
                }
            }
        }

        public IFileGroup FindGroupOfSameFiles(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.*", SearchOption.AllDirectories);

            // string pathToCurrent = "", pathToNext = "";
            // pathToCurrent = files[0].DirectoryName + @"\" + files[0].Name;
            // pathToNext = files[1].DirectoryName + @"\" + files[1].Name;

            // FileCompare(pathToCurrent, pathToNext);

            // byte[] mas1 = File.ReadAllBytes(pathToCurrent);
            // byte[] mas2 = File.ReadAllBytes(pathToNext);
            //// filesInBytes.Add(File.ReadAllBytes(pathToCurrent));

            Sort(files);

            var uncheckedGroups = FormTheGroups(files);
            List<FileGroup> checkedGroups = new List<FileGroup>();
            foreach (var group in uncheckedGroups)
            {
                var chechedGroup = CheckTheGroup(group);
                if(chechedGroup != null)
                checkedGroups.Add(chechedGroup);
            }

            if (checkedGroups.Count != 0)
                for (int i = 0; i < checkedGroups.Count; i++)
                {
                    Console.WriteLine("Group " + (i + 1).ToString() + ":");
                    checkedGroups[i].print();
                }
            else
            {
                Console.WriteLine("There arent same files at this folder");
            }
            return null;
        }

        public List<FileGroup> FormTheGroups(FileInfo[] files)
        {
            List<FileGroup> groups = new List<FileGroup>();

            bool changed = false;
            FileGroup gr = new FileGroup();

            long currentLength = files[0].Length;

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Length > currentLength)
                {
                    groups.Add(gr);
                    gr = new FileGroup();
                    currentLength = files[i].Length;
                }
                gr.m_Group.Add(files[i]);
            }
            groups.Add(gr);
            return groups;
        }

        public FileGroup CheckTheGroup(IFileGroup group)
        {
            List<byte[]> filesInBytes = new List<byte[]>();
            var result = new FileGroup();
            var gr = (FileGroup)group;
            if (gr.m_Group.Count == 1)
            {
                return null;
            }
            else
            {
                string pathToCurrent = "";

                foreach (var file in gr.m_Group)
                {
                    pathToCurrent = file.DirectoryName + @"\" + file.Name;
                    filesInBytes.Add(File.ReadAllBytes(pathToCurrent));
                    //Console.WriteLine(file.Name);
                }
            }
            for (int i = 0; i < filesInBytes.Count; i++)
            {
                for (int j = 0; j < filesInBytes.Count; j++)
                {
                    if (filesInBytes[i].SequenceEqual(filesInBytes[j]))
                    {
                        if (!result.m_Group.Contains(gr.m_Group[j]))
                            result.m_Group.Add(gr.m_Group[j]);
                    }
                }
            }
            return result;
        }

        public void CompareWithAll(byte[] file, List<byte[]> files, FileGroup result)
        {
        }

        private bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

    }
}
