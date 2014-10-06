using System;
using System.Collections;
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
        /// Работа с дисками
        /// </summary>
        public class Volumes
        {
            
            /// <summary>
            /// Возвращает первый свободный диск в системе в виде L:
            /// </summary>
            /// <param name="useFloppyLetter">использовать буквы дисков А: и В:</param>
            /// <returns></returns>
            public static string GetFreeLetter(bool useFloppyLetter)
            {
                int start=65; //A letter
                if (!useFloppyLetter) start = start + 2;
                ArrayList driveLetters = new ArrayList(26); // Allocate space for alphabet
                for (int i = start; i < 91; i++) // increment from ASCII values for A-Z
                {
                    driveLetters.Add(Convert.ToChar(i)); // Add uppercase letters to possible drive letters
                }

                foreach (string drive in Directory.GetLogicalDrives())
                {
                    driveLetters.Remove(drive[0]); // removed used drive letters from possible drive letters
                }
                if (driveLetters.Count > 0) return driveLetters[0].ToString()+":";
                else return "";
                
            }
            /// <summary>
            /// Возвращает первый свободный диск в системе в виде L:, без использования дисков А: и В:
            /// </summary>
            /// <returns></returns>
            public static string GetFreeLetter()
            {
                return GetFreeLetter(false);
            }
            /// <summary>
            /// Возвращает prefLetter если такого диска нет или первый свободный диск
            /// </summary>
            /// <param name="prefLetter">какой диск назначить свободным, если его нет в системе (может быть в любой форме, проверяется только буква)</param>
            /// <returns>Свбодный диск в системе или "" если все занято</returns>
            public static string GetFreeLetter(string prefLetter)
            {
                if (!String.IsNullOrEmpty(prefLetter))
                {
                    if (!IsVolumeExist(prefLetter)) return prefLetter;

                }
                // просто получить первый свободный диск
                return GetFreeLetter();

            }
            /// <summary>
            /// Проверяет существует ли такой диск в системе
            /// </summary>
            /// <param name="letter">Проверяемый диск может быть в любой форме, проверяется только буква (например: d:\\ или d: или d)</param>
            /// <returns></returns>
            public static bool IsVolumeExist(string letter)
            {
                if (String.IsNullOrEmpty(letter)) return true; // Неправльный диск
                
                letter = letter.ToUpper();
                char let=letter[0];
                foreach (char drive in GetVolumes())
                {
                    if (let == drive)
                    {
                        return true;
                    }
                }
                return false;
                
            }
            /// <summary>
            /// Возвращает список занятых букв (без : и \\) в системе
            /// </summary>
            /// <returns></returns>
            private static char[] GetVolumes()
            {
                List<char> drives = new List<char>();
                foreach (string drive in Directory.GetLogicalDrives())
                {
                    string let=drive.ToUpper();
                    drives.Add(let[0]);
                }
                return drives.ToArray();
            }

        }

        /// <summary>
        /// Возвращает список файлов архива по имени архива, для получения всех томов архива
        /// Файлы в каталоге уже должны существовать
        /// Возвращает все файлы в каталоге FullArhName+*
        /// </summary>
        /// <param name="FullArhName"></param>
        /// <returns></returns>
        public static string[] GetArhFiles(string FullArhName)
        {
            string[] files;

            files=Directory.GetFiles(Path.GetDirectoryName(FullArhName), Path.GetFileName(FullArhName) + "*");

            int i;
            for (i = 0; i < files.Length; i++) // Убираются длинные имена
            {
                files[i] = Path.GetFileName(files[i]); // оставляем только имя файла
            }
            return files;

        }

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
        /// <summary>
        /// Преобразовывает список каталогов разделенных ; в массив
        /// </summary>
        /// <param name="delimDirs">список каталогов разделенных ; или ,</param>
        /// <returns>массив с каталогами</returns>
        public static string[] ConvertToArray(string delimDirs)
        {
            char[] delims = { ';', ',' };
            string[] lst;
            lst = delimDirs.Split(delims, StringSplitOptions.RemoveEmptyEntries);
            int i;
            for (i = 0; i < lst.Length; i++)
            {
                lst[i]=lst[i].Trim();
            }
            return lst;
        }
    }
}
