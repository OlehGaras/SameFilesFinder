using System;
using NUnit.Framework;
using SameFileFinder;

namespace SameFileFinderTests
{
    [TestFixture]
    public class FileInfoTests:BaseTests
    {
        [Test]
        public void ArgumentConstructor_NullValues_ThrowsArgumentNullException_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new FileInfo(null, 0, null, null));
        }
    }
}
