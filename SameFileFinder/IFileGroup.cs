﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SameFileFinder
{
    public interface IFileGroup
    {
        List<FileInfo> Group { get; set; }
    }
}
