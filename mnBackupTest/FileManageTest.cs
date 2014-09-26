using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mnBackupLib;
using NUnit.Framework;

namespace mnBackupTest
{
    [TestFixture]
    public class FileManageTest
    {
        [Test]
        public void ConvertToArrayTest()
        {
            string[] dirs = { "c:\\test\\test" };
            string dir = "c:\\test\\test";
            string[] conv = FileManage.ConvertToArray(dir);
            Assert.AreEqual(dirs, conv);
            dirs = new string[]  { "c:\\test\\test","d:\\test\\test2" };
            dir = "c:\\test\\test ; d:\\test\\test2";
            conv = FileManage.ConvertToArray(dir);
            Assert.AreEqual(dirs, conv);
        }
        [Test]
        public void IsVolumeExistTest()
        {
            string let = "C:\\";
            Assert.AreEqual(true, FileManage.Volumes.IsVolumeExist(let));
            let = "L:";
            Assert.AreEqual(let, FileManage.Volumes.GetFreeLetter(let));
        }
    }
}
