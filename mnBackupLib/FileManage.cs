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
        /// <summary>
        /// Удаление каталога
        /// </summary>
        /// <param name="DirectoryName"></param>
        /// <returns></returns>
        public static bool DirectoryDelete(string DirectoryName)
        {
            if (!Directory.Exists(DirectoryName))
            {
                logger.Warn("Каталог для удаления не существует {0}", DirectoryName);
                return false;
            }
            try
            {
                Directory.Delete(DirectoryName,true);
                return true;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка удаления каталога {0}. {1}", DirectoryName, e.Message);
                return false;
            }

        }
        
        /// <summary>
        /// Создание каталога
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool DirectoryCreate(string DirectoryName)
        {
            try
            {
                DirectoryInfo di= Directory.CreateDirectory(DirectoryName);
                if (di == null)
                {
                    return false;
                }
                //logger.Info("Удаленя файл {0}", FileName);
                return true;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка создания каталога {0}. {1}", DirectoryName, e.Message);
                return false;
            }
        }
        public static bool DirectoryClear(string DirectoryName)
        {
            if (!Directory.Exists(DirectoryName))
            {
                logger.Warn("Каталог для очистки не существует {0}", DirectoryName);
                return false;
            }
            // Удаляем файлы
            string[] files = Directory.GetFiles(DirectoryName);
            bool ret = true;
            bool suc;
            foreach (string file in files)
            {
                suc=FileDelete(file);
                ret = ret & suc;
            }
            // Удаляем каталоги
            string[] dirs = Directory.GetDirectories(DirectoryName);
            foreach (string dir in dirs)
            {
                suc =  DirectoryDelete(dir);
                ret = ret & suc;
            }
            return ret;
        }
    }
}
