using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Cryptography;
using System.Collections;
using System.Threading.Tasks;

namespace SameFileFinder
{
    public class Finder : IFinder
    {
        public Int64 HashTheFile(string path, ILogger logger)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            try
            {
                var f1 = new FileStream(path, FileMode.Open);
                byte[] hash1 = ha.ComputeHash(f1);
                f1.Close();
                return BitConverter.ToInt64(hash1, 0);
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
            return 0;
        }

        public List<FileGroup> FindGroupOfSameFiles(string path, ILogger logger, IFileManager fileManager)
        {
            var files = fileManager.DirSearch(path, logger);
            if (files.Count == 0)
                return null;

            files.Sort((file1, file2) => file1.Length.CompareTo(file2.Length));

            var groupsWithSameLength = FormTheGroupsWithSameLength(files);
            var checkedGroups = new List<FileGroup>();


            foreach (var group in groupsWithSameLength)
            {
                var chechedGroup = CheckTheGroup(group, logger);
                if (chechedGroup != null)
                    checkedGroups.AddRange(chechedGroup);
            }
            return checkedGroups;
        }

        public List<FileGroup> FormTheGroupsWithSameHash(List<FileInfo> files, List<Int64> hashedFiles)
        {
            var groups = new List<FileGroup>();
            var gr = new FileGroup();

            Int64 current = hashedFiles[0];
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

        public List<FileGroup> CheckTheGroup(IFileGroup group, ILogger logger)
        {
            var resultList = new List<FileGroup>();
            var files = group.Group;

            if (group.Group.Count == 1)
            {
                return null;
            }

            files.Sort((file1, file2) =>
            {
                return
                    HashTheFile(file1.DirectoryName + @"\" + file1.Name, logger)
                        .CompareTo(HashTheFile(file2.DirectoryName + @"\" + file2.Name, logger));
            });


            var groupsWithSameHash = FormTheGroupsWithSameHash(files, files.ConvertAll((file) => HashTheFile(file.DirectoryName + @"\" + file.Name, logger)));
            
            foreach (var gr in groupsWithSameHash)
            {
                resultList.AddRange(CompareFiles(gr, logger));
            }
            return resultList;
        }

        public List<FileGroup> CompareFiles(IFileGroup group, ILogger logger)
        {
            if (group.Group.Count == 1)
            {
                return null;
            }

            var filesInBytes = new List<byte[]>();
            string pathToCurrent = "";
            foreach (var file in group.Group)
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

            var resultList = new List<FileGroup>();
            var result = new FileGroup();
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
                result.Add(group.Group[i]);
                for (int j = i + 1; j < filesInBytes.Count; j++)
                {
                    if (filesInBytes[i].SequenceEqual(filesInBytes[j]))
                    {
                        result.Add(group.Group[j]);
                    }
                }
            }
            if (result.Group.Count > 1)
                resultList.Add(result);
            return resultList;
        }
    }
}