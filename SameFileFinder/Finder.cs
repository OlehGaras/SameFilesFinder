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
        public Int32 HashTheFile(string path, Logger logger)
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

        public List<FileGroup> FindGroupOfSameFiles(string path, Logger logger, FileManager fileManager)
        {
            var files = fileManager.DirSearch(path, logger);

            if (files.Count != 0)
            {
                files.Sort((file1, file2) => file1.Length.CompareTo(file2.Length));

                var uncheckedGroups = FormTheGroupsWithSameLength(files);
                var checkedGroups = new List<FileGroup>();
                foreach (var group in uncheckedGroups)
                {
                    var chechedGroup = CheckTheGroup(group, logger);
                    if (chechedGroup != null)
                        checkedGroups.AddRange(chechedGroup);
                }

                var resultGroups = new List<FileGroup>();
                foreach (var group in checkedGroups)
                {
                    var chechedGroup = CheckTheGroup(group, logger);
                    if (chechedGroup != null)
                        resultGroups.AddRange(chechedGroup);
                }
                return resultGroups;
            }
            return null;
        }

        public List<FileGroup> FormTheGroupsWithSameHash(List<FileInfo> files, List<Int32> hashedFiles)
        {
            var groups = new List<FileGroup>();
            var gr = new FileGroup();

            Int32 current = hashedFiles[0];

            for (int i = 0; i < files.Count; i++)
            {
                if (hashedFiles[i] > current)
                {
                    if (gr.Group.Count > 1)
                        groups.Add(gr);
                    gr = new FileGroup();
                    current = hashedFiles[i];
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

        public List<FileGroup> CheckTheGroup(FileGroup group, Logger logger)
        {
            var resultList = new List<FileGroup>();
            var files = group.Group;

            if (group.Group.Count == 1)
            {
                return null;
            }

            var hashedPathes = new List<Int32>();

            foreach (var file in files)
            {
                try
                {
                    hashedPathes.Add(HashTheFile(file.DirectoryName + @"\" + file.Name, logger));
                }
                catch (Exception e)
                {
                    logger.Write(e);
                }
            }

            files.Sort((file1, file2) =>
                {
                    return
                        HashTheFile(file1.DirectoryName + @"\" + file1.Name, logger)
                            .CompareTo(HashTheFile(file2.DirectoryName + @"\" + file2.Name, logger));
                });

            //we have the groups of files with same length and same hash
            var newGroups = FormTheGroupsWithSameHash(files, files.ConvertAll((file) => HashTheFile(file.DirectoryName + @"\" + file.Name, logger)));

            //now we can to compare files in these groups as a byte[] arrays
            foreach (var gr in newGroups)
            {
                resultList.AddRange(CompareFiles(gr, logger));
            }

            return resultList;
        }

        public List<FileGroup> CompareFiles(IFileGroup group, Logger logger)
        {
            List<byte[]> filesInBytes = new List<byte[]>();

            var resultList = new List<FileGroup>();
            var result = new FileGroup();
            var gr = (FileGroup)group;
            if (gr.Group.Count == 1)
            {
                return null;
            }

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