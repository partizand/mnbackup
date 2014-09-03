using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using SevenZip;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace mnBackupLib
{
    /// <summary>
    /// Тип бэкапа. Полный, разностный
    /// </summary>
    public enum TypeBackup { Full, Differential };
    

    

    public class Backup
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Имя файла для информации о бэкапах
        /// </summary>
        //public const string ManifestFile="manifest";

        public List<Task> Tasks
        {
            get { return _tasks; }
        }

        private List<Task> _tasks = new List<Task>();

        /// <summary>
        /// Количество заданий
        /// </summary>
        public int Count
        {
            get { return _tasks.Count; }
        }

        public Backup()
        {
            
        }
        /// <summary>
        /// Добавить задание в список
        /// </summary>
        /// <param name="job"></param>
        public void Add(Task job)
        {
            _tasks.Add(job);
        }

        /// <summary>
        /// Прочитать задания из файла. Задания добавляются к текущим
        /// </summary>
        /// <param name="FileName"></param>
        public void Read(string FileName)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Task>));
            if (File.Exists(FileName))
            {
                FileStream fs = new FileStream(FileName, FileMode.Open);
                //List<Task> t=new 
                _tasks.AddRange((List<Task>)serializer.ReadObject(fs));
                fs.Close();
            }
        }

        /// <summary>
        /// Записывает список заданий в файл (JSON)
        /// </summary>
        /// <param name="FileName"></param>
        public void Save(string FileName)
        {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Task>));
            try
            {
                FileStream fs = new FileStream(FileName, FileMode.Create);
                serializer.WriteObject(fs, _tasks);
                fs.Close();
            }
            catch
            {

            }
        }
        /// <summary>
        /// Очищает список заданий
        /// </summary>
        public void Clear()
        {
            _tasks.Clear();
        }

        /// <summary>
        /// Запускает бэкап
        /// </summary>
        public void Start()
        {

            foreach (Task job in _tasks)
            {
                StartTask(job);
            }
        }
        /// <summary>
        /// Обработка задания
        /// </summary>
        /// <param name="job"></param>
        public StatusBackup StartTask(Task job)
        {
            logger.Info("Запуск задания {0}", job.NameTask);
            StatusInfo<StatusBackup> si = new StatusInfo<StatusBackup>(StatusBackup.OK);
            
            //StatusInfo<bool> si2 = new StatusInfo<bool>(true);
            
            
            // Проверка существования каталогов
            if (!Directory.Exists(job.Source))
            {
                logger.Error("Каталог источника не существует {0}", job.Source);
                return StatusBackup.Fatal;
            }

            if (!Directory.Exists(job.Destination))
            {

                bool cr = FileManage.DirectoryCreate(job.Destination);
                if (!cr)
                {
                    logger.Error("Каталог приемника не существует {0}", job.Destination);
                    return StatusBackup.Fatal;
                }
            }


            
            Manifest manifest = new Manifest(job.GetManifestFile());
            TypeBackup BakType=job.Plan.GetCurrentTypeBackup(manifest);
            if (BakType == TypeBackup.Differential) // Текущее копирование не полное
            {
                // Чутка подправим фильтр
                DateTime dt = manifest.GetLastFullDate();
                job.SourceFilter.NewerThan = dt;
            }

            // Имя архива
            string ArhName = job.GetArhName(BakType.ToString());
            string[] files = job.GetFiles(); // файлы для обработки

            string FullArhName = Path.Combine(job.Destination, ArhName);

            // Сжать файлы синхронно
            Compressor cmp = new Compressor(job.ArhParam);
            bool suc=cmp.CompressFiles(FullArhName, files);
            
            if (!suc)
            {
                si.AddStatus(StatusBackup.Error);
            }

            // Добавляем запись в манифест
            BakEntryInfo bakEntry = new BakEntryInfo(BakType, si.Status, ArhName);
            manifest.Add(bakEntry);
            
            // Удаляем старые архивы
            //StatusBackup sb = StatusBackup.OK;
            StatusBackup sb = DeleteOldArh(job, ref manifest);
            si.AddStatus(sb);
            
            manifest.Save();



            return (StatusBackup)si.Status;
        }

        /// <summary>
        /// Удаление старых архивов
        /// </summary>
        /// <param name="job"></param>
        private StatusBackup DeleteOldArh(Task job, ref Manifest manifest)
        {
            // Какие архивы нужно удалить
            //StatusBackup st = StatusBackup.OK;
            StatusInfo<StatusBackup> si = new StatusInfo<StatusBackup>(StatusBackup.OK);
            BakEntryInfo[] baks = manifest.GetAllBeforePeriod(job.Plan.FullIntervalSave);
            if (baks == null) return si.Status;
            if (baks.Length == 0) return si.Status;
            string fullArhName;
            int i;
            bool suc;
            for (i = 0; i < baks.Length; i++)
            {
                fullArhName = Path.Combine(job.Destination, baks[i].BackupFileName);
                suc = FileManage.FileDelete(fullArhName);
                if (suc)
                {
                    manifest.Delete(baks[i].BackupDate);
                }
                else
                {
                    si.AddStatus(StatusBackup.Warning);
                    logger.Warn("Не найден архив для удаления {0}", fullArhName);
                }
            }
            return si.Status;
        }





    }
}
