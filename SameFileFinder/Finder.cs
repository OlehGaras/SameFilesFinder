using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace SameFileFinder
{
    public class Finder : IFinder
    {
        void DirSearch(string dir,List<FileInfo>files )
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
                    DirSearch(d.FullName,files);
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
            DirSearch(path,files);

            List<IFileGroup> checkedGroups = null;
            
            //DirectoryInfo di = new DirectoryInfo(path);
            //FileInfo[] files = di.GetFiles("*.*", SearchOption.AllDirectories);
            if (files.Count != 0)
            {
                files.Sort((file1, file2) => file1.Length.CompareTo(file2.Length));

                var uncheckedGroups = FormTheGroups(files);
                checkedGroups = new List<IFileGroup>();
                foreach (var group in uncheckedGroups)
                {
                    var chechedGroup = CheckTheGroup(group);
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
    }
}
