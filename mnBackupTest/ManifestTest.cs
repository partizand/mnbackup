using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using mnBackupLib;

namespace mnBackupTest
{
    [TestFixture]
    public class ManifestTest
    {
        [Test]
        public void TestManifest()
        {
            Task job = new Task("TestTask", "c:\\temp\\source", "d:\\temp\\dest");
            Manifest man=new Manifest(job);
            DateTime today=DateTime.Today;
            
            DateTime dt=today.AddDays(-6);
            string[] arhName = { "ArhName1" };
            BackupInfo bakEntry1 = new BackupInfo(dt, TypeBackup.Full, StatusBackup.OK, arhName);
            man.Add(bakEntry1);
            
            arhName = new string[] { "ArhName2" };
            dt = today.AddDays(-5);
            BackupInfo bakEntry2 = new BackupInfo(dt, TypeBackup.Differential, StatusBackup.OK, arhName);
            man.Add(bakEntry2);

            arhName = new string[] { "ArhName3" };
            dt = today.AddDays(-4);
            BackupInfo bakEntry3 = new BackupInfo(dt, TypeBackup.Differential, StatusBackup.OK, arhName);
            man.Add(bakEntry3);

            arhName = new string[] { "ArhName4" };
            DateTime dtFull = today.AddDays(-3);
            BackupInfo bakEntry4 = new BackupInfo(dtFull, TypeBackup.Full, StatusBackup.OK, arhName);
            man.Add(bakEntry4);

            arhName = new string[] { "ArhName5" };
            dt = today.AddDays(-2);
            BackupInfo bakEntry5 = new BackupInfo(dt, TypeBackup.Differential, StatusBackup.OK, arhName);
            man.Add(bakEntry5);

            DateTime lastFull=man.GetLastFullDate();
            Assert.AreEqual(dtFull, lastFull, "Последнее полное копирование");

            TimePeriod period = new TimePeriod("4d");
            BackupInfo[] toDelete=man.GetAllBeforePeriod(period);
            Assert.AreEqual(3, toDelete.Length, "Заданий на удаление");

            Assert.AreEqual(bakEntry1, toDelete[0], "Удаляемое задание 1");
            Assert.AreEqual(bakEntry2, toDelete[1], "Удаляемое задание 2");
            Assert.AreEqual(bakEntry3, toDelete[2], "Удаляемое задание 3");

        }
    }
}
