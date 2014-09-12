using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace mnBackupLib
{
    /// <summary>
    /// Свойства одного задания
    /// </summary>
    [DataContract]
    public class Task
    {
        //public enum Action {Copy,Arh };


        /// <summary>
        /// Имя задания
        /// </summary>
        [DataMember]
        public string NameTask { get; set; }

        /// <summary>
        /// Префикс имени архива
        /// </summary>
        [DataMember]
        public string Prefix { get; set; }

        /// <summary>
        /// Задание разрешено, будет запускаться
        /// </summary>
        [DataMember]
        public bool Enabled { get; set; }

        /// <summary>
        /// Использовать теневое копирование
        /// </summary>
        [DataMember]
        public bool Shadow { get; set; }

        /// <summary>
        /// Каталог источник
        /// </summary>
        [DataMember]
        public string Source { get; set; }

        /// <summary>
        /// Каталог приемник
        /// </summary>
        [DataMember]
        public string Destination { get; set; }

        /// <summary>
        /// План бэкапа (полный, разностный, сколько копий хранить и т.д.)
        /// </summary>
        [DataMember]
        public BackupPlan Plan { get; set; }//=new BackupPlan();

        /// <summary>
        /// Фильтр
        /// </summary>
        [DataMember]
        public FileFilter SourceFilter { get; set; }//=new FileFilter();

        [DataMember]
        public CompressParam ArhParam { get; set; }//=new CompressParam();

        public Task(string nameTask, string source,string destination)
        {
            //Enabled = true;
            NameTask = nameTask;
            Source = source;
            Destination = destination;
            Init();
        }
        public Task(TaskSubOptions taskOptions)
        {
            Init();
            this.NameTask = "CmdTask";
            this.Source = taskOptions.Source;
            this.Destination = taskOptions.Destination;
            this.Shadow = taskOptions.Shadow;
            this.Plan.Type = taskOptions.typeBackup;
            this.Plan.Interval = new Period(taskOptions.Interval);
            this.Plan.Store = new Period(taskOptions.Store);

        }
        /// <summary>
        /// Первоначальная инициализация для конструкторов
        /// </summary>
        private void Init()
        {
            Enabled = true;
            Shadow = false;
            Plan = new BackupPlan();
            SourceFilter = new FileFilter();
            ArhParam = new CompressParam();
        }

        /// <summary>
        /// Возвращает имя файла манифеста
        /// </summary>
        /// <returns></returns>
        public string GetManifestFile()
        {
            string manifestFile = Path.Combine(Destination, "manifest"+GetPrefix()+".json");
            
            return manifestFile;
        }

        /// <summary>
        /// Получить имя архива, без пути
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetArhName(string type)
        {
            string pref=GetPrefix();
            
            pref = pref + DateTime.Now.ToString("_yyMMdd-HHmmss_") + type + ".7z";// Добавить текущую дату и время
            pref = Path.Combine(this.Destination, pref);
            return pref; 
        }
        /// <summary>
        /// Получить префикс задания
        /// </summary>
        /// <returns></returns>
        public string GetPrefix()
        {
            string pref;
            if (!String.IsNullOrEmpty(Prefix))
            {
                pref = Prefix;
            }
            else
            {
                pref = NameTask.Replace(" ", ""); // Убрать пробелы из имени
            }
            return pref;
        }

        /// <summary>
        /// Возвращает список файлов для обработки задания в соответсвии с фильтром
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public string[] GetFiles()
        {
            List<string> files = new List<string>();
            // Перебрать все файлы в каталоге
            string dir = Source;
            SearchOption rec = SourceFilter.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] allFiles = Directory.GetFiles(dir, "*", rec);
            foreach (string curFile in allFiles)
            {
                if (SourceFilter.isIn(curFile))
                {
                    files.Add(curFile);
                }
            }

            return files.ToArray();

        }
        

      
    }
    
}
