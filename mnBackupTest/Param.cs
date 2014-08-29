using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mnBackupLib;
using System.IO;
using NLog;
using NUnit.Framework;

namespace mnBackupTest
{
    // Параметры тестирования
    class Param
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static string SubDirForTest = "Test";

        
        
        /// <summary>
        /// Создание каталога для тестов
        /// </summary>
        /// <returns></returns>
        public static bool CreateTestDir()
        {
            string TestDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, SubDirForTest);
            return FileManage.DirectoryCreate(TestDir);
        }
        /// <summary>
        /// Очистка каталога для тестов
        /// </summary>
        /// <returns></returns>
        public static bool ClearTestDir()
        {
            string TestDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, SubDirForTest);
            return FileManage.DirectoryClear(TestDir);
        }

        /// <summary>
        /// Создание тестового файла
        /// </summary>
        /// <param name="FileName">имя файла относительно тестового каталога</param>
        /// <returns></returns>
        public static bool CreateFileToTest(string FileName)
        {
            return CreateFileToTest(FileName, DateTime.MinValue);
        }
        
        /// <summary>
        /// Создание тестового файла с заданной датой модификации
        /// </summary>
        /// <param name="FileName">имя файла относительно тестового каталога</param>
        /// <param name="lastWriteTime">Если не нужно менять дату то DateTime.MinValue или MaxValue</param>
        /// <returns></returns>
        public static bool CreateFileToTest(string FileName, DateTime lastWriteTime)
        {
            string TestDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, SubDirForTest);
            string fullFileName = Path.Combine(TestDir, FileName);
            try
            {
                StreamWriter sw = File.CreateText(fullFileName);
                
                sw.Write("Test file for mnBackup");
                sw.Close();
                if (lastWriteTime != DateTime.MinValue && lastWriteTime != DateTime.MaxValue) // Установить дату нужно
                {
                    File.SetLastWriteTime(fullFileName, lastWriteTime);
                }
                return true;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка создания файла {0}. {1}", fullFileName,e.Message);
                return false;
            }

        }
    }
}
