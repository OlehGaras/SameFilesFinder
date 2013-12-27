using System;
using NUnit.Framework;
using FakeItEasy;
using SameFileFinder;

namespace SameFileFinderTests
{
    [TestFixture]
    public class FinderTests : BaseTests
    {
        [Test]
        public void CompareFiles_FakeSimilarFiles_ReturnsGroupWithThisFiles_Test()
        {
            var firstFile = A.Fake<FileInfo>();
            var secondfile = A.Fake<FileInfo>();            
            var manager = A.Fake<IFileManager>();            
            var logger = A.Fake<ILogger>();

            var group = new FileGroup();
            var finder = new Finder();

            group.Add(firstFile);
            group.Add(secondfile);

            A.CallTo(() => manager.ByteCompare(A<FileInfo>.Ignored, A<FileInfo>.Ignored, A<ILogger>.Ignored)).Returns(true);
            var res = finder.CompareFiles(group, logger, manager);

            Assert.IsTrue(res.Count == 1);

        }

        [Test]
        public void HashTheFile_IncorrectFilePath_ReturnsStringEmpty_Test()
        {
            var finder = new Finder();
            var logger = A.Fake<ILogger>();
            string hash = finder.HashTheFile(@"Incorrect\Path.txt", logger);

            Assert.IsTrue(hash == string.Empty);
        }

        [Test]
        public void HashTheFile_CorrectFilePath_ReturnsNotEmptyString()
        {
            var finder = new Finder();
            var logger = A.Fake<ILogger>();
            string filePath = TestsHelper.CreateTestFile("CorrectFilePathFileTest.txt", "hash the file");

            string hash = finder.HashTheFile(filePath, logger);
            Assert.IsTrue(hash != string.Empty);
        }

        [Test]
        public void HashTheFile_SituationWithException_WritesToLog()
        {
            var finder = new Finder();
            var logger = A.Fake<ILogger>();

            finder.HashTheFile(@"Incorrect\Path.txt", logger);

            A.CallTo(()=>logger.Write(A<Exception>.Ignored)).MustHaveHappened();
        }
    }
}
