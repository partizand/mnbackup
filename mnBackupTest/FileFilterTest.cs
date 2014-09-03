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
            
            // Включение файлов

            FileFilter ff = new FileFilter("*.txt,*.zip", "");

            bool a;
            
            a = ff.isIn("c:\\dir\\test.txt");
            Assert.AreEqual(a, true);

            a = ff.isIn("c:\\dir\\test.log");
            Assert.AreNotEqual(a, true);

            a = ff.isIn("c:\\dir\\code.zip");
            Assert.AreEqual(a, true);

            a = ff.isIn("c:\\dir\\todo.zip");
            Assert.AreEqual(a, true);

            a = ff.isIn("c:\\dir\\test.7z");
            Assert.AreNotEqual(a, true);

            // Исключение файлов
            ff = new FileFilter("*", "*.txt,*.zip");

            a = ff.isIn("c:\\dir\\test.txt");
            Assert.AreNotEqual(a, true);

            a = ff.isIn("c:\\dir\\test.log");
            Assert.AreEqual(a, true);

            a = ff.isIn("c:\\dir\\code.zip");
            Assert.AreNotEqual(a, true);

            a = ff.isIn("c:\\dir\\todo.zip");
            Assert.AreNotEqual(a, true);

            a = ff.isIn("c:\\dir\\test.7z");
            Assert.AreEqual(a, true);

            // Включение и исключение файлов
            ff = new FileFilter("*.txt,*.zip", "????.txt,~*.zip");

            a = ff.isIn("c:\\dir\\test.txt");
            Assert.AreNotEqual(a, true);

            a = ff.isIn("c:\\dir\\test.log");
            Assert.AreNotEqual(a, true);

            a = ff.isIn("c:\\dir\\code.zip");
            Assert.AreEqual(a, true);

            a = ff.isIn("c:\\dir\\~code.zip");
            Assert.AreNotEqual(a, true);

            a = ff.isIn("c:\\dir\\1234.txt");
            Assert.AreNotEqual(a, true);

            a = ff.isIn("c:\\dir\\test.7z");
            Assert.AreNotEqual(a, true);




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
            bak.Add(job);
            job = new Task("Задание2", "Каталог2", "Каталог22");
            job.SourceFilter.IncludeFileMask = "21*";
            job.SourceFilter.ExcludeFileMask = "22*";
            bak.Add(job);
            bak.Save("d:\\test.json");
            bak.Clear();
            bak.Read("d:\\test.json");
            Assert.AreEqual(bak.Tasks[0].NameTask,"Задание1");
            Assert.AreEqual(bak.Tasks[0].SourceFilter.IncludeFileMask, "1*");
            Assert.AreEqual(bak.Tasks[0].SourceFilter.ExcludeFileMask, "2*");
        }


    }
}
