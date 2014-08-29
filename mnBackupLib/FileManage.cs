using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.IO;

namespace mnBackupLib
{
    /// <summary>
    /// Операции с файлами
    /// </summary>
    public class FileManage
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// безопасное удаление файла
        /// </summary>
        /// <param name="FileName">Полное имя файла</param>
        /// <returns>успех</returns>
        public static bool FileDelete(string FileName)
        {
            try
            {
                File.Delete(FileName);
                logger.Info("Удален файл {0}", FileName);
                return true;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка удаления файла {0}. {1}", FileName,e.Message);
                return false;
            }
        }
        public static bool DirectoryCreate(string FileName)
        {
            try
            {
                Directory.CreateDirectory(FileName);
                //logger.Info("Удаленя файл {0}", FileName);
                return true;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка создания каталога {0}. {1}", FileName, e.Message);
                return false;
            }
        }
    }
}
