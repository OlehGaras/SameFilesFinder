using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Cryptography;
using System.Collections;

namespace SameFileFinder
{
    public class Finder : IFinder
    {
        public Dictionary<string,FileGroup> FindGroupOfSameFiles(string path,Logger logger, FileManager fileManager)
        {
            //var fileManager = new FileManager();
            var files = fileManager.DirSearch(path,logger);
            var pathes = new List<string>();
            foreach (var file in files)
            {
                try
                {
                    pathes.Add(file.DirectoryName + @"\" + file.Name);
                }
                catch (Exception e)
                {
                    logger.Write(e);
                }
            }

            var groups = new Dictionary<string, FileGroup>();
            for (int i = 0; i < pathes.Count; i++)
            {
                string hash = HashTheFile(pathes[i], logger);
                if (hash != null)
                {
                    if (!groups.ContainsKey(hash))
                    {
                        groups.Add(hash, new FileGroup());
                    }
                    groups[hash].Add(files[i]);
                }
            }
            
            return groups;
        }

        public string HashTheFile(string path,Logger logger)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            try
            {
                var f1 = new FileStream(path, FileMode.Open);
                byte[] hash1 = ha.ComputeHash(f1);
                f1.Close();
                return BitConverter.ToString(hash1);
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
            return null;
        }

        public bool FileComparer(string path1, string path2)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            var f1 = new FileStream(path1, FileMode.Open);
            var f2 = new FileStream(path2, FileMode.Open);
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
        public List<IFileGroup> FindGroupOfSameFiles(string path,FileManager fileManager, Logger logger)
        {
            //List<FileInfo> files = new List<FileInfo>();
            var files = fileManager.DirSearch(path,logger);

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
                    var chechedGroup = CheckTheGroup2(group,logger);
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

        public FileGroup CheckTheGroup2(IFileGroup group,Logger logger)
        {
            var resultList = new List<FileGroup>();
            List<string> pathes = new List<string>();
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
                    if (currHash != HashTheFile(pathes[i],logger))
                    {
                        currHash = HashTheFile(pathes[i],logger);
                        if (result.Group.Count > 1)
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
                        if (HashTheFile(pathes[i],logger) == HashTheFile(pathes[j],logger))
                        {
                            result.Group.Add(gr.Group[j]);
                        }
                    }
                }
                if (result.Group.Count > 1)
                    resultList.Add(result);
            }
            return null;
        }
    }
}