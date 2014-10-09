using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mnBackupLib;
using NUnit.Framework;
using System.Configuration;

namespace mnBackupTest
{
    [TestFixture]
    public class ConfigTest
    {
        [Test]
        public void MergeOptionsTest()
        {
            //Config conf = new Config();
            Assert.AreEqual("L:", Config.Instance.mnConfig.ExposeVolume);
            TimePeriod EtalonPer = new TimePeriod("2w");
            TimePeriod per = new TimePeriod(Config.Instance.mnConfig.Interval);
            Assert.AreEqual(EtalonPer, per, "Default interval");

            Options opt = new Options();
            opt.TaskOpt.ExposeVolume = "Z:";
            //Config conf = new Config(opt);
            Config.Instance.MergeOptions(opt.TaskOpt);
            Assert.AreEqual("Z:", Config.Instance.mnConfig.ExposeVolume, "Слияение с Options");
        }
    }
}
