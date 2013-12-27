using System;
using System.Collections.Generic;
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

            A.CallTo(() => logger.Write(A<Exception>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void CheckTheGroup_GroupWithSimilarFiles_ReturnsGroupsWithSimilarHashes_Test()
        {

        }

        [Test]
        [TestCase("hash", "hash", "hash")]
        [TestCase("hash", "hash", "otherHash")]
        public void FormTheGroup_ListWithFakeFilesWithSameHash_ReturnsGroupWithThisFiles_Test(string firstHash, string secondHash, string thirdHash)
        {
            var files = new List<FileInfo>()
                {
                    new FileInfo("", 0, "", firstHash), 
                    new FileInfo("", 0, "", secondHash), 
                    new FileInfo("", 0, "", thirdHash)
                };
            var finder = new Finder();

            var result = finder.FormTheGroup(files, f => f.Hash);

            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public void FormTheGroup_ListWithDifferentFiles_ReturnsEmptyListWithGroups_Test()
        {
            var files = new List<FileInfo>()
                {
                    new FileInfo("", 0, "", "firstHash"),
                    new FileInfo("", 0, "", "secondHash")
                };
            var finder = new Finder();

            var result = finder.FormTheGroup(files, file => file.Hash);
            Assert.IsTrue(result.Count == 0);
        }

        [Test]
        public void FindGroupOfSameFiles_FakeNullLenthSimilarFiles_ReturnsGroupWithThisFiles_Test()
        {
            var finder = new Finder();
            var manager = A.Fake<IFileManager>();
            var logger = A.Fake<ILogger>();
            var firstFile = A.Fake<FileInfo>();
            var secondFile = A.Fake<FileInfo>();
            var thirdFile = A.Fake<FileInfo>();

            A.CallTo(() => manager.DirSearch(A<string>.Ignored, logger)).Returns(new List<FileInfo>() { firstFile, secondFile, thirdFile });
            A.CallTo(() => manager.ByteCompare(A<FileInfo>.Ignored, A<FileInfo>.Ignored, logger)).Returns(true);

            var result = finder.FindGroupOfSameFiles(A<string>.Ignored, logger, manager);

            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public void FindGroupOfSameFiles_IncorrectDirectoryPath_ReturnsNull_Test()
        {
            var finder = new Finder();
            var manager = A.Fake<IFileManager>();
            var logger = A.Fake<ILogger>();

            var res = finder.FindGroupOfSameFiles(@"Incorrect\Path", logger, manager);

            Assert.IsTrue(res == null);
        }

        [Test]
        public void FindGroupOfSameFiles_FilesWithDifferentHashes_ReturnsEmptyGroupList_Test()
        {
            var firstFile = new FileInfo("", 0, "", "hash1");
            var secondFile = new FileInfo("", 0, "", "hash2");
            var thirdFile = new FileInfo("", 0, "", "hash3");

            var files = new List<FileInfo>() {firstFile, secondFile, thirdFile};
            var logger = A.Fake<ILogger>();
            var manager = A.Fake<IFileManager>();

            A.CallTo(() => manager.DirSearch(A<string>.Ignored, logger)).Returns(files);

            var finder = new Finder();

            var res = finder.FindGroupOfSameFiles(A<string>.Ignored, logger, manager);

            Assert.IsTrue(res.Count == 0);
        }
    }
}
