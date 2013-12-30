using System;
using FakeItEasy;
using NUnit.Framework;
using SameFileFinder;
using FileInfo = SameFileFinder.FileInfo;

namespace SameFileFinderTests
{
    [TestFixture]
    public class FileManagerTests : BaseTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            TestsHelper.CleanUpTestDirectory(TestsHelper.TestsFolderPath);
        }

        [Test]
        public void ByteByByteCompare_SimilarArrays_ReturnsTrue_Test()
        {
            //arrange
            var first = new byte[] { 1 };
            var second = new byte[] { 1 };
            var manager = new FileManager();

            //act
            bool res = manager.ByteByByteCompare(first, second);

            //assert
            Assert.IsTrue(res);
        }

        [Test]
        [TestCase(new byte[] { 1 }, new byte[] { 2 })]
        [TestCase(new byte[] { 2 }, new byte[] { 1 })]
        [TestCase(new byte[] { 1 }, new byte[] { })]
        public void ByteByByteCompare_DiferentArrays_ReturnsFalse_Test(byte[] first, byte[] second)
        {
            var manager = new FileManager();
            Assert.IsFalse(manager.ByteByByteCompare(first, second));
        }

        [Test]
        [TestCase(new byte[] { }, null)]
        [TestCase(null, new byte[] { })]
        public void ByteByByteCompare_NullArray_ThrowsArgumentException(byte[] first, byte[] second)
        {
            var manager = new FileManager();

            Assert.Throws<ArgumentNullException>(() => manager.ByteByByteCompare(first, second));
        }

        [Test]
        public void ByteCompare_SameFilePath_ReturnsTrue_Test()
        {
            var filePath = TestsHelper.CreateTestFile("SameFilePathTestFile.txt", "content");

            var logger = A.Fake<ILogger>();
            var manager = new FileManager();

            var file1 = new FileInfo(filePath, 0, "", "");
            var file2 = new FileInfo(filePath, 0, "", "");

            Assert.IsTrue(manager.ByteCompare(file1, file2, logger));
        }

        [Test]
        public void ByteCompare_NonExistentFilePath_WritesToLog_Test()
        {
            var logger = A.Fake<ILogger>();
            var file = A.Fake<FileInfo>();
            var manager = new FileManager();

            manager.ByteCompare(file, file, logger);

            A.CallTo(() => logger.Write(A<Exception>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void ByteCompare_DifferentFilesWithSameLength_ReturnsFalse_Test()
        {
            var manager = new FileManager();
            var logger = A.Fake<ILogger>();

            var firstFilePath = TestsHelper.CreateTestFile("SameFilePathTestFile.txt", "content");
            var secondfilePath = TestsHelper.CreateTestFile("SameFilePathTestFileCopy.txt", "tnetnoc");

            var file1 = new FileInfo(firstFilePath, 0, "", "");
            var file2 = new FileInfo(secondfilePath, 0, "", "");

            Assert.IsFalse(manager.ByteCompare(file1, file2, logger));
        }

        [Test]
        [TestCase(@"\/")]
        [TestCase("<>")]
        public void DirSearch_WrongDirectoryPath_WritesToLog_Test(string directoryPath)
        {
            var manager = new FileManager();
            var logger = A.Fake<ILogger>();
            manager.DirSearch(directoryPath, logger);

            A.CallTo(() => logger.Write(A<Exception>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void DirSearch_NotEmptyDirectory_ReturnsNotEmptyListOfFiles()
        {
            TestsHelper.CreateTestFile("NotEmptyDirectoryTestFile.txt", "some content");
            var manager = new FileManager();
            var logger = A.Fake<ILogger>();

            var files = manager.DirSearch(TestsHelper.TestsFolderPath, logger);

            Assert.IsTrue(files.Count != 0);
        }
    }
}
