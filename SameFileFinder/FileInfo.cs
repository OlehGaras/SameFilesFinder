namespace SameFileFinder
{
    public class FileInfo
    {
        public string Path { get; private set; }
        public long Length { get; private set; }
        public string Name { get; private set; }
        public string Hash { get; set; }

        public FileInfo(System.IO.FileInfo inform, string hash)
        {
            Path = inform.DirectoryName + @"\" + inform.Name;
            Length = inform.Length;
            Name = inform.Name;
            Hash = hash;        
        }   
    }
}
