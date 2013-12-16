using System;
using System.Collections.Concurrent;
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
        public Dictionary<string,FileGroup> FindGroupOfSameFiles(string path,Logger logger)
        {
            var fileManager = new FileManager();
            fileManager.DirSearch(path,logger);
            var pathes = new List<string>();
            foreach (var file in fileManager.Files)
            {
                try
                {
                    pathes.Add(file.DirectoryName + @"\" + file.Name);
                }
                catch (Exception e)
                {
                    logger.WritetoFile(e);
                }
            }

            var groups = new Dictionary<string, FileGroup>();
            for (int i = 0; i < pathes.Count; i++)
            {
                string hash = HashTheFile(pathes[i],logger);
                if (hash != null)
                {
                    if (!groups.ContainsKey(hash))
                    {
                        groups.Add(hash, new FileGroup());
                    }
                    groups[hash].Add(fileManager.Files[i]);
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
                logger.WritetoFile(e);
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
    }
}