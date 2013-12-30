using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using SameFileFinder;

namespace SameFileFinderTests
{
    [TestFixture]
    public class LoggerTests : BaseTests
    {
        [Test]
        public void ArgumentConstructor_NullValues_ThrowsArgumentNullException_Test()
        {
            Assert.Throws<ArgumentNullException>(()=>new Logger(null, null));
        }

        [Test]
        public void ArgumentConstructor_FilePath_FileExactlyExists_Test()
        {
            var logger = new Logger("", "log.txt");
            Assert.IsTrue(File.Exists(logger.Path));
        }

        [Test]
        public void ArgumentConstructor_InCorrectFilePath_UsesTheDefaultPath_Test()
        {
            var logger = new Logger("???", "log.txt");
            var year = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
            var month = DateTime.Now.Month.ToString(CultureInfo.InvariantCulture);
            var day = DateTime.Now.Day.ToString(CultureInfo.InvariantCulture);

            Assert.IsTrue(logger.Path == year + @"\" + month + @"\" + day + @"\log.txt");
        }

        [Test]
        public void Exception_NewAggregateException_ReturnsNonEmptyMessage_Test()
        {
            var e = new AggregateException(new OutOfMemoryException(),new OutOfMemoryException());
            TestsHelper.CreateTestFile("log.txt", string.Empty);
            var logger = new Logger(TestsHelper.TestsFolderPath,"log.txt");
            var res = logger.Exception(e);
            Assert.IsTrue(res != "");
        }

        [Test]
        public void FixPathToFolder_EmptyPath_ReturnsEmptyPath_Test()
        {
            var logger = new Logger("", "log.txt");
            Assert.IsTrue(logger.FixPathToFolder("") == "");
        }

        [Test]
        public void FixPathToFolder_NonEmptyPath_ReturnsPathWithSlash_Test()
        {
            var logger = new Logger("", "log.txt");
            Assert.IsTrue(logger.FixPathToFolder("qwerty") == @"qwerty\");
        }
    }
}
