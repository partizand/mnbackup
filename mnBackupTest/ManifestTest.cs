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
            Manifest man=new Manifest("");
            DateTime today=DateTime.Today;
            
            DateTime dt=today.AddDays(-6);
            BakEntryInfo bakEntry1 = new BakEntryInfo(dt,TypeBackup.Full, StatusBackup.OK, "ArhName1");
            man.Add(bakEntry1);
            
            dt = today.AddDays(-5);
            BakEntryInfo bakEntry2 = new BakEntryInfo(dt, TypeBackup.Differential, StatusBackup.OK, "ArhName2");
            man.Add(bakEntry2);
            
            dt = today.AddDays(-4);
            BakEntryInfo bakEntry3 = new BakEntryInfo(dt, TypeBackup.Differential, StatusBackup.OK, "ArhName3");
            man.Add(bakEntry3);
            
            DateTime dtFull = today.AddDays(-3);
            BakEntryInfo bakEntry4 = new BakEntryInfo(dtFull, TypeBackup.Full, StatusBackup.OK, "ArhName4");
            man.Add(bakEntry4);
            
            dt = today.AddDays(-2);
            BakEntryInfo bakEntry5 = new BakEntryInfo(dt, TypeBackup.Differential, StatusBackup.OK, "ArhName5");
            man.Add(bakEntry5);

            DateTime lastFull=man.GetLastFullDate();
            Assert.AreEqual(dtFull, lastFull, "Последнее полное копирование");

            Period period=new Period(Period.PeriodName.Day,4);
            BakEntryInfo[] toDelete=man.GetAllBeforePeriod(period);
            Assert.AreEqual(3, toDelete.Length, "Заданий на удаление");

            Assert.AreEqual(bakEntry1, toDelete[0], "Удаляемое задание 1");
            Assert.AreEqual(bakEntry2, toDelete[1], "Удаляемое задание 2");
            Assert.AreEqual(bakEntry3, toDelete[2], "Удаляемое задание 3");

        }
    }
}
