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
            // Дата
            string str = "Date=${yyyy-MM-dd}";
            DateTime dt = new DateTime(2014, 10, 15);
            string newStr = mnBackupLib.ReplVar.ExpandVars(str, dt);
            Assert.AreEqual("Date=2014-10-15", newStr,"date test");
            
            // Переменные среды
            str = "Temp=%Temp% CompName=%ComputerName%";
            string CompName = System.Environment.MachineName;
            string TestParamName = "Temp";
            string TestParamValue;
            TestParamValue = System.Environment.GetEnvironmentVariable(TestParamName);

            string Etalon = String.Format("Temp={0} CompName={1}", TestParamValue, CompName);
            newStr = mnBackupLib.ReplVar.ExpandVars(str);
            Assert.AreEqual(Etalon, newStr,"Enviroment test");

            // Пользовательские переменные
            Dictionary<string, string> opt = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            string key1="val1";
            string key2="wally";
            opt.Add("key1", key1);
            opt.Add("key2", key2);
            Etalon = String.Format("key1={0} key2={1}",key1,key2);
            str = "key1=${key1} key2=${key2}";
            newStr = ReplVar.ExpandVars(str, opt);
            Assert.AreEqual(Etalon, newStr, "User options");

        }
        

    }
}
