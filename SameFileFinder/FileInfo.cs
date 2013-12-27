namespace SameFileFinder
{
    public class FileInfo
    {
        public string Path { get; private set; }
        public long Length { get; private set; }
        public string Name { get; private set; }
        public string Hash { get; set; }

        public FileInfo(string path,long length,string name, string hash)
        {
            Path = path;
            Length = length;
            Name = name;
            Hash = hash;        
        }   
    }
}
