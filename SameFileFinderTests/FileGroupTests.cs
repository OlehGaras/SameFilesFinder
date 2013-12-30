using NUnit.Framework;
using SameFileFinder;

namespace SameFileFinderTests
{
    [TestFixture]
    public class FileGroupTests: BaseTests
    {
        [Test]
        public void Add_NewFile_IncrementsGroupCount_Test()
        {
            var group = new FileGroup();
            var file = new FileInfo("",0,"","");

            group.Add(file);

            Assert.IsTrue(group.Files.Count > 0);
        }
    }
}
