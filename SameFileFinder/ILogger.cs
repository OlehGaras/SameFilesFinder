using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SameFileFinder
{
    public interface ILogger
    {
        void AppStart();
        void WriteToFile(Exception e);
        void AppAbort();
        void AppEnd();
    }
}
