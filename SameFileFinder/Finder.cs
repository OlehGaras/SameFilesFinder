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
        public Dictionary<string, FileGroup> FindGroupOfSameFiles(string path, Logger logger, FileManager fileManager)
        {
            var files = fileManager.DirSearch(path, logger);
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

        public string HashTheFile(string path, Logger logger)
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

        public Int32 HashTheFile2(string path, Logger logger)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            try
            {
                var f1 = new FileStream(path, FileMode.Open);
                byte[] hash1 = ha.ComputeHash(f1);
                f1.Close();
                return BitConverter.ToInt32(hash1, 0);
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
            return 0;
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
        public List<FileGroup> FindGroupOfSameFiles(string path, FileManager fileManager, Logger logger)
        {
            var files = fileManager.DirSearch(path, logger);

            if (files.Count != 0)
            {
                files.Sort((file1, file2) => file1.Length.CompareTo(file2.Length));

                var uncheckedGroups = FormTheGroupsWithSameLength(files);
                var checkedGroups = new List<IFileGroup>();
                foreach (var group in uncheckedGroups)
                {
                    var chechedGroup = CheckTheGroup(group, logger);
                    if (chechedGroup != null)
                        checkedGroups.AddRange(chechedGroup);
                }

                var RESULT = new List<FileGroup>();
                foreach (var group in checkedGroups)
                {
                    var chechedGroup = CheckTheGroup(group, logger);
                    if (chechedGroup != null)
                        RESULT.AddRange(chechedGroup);
                }
                return RESULT;
            }
            return null;
        }

        public List<FileGroup> FormTheGroupsWithSameHash(List<FileInfo> files, List<Int32> hashedPathes, Logger logger)
        {
            var groups = new List<FileGroup>();
            var gr = new FileGroup();

            Int64 current = hashedPathes[0];

            for (int i = 0; i < files.Count; i++)
            {
                if (hashedPathes[i] > current)
                {
                    if (gr.Group.Count > 1)
                        groups.Add(gr);
                    gr = new FileGroup();
                    current = hashedPathes[i];
                }
                gr.Add(files[i]);
            }
            if (gr.Group.Count > 1)
                groups.Add(gr);
            return groups;
        }
        public List<FileGroup> FormTheGroupsWithSameLength(List<FileInfo> files)
        {
            var groups = new List<FileGroup>();
            var gr = new FileGroup();

            long currentLength = files[0].Length;

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Length > currentLength)
                {
                    if (gr.Group.Count > 1)
                        groups.Add(gr);
                    gr = new FileGroup();
                    currentLength = files[i].Length;
                }
                gr.Add(files[i]);
            }
            groups.Add(gr);
            return groups;
        }

        public List<FileGroup> CheckTheGroup(IFileGroup group, Logger logger)
        {
            var resultList = new List<FileGroup>();
            var r = new List<FileInfo>();
            var pathes = new List<string>();
            var gr = (FileGroup)group;

            if (gr.Group.Count == 1)
            {
                return null;
            }

            foreach (var file in gr.Group)
            {
                r.Add(file);
            }
            var hashedPathes = new List<Int32>();

            foreach (var file in gr.Group)
            {
                try
                {
                    hashedPathes.Add(HashTheFile2(file.DirectoryName + @"\" + file.Name, logger));
                }
                catch (Exception e)
                {
                    logger.Write(e);
                }
            }

            r.Sort((file1, file2) =>
                {
                    return
                        HashTheFile2(file1.DirectoryName + @"\" + file1.Name, logger)
                            .CompareTo(HashTheFile2(file2.DirectoryName + @"\" + file2.Name, logger));
                });

            //we have the groups of files with same length and same hash
            resultList = FormTheGroupsWithSameHash(r, r.ConvertAll((file) => HashTheFile2(file.DirectoryName + @"\" + file.Name, logger)), logger);


            var RESULT = new List<FileGroup>();
            foreach (var res in resultList)
            {
                RESULT.AddRange(CheckTheGroup2(res, logger));
            }

            //now we can to compare files in these groups as a byte[] arrays

            return RESULT;
        }

        public List<FileGroup> CheckTheGroup2(IFileGroup group, Logger logger)
        {
            List<byte[]> filesInBytes = new List<byte[]>();

            var resultList = new List<FileGroup>();
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
                    catch (Exception e)
                    {
                        logger.Write(e);
                    }
                }
            }
            byte[] currentValue = new byte[1];
            for (int i = 0; i < filesInBytes.Count; i++)
            {
                if (!filesInBytes[i].SequenceEqual(currentValue))
                {
                    currentValue = filesInBytes[i];
                    if (result.Group.Count > 1)
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
            if (result.Group.Count > 1)
                resultList.Add(result);
            return resultList;
        }
    }
}