using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;


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
            files.Sort((file1, file2) => file1.Information.Length.CompareTo(file2.Information.Length));

            var groupsWithSameLength = FormTheGroup(files, f => f.Information.Length);
            var checkedGroups = new List<FileGroup>();

            var query = groupsWithSameLength.AsParallel().Select(group => CheckTheGroup(group, logger));
            foreach (var group in query)
            {
                if (group != null)
                    checkedGroups.AddRange(group);
            }

            //foreach (var group in groupsWithSameLength)
            //{
            //    var chechedGroup = CheckTheGroup(group, logger);
            //    if (chechedGroup != null)
            //        checkedGroups.AddRange(chechedGroup);
            //}
            return checkedGroups;
        }

        public List<FileGroup> FormTheGroup<T>(List<MyFileInfo> files, Func<MyFileInfo, T> selector)
            where T : IComparable<T>
        {
            var groups = new List<FileGroup>();
            var gr = new FileGroup();

            T currentValue = selector(files[0]);
            for (int i = 0; i < files.Count; i++)
            {
                if (selector(files[i]).CompareTo(currentValue) != 0)
                {
                    if (gr.Group.Count > 1)
                        groups.Add(gr);
                    gr = new FileGroup();
                    currentValue = selector(files[i]);
                }
                gr.Add(files[i]);
            }
            if (gr.Group.Count > 1)
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

            //var hash = files.AsParallel().AsOrdered().Select(file => HashTheFile(file.Information.DirectoryName + @"\" + file.Information.Name, logger)).ToArray();

            //for (int i = 0; i < files.Count; i++)
            //{
            //    files[i].Hash = hash[i];
            //}
            for (int i = 0; i < files.Count; i++)
            {
                files[i].Hash = HashTheFile(files[i].Information.DirectoryName + @"\" + files[i].Information.Name, logger);
            }

            files.Sort((file1, file2) =>
            {
                return file1.Hash.CompareTo(file2.Hash);
            });


            var groupsWithSameHash = FormTheGroup(files, file => file.Hash);

            var query = groupsWithSameHash.AsParallel().Select(gr => CompareFiles(gr, logger));
            foreach (var gr in query)
            {
                resultList.AddRange(gr);
            }

            //foreach (var gr in groupsWithSameHash)
            //{
            //    resultList.AddRange(CompareFiles(gr, logger));
            //}
            return resultList;
        }

        public List<FileGroup> CompareFiles(IFileGroup group, ILogger logger)
        {
            if (group.Group.Count == 1)
            {
                return null;
            }

            var files = group.Group;
            MyFileInfo currentValue = null;
            var resultList = new List<FileGroup>();
            var result = new FileGroup();
            for (int i = 0; i < files.Count; i++)
            {
                if (!ByteCompare(files[i], currentValue, logger))
                {
                    currentValue = files[i];
                    if (result.Group.Count > 1)
                        resultList.Add(result);
                    result = new FileGroup();
                }
                else
                {
                    continue;
                }
                result.Add(files[i]);
                for (int j = i + 1; j < group.Group.Count; j++)
                {
                    if (ByteCompare(files[i], files[j], logger))
                    {
                        result.Add(files[j]);
                    }
                }
            }
            if (result.Group.Count > 1)
                resultList.Add(result);
            return resultList;
        }
        private bool ByteCompare(MyFileInfo file1, MyFileInfo file2, ILogger logger)
        {
            if (file1 == null || file2 == null)
                return false;
            try
            {
                FileStream firstFile = new FileStream(file1.Information.DirectoryName + @"\" + file1.Information.Name,
                                                      FileMode.Open, FileAccess.Read),
                           secondFile = new FileStream(file2.Information.DirectoryName + @"\" + file2.Information.Name,
                                                       FileMode.Open, FileAccess.Read);
                byte[] byte1 = new byte[4096];
                byte[] byte2 = new byte[4096];

                int res1, res2;
                do
                {
                    res1 = firstFile.Read(byte1, 0, byte1.Length);
                    res2 = secondFile.Read(byte2, 0, byte2.Length);

                    for (int i = 0; i < byte1.Length; i++)
                    {
                        if (byte1[i] != byte2[i])
                        {
                            return false;
                        }

                    }
                }
                while (res1 != 0 && res2 != 0);

                return true;
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
            return false;
        }
    }
}