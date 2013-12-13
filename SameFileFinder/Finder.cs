using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace SameFileFinder
{
    public class Finder : IFinder
    {
        List<FileGroup> resultList = new List<FileGroup>();
        //make a class
        void DirSearch(string dir, List<FileInfo> files)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                List<FileInfo> filesInCurrentDirectory = di.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList();
                List<DirectoryInfo> directoriesInCurrentDirectory = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly).ToList();
                foreach (FileInfo f in filesInCurrentDirectory)
                    files.Add(f);
                foreach (DirectoryInfo d in directoriesInCurrentDirectory)
                {
                    DirSearch(d.FullName, files);
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (SecurityException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public List<IFileGroup> FindGroupOfSameFiles(string path)
        {
            List<FileInfo> files = new List<FileInfo>();
            DirSearch(path, files);

            var checkedGroups = new List<IFileGroup>();

            //DirectoryInfo di = new DirectoryInfo(path);
            //FileInfo[] files = di.GetFiles("*.*", SearchOption.AllDirectories);
            if (files.Count != 0)
            {
                files.Sort((file1, file2) => file1.Length.CompareTo(file2.Length));

                var uncheckedGroups = FormTheGroups(files);
                checkedGroups = new List<IFileGroup>();
                foreach (var group in uncheckedGroups)
                {
                    var chechedGroup = CheckTheGroup2(group);
                    if (chechedGroup != null)
                        checkedGroups.Add(chechedGroup);
                }
            }
            return checkedGroups;
        }

        public List<FileGroup> FormTheGroups(List<FileInfo> files)
        {
            List<FileGroup> groups = new List<FileGroup>();

            FileGroup gr = new FileGroup();

            long currentLength = files[0].Length;

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Length > currentLength)
                {
                    groups.Add(gr);
                    gr = new FileGroup();
                    currentLength = files[i].Length;
                }
                gr.Group.Add(files[i]);
            }
            groups.Add(gr);
            return groups;
        }

        public FileGroup CheckTheGroup(IFileGroup group)
        {
            List<byte[]> filesInBytes = new List<byte[]>();
            
            var result = new FileGroup();
            var gr = (FileGroup)group;
            if (gr.Group.Count == 1)
            {
                return null;
            }
            else
            {
                string pathToCurrent = "";

                foreach (var file in gr.Group)
                {
                    pathToCurrent = file.DirectoryName + @"\" + file.Name;
                    try
                    {
                        filesInBytes.Add(File.ReadAllBytes(pathToCurrent));
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (NotSupportedException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (SecurityException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
            }
            byte[] currentValue = new byte[1];
            for (int i = 0; i < filesInBytes.Count; i++)
            {
                if (!filesInBytes[i].SequenceEqual(currentValue))
                {
                    currentValue = filesInBytes[i];
                    if (result.Group.Count != 0)
                        resultList.Add(result);
                    result = new FileGroup();
                }
                else
                {
                    continue;
                }
                result.Group.Add(gr.Group[i]);
                for (int j = i + 1; j < filesInBytes.Count; j++)
                {
                    if (filesInBytes[i].SequenceEqual(filesInBytes[j]))
                    {
                        result.Group.Add(gr.Group[j]);
                    }
                }
            }
            resultList.Add(result);
            return result;
        }

        public FileGroup CheckTheGroup2(IFileGroup group)
        {
            List<string>pathes = new List<string>();
            var gr = (FileGroup)group;
            if (gr.Group.Count == 1)
            {
                return null;
            }
            else
            {
                foreach (var file in gr.Group)
                {
                    try
                    {
                        pathes.Add(file.DirectoryName + @"\" + file.Name);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (NotSupportedException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (SecurityException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                string currHash = string.Empty;
                var result = new FileGroup();
                for (int i = 0; i < pathes.Count; i++)
                {
                    if (currHash != HashTheFile(pathes[i]))
                    {
                        currHash = HashTheFile(pathes[i]);
                        if (result.Group.Count != 0)
                            resultList.Add(result);
                        result = new FileGroup();
                    }
                    else
                    {
                        continue;
                    }
                    result.Group.Add(gr.Group[i]);
                    for (int j = i + 1; j < pathes.Count; j++)
                    {
                        if (HashTheFile(pathes[i])==HashTheFile(pathes[j]))
                        {
                            result.Group.Add(gr.Group[j]);
                        }
                    }
                }
                resultList.Add(result);
            }
            return null;
        }

        public string HashTheFile(string path)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            FileStream f1 = new FileStream(path, FileMode.Open);
            byte[] hash1 = ha.ComputeHash(f1);
            f1.Close();
            return BitConverter.ToString(hash1);
        }

        public bool FileComparer(string path1, string path2)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            FileStream f1 = new FileStream(path1, FileMode.Open);
            FileStream f2 = new FileStream(path2, FileMode.Open);
            /* Calculate Hash */
            byte[] hash1 = ha.ComputeHash(f1);
            byte[] hash2 = ha.ComputeHash(f2);
            f1.Close();
            f2.Close();
            /* Show Hash in TextBoxes */
            string sHash1 = BitConverter.ToString(hash1);
            string sHash2 = BitConverter.ToString(hash2);
            /* Compare the hash and Show Message box */
            if (sHash1 == sHash2)
            {
                return true;
            }
            
            return false;
        }
    }
}


//if(txtFile1.Text != "" && txtFile2.Text !="")
//  {
//      HashAlgorithm ha = HashAlgorithm.Create();
//      FileStream f1 = new FileStream(txtFile1.Text, FileMode.Open);
//      FileStream f2 = new FileStream(txtFile2.Text, FileMode.Open);
//      /* Calculate Hash */
//      byte[] hash1 = ha.ComputeHash(f1);
//      byte[] hash2 = ha.ComputeHash(f2);
//      f1.Close();
//      f2.Close();
//      /* Show Hash in TextBoxes */
//      txtHash1.Text = BitConverter.ToString(hash1);
//      txtHash2.Text = BitConverter.ToString(hash2);
//      /* Compare the hash and Show Message box */
//      if (txtHash1.Text == txtHash2.Text)
//      {
//          MessageBox.Show("Files are Equal !");
//      }
//      else
//      {
//          MessageBox.Show("Files are Diffrent !");
//      }
//  }