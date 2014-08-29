using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mnBackupLib;
using NUnit.Framework;

namespace mnBackupTest
{
    [TestFixture]
    public class StatusInfoTest
    {
        [Test]
        public void TestStatusInfo()
        {
            StatusInfo si = new StatusInfo();
            Assert.AreEqual(si.Status, StatusBackup.OK,"Первоначально должно быть ОК");
            si.AddStatus(StatusBackup.Error);
            Assert.AreEqual(si.Status, StatusBackup.Error, "Обновилось до error");
            si.AddStatus(StatusBackup.Warning);
            Assert.AreNotEqual(si.Status, StatusBackup.Warning, "Warning ниже error");


        }
    }
}
