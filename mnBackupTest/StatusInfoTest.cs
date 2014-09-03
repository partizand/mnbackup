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
            StatusInfo<StatusBackup> si = new StatusInfo<StatusBackup>(StatusBackup.OK);
            Assert.AreEqual(si.Status, StatusBackup.OK,"Первоначально должно быть ОК");
            si.AddStatus(StatusBackup.Error);
            Assert.AreEqual(si.Status, StatusBackup.Error, "Обновилось до error");
            si.AddStatus(StatusBackup.Warning);
            Assert.AreNotEqual(si.Status, StatusBackup.Warning, "Warning ниже error");

            //bool a = true;

            bool a = true;
            bool b = false;
            int res= a.CompareTo(b);

            Assert.Greater(a.CompareTo(b),0,"true больше false");
            
            Assert.Less(b.CompareTo(a), 0, "false меньше true");

            

            StatusInfo<bool> isError = new StatusInfo<bool>(false);
            isError.AddStatus(false);
            Assert.AreEqual(isError.Status, false, "IsError в начале false");
            isError.AddStatus(true);
            Assert.AreEqual(isError.Status, true, "IsError в конце true");
            isError.AddStatus(false);
            Assert.AreEqual(isError.Status, true, "IsError в конце все равно true");


        }
    }
}
