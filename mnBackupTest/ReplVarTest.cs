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
        public void ExpandVarsTest()
        {
            // Дата
            string str = "Date=${yyyy-MM-dd}";
            DateTime dt = new DateTime(2014, 10, 15);
            string newStr = mnBackupLib.ReplVar.ExpandVars(str, dt);
            Assert.AreEqual("Date=2014-10-15", newStr,"date test");
            
            // Переменные среды
            string TestEnvName = "Temp";
            str = String.Format("{0}=%{0}%",TestEnvName);
            
            
            string TestParamValue;
            TestParamValue = System.Environment.GetEnvironmentVariable(TestEnvName);

            string Etalon = String.Format("{0}={1}", TestEnvName,TestParamValue);
            newStr = mnBackupLib.ReplVar.ExpandVars(str);
            Assert.AreEqual(Etalon, newStr,"Enviroment test");

            // Имя компьютера
            string CompName = System.Environment.MachineName;
            str = "CompName=${ComputerName}";
            newStr = ReplVar.ExpandVars(str);
            Etalon = String.Format("CompName={0}",CompName);
            Assert.AreEqual(Etalon, newStr, "Enviroment test");


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
