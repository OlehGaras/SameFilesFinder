using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;


namespace SameFileFinder
{
    [Export (typeof(IFinder))]
    public class Finder : IFinder
    {
        public string HashTheFile(string path, ILogger logger)
        {
            var ha = HashAlgorithm.Create();
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
            return string.Empty;
        }

        public List<FileGroup> FindGroupOfSameFiles(string path, ILogger logger, IFileManager fileManager, CancellationToken e)
        {
            var files = fileManager.DirSearch(path, logger);
            if (files.Count == 0)
                return null;
            files.Sort((file1, file2) => file1.Length.CompareTo(file2.Length));

            var groupsWithSameLength = FormTheGroup(files, f => f.Length);
            var checkedGroups = new List<FileGroup>();
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var query = groupsWithSameLength.Select(group => CheckTheGroup(group, logger, fileManager,e));
            foreach (var group in query)
            {
                if (e.IsCancellationRequested)
                    return new List<FileGroup>();
                if (group != null)
                    checkedGroups.AddRange(group);
            }
            stopWatch.Stop();

            Console.WriteLine(string.Format("Runtime:  {0}", TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds)));
            return checkedGroups;
        }

        public List<FileGroup> FormTheGroup<T>(List<FileInfo> files, Func<FileInfo, T> selector)
            where T : IComparable<T>
        {
            var groups = new List<FileGroup>();
            var gr = new FileGroup();

            T currentValue = selector(files[0]);
            for (int i = 0; i < files.Count; i++)
            {
                if (selector(files[i]).CompareTo(currentValue) != 0)
                {
                    if (gr.Files.Count > 1)
                        groups.Add(gr);
                    gr = new FileGroup();
                    currentValue = selector(files[i]);
                }
                gr.Add(files[i]);
            }
            if (gr.Files.Count > 1)
                groups.Add(gr);
            return groups;
        }

        public List<FileGroup> CheckTheGroup(IFileGroup group, ILogger logger, IFileManager manager,CancellationToken e)
        {
            var resultList = new List<FileGroup>();
            var files = group.Files;

            if (files.Count == 1)
            {
                return null;
            }

            for (int i = 0; i < files.Count; i++)
            {
                if (e.IsCancellationRequested)
                    return new List<FileGroup>();
                files[i].Hash = HashTheFile(files[i].Path, logger);
            }

            files.Sort((file1, file2) => String.Compare(file1.Hash, file2.Hash, StringComparison.Ordinal));

            var groupsWithSameHash = FormTheGroup(files, file => file.Hash);

            var query = groupsWithSameHash.Select(gr => CompareFiles(gr, logger, manager,e));
            foreach (var gr in query)
            {
                if (e.IsCancellationRequested)
                    return new List<FileGroup>();
                resultList.AddRange(gr);
            }

            return resultList;
        }

        public List<FileGroup> CompareFiles(IFileGroup group, ILogger logger, IFileManager manager, CancellationToken e)
        {
            if (group.Files.Count < 2)
            {
                return null;
            }
            var resultList = new List<FileGroup>();
            var set = new HashSet<FileInfo>(group.Files);
            while (set.Count != 0)
            {
                var enumerator = set.GetEnumerator();
                var result = new FileGroup();

                enumerator.MoveNext();
                var curr = enumerator.Current;
                result.Add(curr);

                while (enumerator.MoveNext())
                {
                    if (manager.ByteCompare(curr, enumerator.Current, logger,e))
                    {
                        result.Add(enumerator.Current);
                    }
                    if (e.IsCancellationRequested)
                        return new List<FileGroup>();
                }

                set.RemoveWhere(file => result.Files.Contains(file));
                enumerator.Dispose();
                if (result.Files.Count > 1)
                    resultList.Add(result);               
            }
            return resultList;
        }

    }
}