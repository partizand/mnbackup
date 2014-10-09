using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
//using System.IO;


namespace mnBackupLib
{
    /// <summary>
    /// Фильтр файлов для обработки
    /// </summary>
    [DataContract]
    public class FileFilter
    {
        
        //public enum AttributeFlag { NotUse, Set, NotSet };
        
        /// <summary>
        /// Маски включаемых файлов, разделяются ,;|
        /// </summary>
        [DataMember]
        public string IncludeFileMask;
        /// <summary>
        /// Маски исключаемых файлов, разделяются ,;|
        /// </summary>
        [DataMember]
        public string ExcludeFileMask;
        /// <summary>
        /// Рекурсивно (с подкаталогами)
        /// </summary>
        [DataMember]
        public bool Recurse=true;

        /// <summary>
        /// Файлы старее чем +==
        /// </summary>
        [DataMember]
        public DateTime OlderThan;
        /// <summary>
        /// Файлы новее чем ==+
        /// </summary>
        [DataMember]
        public DateTime NewerThan;

        /// <summary>
        /// Используемые атрибуты. 0-все. Или FileAttributes.Archive|FileAttributes.Hidden
        /// Сами значения атрибутов указываются в Attributes
        /// </summary>
        [DataMember]
        public System.IO.FileAttributes UsedAttributes;
        /// <summary>
        /// Значения атрибутов, если нужен сброшенный - не указывать в перечислении
        /// FileAttributes.Archive|FileAttributes.Hidden
        /// Используется совместно с UsedAttributes
        /// </summary>
        [DataMember]
        public System.IO.FileAttributes Attributes;

        public FileFilter()
        {
            IncludeFileMask = "*";
            ExcludeFileMask = "";
            OlderThan=DateTime.MaxValue;
            NewerThan = DateTime.MinValue;
            UsedAttributes = 0;
            
        }
        
        public FileFilter(string IncFileMask,string ExcFileMask)
        {
            IncludeFileMask = IncFileMask;
            ExcludeFileMask = ExcFileMask;
            OlderThan = DateTime.MaxValue;
            NewerThan = DateTime.MinValue;
            UsedAttributes = 0;
        }
        
        
        /// <summary>
        /// Входит ли файл в фильтр
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public bool isIn(string FileName)
        {
            string IncludeFileMaskPattern = makePattern(IncludeFileMask);
            string ExcludeFileMaskPattern = makePattern(ExcludeFileMask);

            // Проверка что файл вообще есть
            //if (!System.IO.File.Exists(FileName)) return false;


            // проверка что в Include
            bool inc = checkMask(FileName, IncludeFileMaskPattern);
            if (!inc) return false;
            // Проверка что в Exclude
            bool exc = checkMask(FileName, ExcludeFileMaskPattern);
            if (exc) return false;
            
            bool date = checkDate(FileName);
            if (!date) return false; // эта байда для ускорения
            bool attr = checkAttr(FileName);
            if (!attr) return false;
            return true;
            
            
        }
        /// <summary>
        /// Входит ли файл в диапазон дат
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private bool checkDate(string FileName)
        {
            // Нет фильтра на дату
            if (OlderThan == DateTime.MinValue && NewerThan == DateTime.MaxValue) return true;
            DateTime fDate = System.IO.File.GetLastWriteTime(FileName);
            if (fDate >= NewerThan && fDate <= OlderThan) return true;
            else return false;

            
        }

        /// <summary>
        /// проверка на атрибуты
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private bool checkAttr(string FileName)
        {
            if (UsedAttributes==0) return true; // Атрибуты не используются

            System.IO.FileAttributes attr = System.IO.File.GetAttributes(FileName);
            attr=attr&UsedAttributes; // Накладывается маска

            if (attr==Attributes) return true;
            else return false;
            
            

            
        }

        /// <summary>
        /// возвращает регулярное выражение для поиска RegEx из строки типа "STUD-*.xml|*.csv|*.7z|???.???"
        /// Исходные dos-овские маски переводятся в регулярные выражения. 

        /// </summary>
        /// <param name="delimS"></param>
        /// <returns></returns>
        /*
         Например, в нашем примере:

        STUD-*.xml переведется в ^STUD-.*\.xml$
        *.csv переведется в ^.*\.csv$
        *.7z переведется в ^.*\.7z$
        ???.??? переведется в ^...\....$ 
         */
        string makePattern(string delimS)
        {
            char[] Separator = new char[3] { ',', '|', ';' };
            string[] exts = delimS.Split(Separator);
            string pattern = string.Empty;
            foreach (string ext in exts)
            {
                pattern += @"^";//признак начала строки
                foreach (char symbol in ext)
                    switch (symbol)
                    {
                        case '.': pattern += @"\."; break;
                        case '?': pattern += @"."; break;
                        case '*': pattern += @".*"; break;
                        default: pattern += symbol; break;
                    }
                pattern += @"$|";//признак окончания строки
            }
            if (pattern.Length == 0) return pattern;
            pattern = pattern.Remove(pattern.Length - 1);
            return pattern;
        }


        /// <summary>
        /// Проверка соответствия имени файла маске
        /// </summary>
        /// <param name="fileName">Имя проверяемого файла</param>
        /// <param name="pattern">Паттерн</param>
        /// <returns>true - файл удовлетворяет маске, иначе false</returns>
        private bool checkMask(string fileName, string pattern)
        {

            if (String.IsNullOrEmpty(pattern)) return false;

            Regex mask = new Regex(pattern, RegexOptions.IgnoreCase);
            return mask.IsMatch(System.IO.Path.GetFileName(fileName));

        }
    }
}
