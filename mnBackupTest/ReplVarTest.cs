using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using mnBackupLib;

namespace mnBackupTest
{
    class ReplVarTest
    {
        [Test]
        public void ReplDateTest()
        {
            string str = "Date={yyyy-MM-dd}";
            DateTime dt = new DateTime(2014, 10, 15);
            string newStr = mnBackupLib.ReplVar.ReplDate(str, dt);
            Assert.AreEqual("Date=2014-10-15", newStr);
        }
        [Test]
        public void ReplENVTest()
        {
            string str = "TestEnvParam=%Temp% CompName=%ComputerName%";
            string CompName = System.Environment.MachineName;
            string TestParamName = "Temp";
            string TestParamValue;
            TestParamValue = System.Environment.GetEnvironmentVariable(TestParamName);

            string Etalon = String.Format("TestEnvParam={0} CompName={1}", TestParamValue, CompName);
            string newStr = mnBackupLib.ReplVar.ReplENV(str);
            Assert.AreEqual(Etalon, newStr);
        }

    }
}
