using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mnBackupLib;
using NUnit.Framework;
using System.IO;

namespace mnBackupTest
{
    [TestFixture]
    public class FileFilterTest
    {
        /// <summary>
        /// Тест фильтров
        /// </summary>
        [Test]
        public void TestFilt()
        {
            // Нужно создать файлы
            /*
            bool res;
            res = Param.CreateTestDir();
            Assert.AreEqual(res, true,"Каталог для тестов успешно создался");
            if (!res) return;

            
            res=Param.ClearTestDir();
            Assert.AreEqual(res, true, "Каталог для тестов успешно очистился");
            if (!res) return;

            bool suc;
            res = true;
            suc=Param.CreateFileToTest("test.txt");
            res = res & suc;
            suc = Param.CreateFileToTest("test.log");
            res = res & suc;
            suc = Param.CreateFileToTest("test.zip");
            res = res & suc;
            suc = Param.CreateFileToTest("mtest.zip");
            res = res & suc;
            suc = Param.CreateFileToTest("test.7z");
            res = res & suc;
            Assert.AreEqual(res, true, "Файлы для тестов успешно создались");
            */
            FileFilter ff = new FileFilter("*.txt", "");

            bool a;
            
            a = ff.isIn("c:\\dir\\test.txt");
            Assert.AreEqual(a, true);

            

            a = ff.isIn("c:\\dir\\test.log");
            Assert.AreNotEqual(a, true);

            a = ff.isIn("c:\\dir\\code.zip");
            Assert.AreEqual(a, true);

            a = ff.isIn("c:\\dir\\todo.zip");
            Assert.AreNotEqual(a, true);

            ff = new FileFilter();
            a = ff.isIn("c:\\dir\\test.txt");
            Assert.AreEqual(a, true);

        }
        /// <summary>
        /// Тест чтения конфига из файла
        /// </summary>
        [Test]
        public void TaskListReadTest()
        {
            Backup bak = new Backup();
            Task job = new Task("Задание1", "Каталог1", "Каталог12");
            job.SourceFilter.IncludeFileMask = "1*";
            job.SourceFilter.ExcludeFileMask = "2*";
            bak.Tasks.Tasks.Add(job);
            job = new Task("Задание2", "Каталог2", "Каталог22");
            job.SourceFilter.IncludeFileMask = "21*";
            job.SourceFilter.ExcludeFileMask = "22*";
            bak.Tasks.Tasks.Add(job);
            bak.Tasks.Save("d:\\test.json");
            bak.Tasks.Clear();
            bak.Tasks.Read("d:\\test.json");
            Assert.AreEqual(bak.Tasks.Tasks[0].NameTask,"Задание1");
            Assert.AreEqual(bak.Tasks.Tasks[0].SourceFilter.IncludeFileMask, "1*");
            Assert.AreEqual(bak.Tasks.Tasks[0].SourceFilter.ExcludeFileMask, "2*");
        }


    }
}
