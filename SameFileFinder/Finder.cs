using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SameFileFinder
{
    public class Finder : IFinder
    {
        public string HashTheFile(string path, ILogger logger)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            try
            {
                var aggregateException = new AggregateException(new FileNotFoundException(), new FormatException(), new EventLogReadingException());

                throw aggregateException;
                var f1 = new FileStream(path, FileMode.Open);
                byte[] hash1 = ha.ComputeHash(f1);
                f1.Close();          
                return BitConverter.ToString(hash1);
            }
            catch (Exception e)
            {
                logger.Write(e);
            }
            return string.Empty;
        }

        public List<FileGroup> FindGroupOfSameFiles(string path, ILogger logger, IFileManager fileManager)
        {
            var files = fileManager.DirSearch(path, logger);
            if (files.Count == 0)
                return null;
            files.Sort((file1, file2) => file1.Information.Length.CompareTo(file2.Information.Length));

            var groupsWithSameLength = FormTheGroup(files, f => f.Information.Length);
            var checkedGroups = new List<FileGroup>();

            var query = groupsWithSameLength.AsParallel().Select(group => CheckTheGroup(group, logger, fileManager));
            foreach (var group in query)
            {
                if (group != null)
                    checkedGroups.AddRange(group);
            }

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

        public List<FileGroup> CheckTheGroup(IFileGroup group, ILogger logger,IFileManager manager)
        {
            var resultList = new List<FileGroup>();
            var files = group.Group;

            if (group.Group.Count == 1)
            {
                return null;
            }

            for (int i = 0; i < files.Count; i++)
            {
                files[i].Hash = HashTheFile(files[i].Information.DirectoryName + @"\" + files[i].Information.Name, logger);
            }

            files.Sort((file1, file2) => file1.Hash.CompareTo(file2.Hash));

            var groupsWithSameHash = FormTheGroup(files, file => file.Hash);

            var query = groupsWithSameHash.AsParallel().Select(gr => CompareFiles(gr, logger,manager));
            foreach (var gr in query)
            {
                resultList.AddRange(gr);
            }

            return resultList;
        }

        public List<FileGroup> CompareFiles(IFileGroup group, ILogger logger,IFileManager manager)
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
                if (!manager.ByteCompare(files[i], currentValue, logger))
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
                    if (manager.ByteCompare(files[i], files[j], logger))
                    {
                        result.Add(files[j]);
                    }
                }
            }
            if (result.Group.Count > 1)
                resultList.Add(result);
            return resultList;
        }
        
    }
}