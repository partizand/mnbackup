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
        public string[] Source { get; set; }
        //public string[] Source { get; set; }
        //private string[] _Source;

        /// <summary>
        /// Список дисков источников (для теневого копирования)
        /// </summary>
        //public string[] SourceVolumes { get { return _SourceVolumes; } }
        //private string[] _SourceVolumes;
        /// <summary>
        /// Каталог приемник
        /// </summary>
        [DataMember]
        public string[] Destination { get; set; }

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
            this.Source = FileManage.ConvertToArray(source);// SetSource(source);
            this.Destination = FileManage.ConvertToArray(destination);
            Init();
        }
        public Task(TaskSubOptions taskOptions)
        {
            Init();
            // Prefix
            if (!String.IsNullOrEmpty(taskOptions.Prefix))
            {
                this.Prefix = taskOptions.Prefix;
            }

            this.Source = FileManage.ConvertToArray(taskOptions.Source);
            //SetSource(taskOptions.Source);
            // Name
            if (!String.IsNullOrEmpty(taskOptions.NameTask))
            {
                this.NameTask = taskOptions.NameTask;
            }
            else
            {
                if (this.Source.Length > 0)
                {
                    this.NameTask = Path.GetFileName(this.Source[0]); // Имя задания последний каталог первого источника
                }
                else
                {
                    this.NameTask = "Cmd";
                }
            }
            //this.Source = FileManage.ConvertToArray(taskOptions.Source);
            this.Destination = FileManage.ConvertToArray(taskOptions.Destination);
            this.Shadow = taskOptions.Shadow;
            this.Plan.Type = taskOptions.typeBackup;
            // Interval
            if (taskOptions.Interval == null)
            {
                this.Plan.Interval = new TimePeriod(Config.Instance.mnConfig.Interval);
            }
            else
            {
                this.Plan.Interval = new TimePeriod(taskOptions.Interval);
            }
            // Store
            if (String.IsNullOrEmpty(taskOptions.Store))
            {
                this.Plan.Store = new TimePeriod(Config.Instance.mnConfig.Store);
            }
            else
            {
                this.Plan.Store = new TimePeriod(taskOptions.Store);
            }
            // VolumeSize
            if (taskOptions.VolumeSize != null)
            {
                this.ArhParam.VolumeSize = (int)taskOptions.VolumeSize;
            }
            // Password
            if (!String.IsNullOrEmpty(taskOptions.Password))
            {
                this.ArhParam.Password = taskOptions.Password;
            }
            // FileFilter
            if (!String.IsNullOrEmpty(taskOptions.IncludeFileMask)) 
                this.SourceFilter.IncludeFileMask=taskOptions.IncludeFileMask;
            if (!String.IsNullOrEmpty(taskOptions.ExcludeFileMask)) 
                this.SourceFilter.ExcludeFileMask=taskOptions.ExcludeFileMask;
            if (!String.IsNullOrEmpty(taskOptions.ExcludeFileMask)) 
                this.SourceFilter.ExcludeFileMask=taskOptions.ExcludeFileMask;
            if (taskOptions.OlderThan!=null) 
                this.SourceFilter.OlderThan=(DateTime)taskOptions.OlderThan;
            if (taskOptions.NewerThan!=null)
                this.SourceFilter.NewerThan = (DateTime)taskOptions.NewerThan;


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
        /// Возвращает имя файла манифеста в каталоге назначения
        /// </summary>
        /// <returns></returns>
        public string GetManifestFile()
        {
            //string dest = String.Empty;
            //if (destIndex < Destination.Length) dest = Destination[destIndex];
            //string manifestFile = Path.Combine(dest, "manifest"+GetPrefix()+".json");
            string manifestFile =  "manifest" + NameTask + ".json";

            return manifestFile;
        }
        /// <summary>
        /// Задает источник для копирования из каталогов разделенных ;
        /// </summary>
        /// <param name="sources">Список каталогов источника разделенных ;</param>
        /*
        private void SetSource(string sources)
        {
            this.Source = FileManage.ConvertToArray(sources);
            //_SourceVolumes = GetSourceVolumes();
        }
        */
        /// <summary>
        /// Задает источник для копирования из каталогов разделенных ;
        /// </summary>
        /// <param name="sources">Список каталогов источника разделенных ;</param>
        /*
        private void SetSource(string[] sources)
        {
            this.Source = sources;
            //_SourceVolumes = GetSourceVolumes();
        }
        */
        
        /// <summary>
        /// Получить имя архива, без пути
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetArhName(string type)
        {
            string pref=GetPrefix(type);
            
            

            pref = NameTask + pref  + ".7z";// Добавить текущую дату и время
            //pref = Path.Combine(this.Destination, pref);
            return pref; 
        }

        /// <summary>
        /// Получить префикс задания раскрытый из переменных
        /// </summary>
        /// <returns></returns>
         string GetPrefix(string type)
        {
            string pref;

            if (String.IsNullOrEmpty(Prefix))
                Prefix = Config.Instance.mnConfig.Prefix;
            Dictionary<string, string> opt = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            opt.Add("Type", type);
            pref = ReplVar.ExpandVars(Prefix,opt);
            


            return pref;
        }


        /// <summary>
        /// Возвращает каталог в который нужно архивировать.
        /// Это либо временный каталог, либо первый каталог из списка Destination
        /// </summary>
        /// <returns></returns>
        public string GetArhWorkDir()
        {
            string WorkDir;
            
            // Архивация может идти в TempDir или в первый каталог приемника
            if (this.Destination.Length > 0)
            {
                if (!String.IsNullOrEmpty(Config.Instance.mnConfig.TempDir))
                {
                    // Опеределить фиксированный ли это диск или сетевой
                    string root = Path.GetPathRoot(Destination[0]);
                    try
                    {
                        DriveInfo drive = new DriveInfo(root);
                        if (drive.DriveType == DriveType.Fixed) // Fixed disk
                        {
                            WorkDir = Destination[0];
                            return WorkDir;

                        }
                        else // Non fixed disk
                        {
                            WorkDir = Config.Instance.mnConfig.TempDir;
                            return WorkDir;
                        }
                    }
                    catch // No drive letter
                    {
                        WorkDir = Config.Instance.mnConfig.TempDir;
                        return WorkDir;
                    }
                    
                }
                else
                {
                    WorkDir = Destination[0];
                    return WorkDir;
                }

            }
            return String.Empty;
            
            


        }

        
        /// <summary>
        /// Возвращает список файлов для обработки задания в соответсвии с фильтром
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public string[] GetFiles()
        {
            List<string> files = new List<string>();
            // Перебрать все каталоги
            foreach (string dir in Source)
            {
                // Перебрать все файлы в каталоге
                
                SearchOption rec = SourceFilter.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                string[] allFiles = Directory.GetFiles(dir, "*", rec);
                foreach (string curFile in allFiles)
                {
                    if (SourceFilter.isIn(curFile))
                    {
                        files.Add(curFile);
                    }
                }
            }

            return files.ToArray();

        }
        /// <summary>
        /// Для источников можно использовать теневое копирование
        /// проверяется что идет использование одного диска и он fixed
        /// </summary>
        /// <returns></returns>
        public bool isSourceVSS()
        {

            

            if (Source.Length < 0) return false;
            string volume = Path.GetPathRoot(Source[0]).ToUpper();
            string volume2;
            int i;
            for (i = 1; i < Source.Length; i++)
            {
                volume2 = Path.GetPathRoot(Source[i]).ToUpper();
                if (volume2.CompareTo(volume) == 0)
                {
                    return false;
                }
            }
            DriveInfo drive = new DriveInfo(volume);
            if (drive.DriveType == DriveType.Fixed) // Fixed disk
                return true;
            else
                return false;

            
        }

        /// <summary>
        /// Возвращает список дисков в каталогах источниках
        /// </summary>
        /// <returns></returns>
        /*
        private string[] GetSourceVolumes()
        {
            string volume;
            List<string> lst = new List<string>();
            foreach (string dir in Source)
            {
                volume=Path.GetPathRoot(dir).ToUpper();
                if (!lst.Exists(obj => obj.CompareTo(volume) == 0))
                {
                    lst.Add(volume);
                }
            }
            return lst.ToArray();
        }
        */
      
    }
    
}
