using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mnBackupLib;
using NUnit.Framework;

namespace mnBackupTest
{
    [TestFixture]
    public class ConfigTest
    {
        [Test]
        public void MergeOptionsTest()
        {
            Config conf = new Config();
            Assert.AreEqual("L:", conf.mnConfig.ExposeVolume);

            Options opt = new Options();
            opt.ExposeVolume = "Z:";
            //Config conf = new Config(opt);
            conf.MergeOptions(opt);
            Assert.AreEqual("Z:", conf.mnConfig.ExposeVolume);
        }
    }
}
