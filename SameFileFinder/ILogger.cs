using System;

namespace SameFileFinder
{
    public interface ILogger
    {
        void Write(Exception e);
        void Write(string message);
    }
}
