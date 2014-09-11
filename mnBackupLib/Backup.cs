using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using SevenZip;
using System.IO;


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
        /// Список заданий
        /// </summary>
        public List<Task> Tasks
        {
            get { return _tasks; }
        }

        private List<Task> _tasks = new List<Task>();

        public Config Conf;

        /// <summary>
        /// Количество заданий
        /// </summary>
        public int Count
        {
            get { return _tasks.Count; }
        }

        public Backup()
        {
            Conf = new Config();
        }

        public Backup(Options options)
        {
            Conf = new Config(options);
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
        /// Прочитать задания из файла. Задания добавляются к текущим. Тип определяется по расширению. По умолчанию JSON
        /// </summary>
        /// <param name="FileName"></param>
        public void Read(string FileName)
        {
            List<Task> tsks = SerialIO.Read<List<Task>>(FileName);
            _tasks.AddRange(tsks);
        }

        /// <summary>
        /// Записывает список заданий в файл. Тип определяется по расширению. По умолчанию JSON
        /// </summary>
        /// <param name="FileName"></param>
        public bool Save(string FileName)
        {

            return SerialIO.Save(FileName, _tasks);
            
            
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
            logger.Info("Start task {0}", job.NameTask);
            StatusInfo<StatusBackup> si = new StatusInfo<StatusBackup>(StatusBackup.OK);
            
            //StatusInfo<bool> si2 = new StatusInfo<bool>(true);
            
            
            // Проверка существования каталогов
            if (!Directory.Exists(job.Source))
            {
                logger.Error("Каталог источника не существует {0}", job.Source);
                logger.Error("Task finished with status {0}", si.Status);
                return StatusBackup.Fatal;
            }

            if (!Directory.Exists(job.Destination))
            {

                bool cr = FileManage.DirectoryCreate(job.Destination);
                if (!cr)
                {
                    
                    logger.Error("Каталог приемника не существует {0}", job.Destination);
                    logger.Error("Task finished with status {0}", si.Status);
                    return StatusBackup.Fatal;
                }
            }
            
            Manifest manifest = new Manifest(job.GetManifestFile());
            TypeBackup BakType = manifest.GetCurrentTypeBackup(job.Plan);
            logger.Info("Backup type: {0}", BakType);
            if (BakType == TypeBackup.Differential) // Текущее копирование не полное
            {
                // Чутка подправим фильтр
                DateTime dt = manifest.GetLastFullDate();
                job.SourceFilter.NewerThan = dt;
            }

            // Имя архива
            string ArhName = job.GetArhName(BakType.ToString());
            string[] files = job.GetFiles(); // файлы для обработки

            

            if (files.Length > 0)
            {

                string FullArhName = Path.Combine(job.Destination, ArhName);

                // Сжать файлы синхронно
                Compressor cmp = new Compressor(job.ArhParam);
                bool suc = cmp.CompressFiles(FullArhName, files);

                if (!suc)
                {
                    si.UpdateStatus(StatusBackup.Error);
                }

                // Добавляем запись в манифест
                BakEntryInfo bakEntry = new BakEntryInfo(BakType, si.Status, ArhName);
                manifest.Add(bakEntry);
            }
            else // нечего копировать
            {
                if (BakType == TypeBackup.Full)
                {
                    logger.Error("Nothing to full backup");
                    si.UpdateStatus(StatusBackup.Fatal);
                }
                else
                {
                    logger.Info("Nothing to backup");
                }
            }
            // Удаляем старые архивы
            //StatusBackup sb = StatusBackup.OK;
            StatusBackup sb = DeleteOldArh(job, ref manifest);
            si.UpdateStatus(sb);
            
            manifest.Save();


            logger.Info("Task finished with status {0}", si.Status);

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
            BakEntryInfo[] baks = manifest.GetAllBeforePeriod(job.Plan.Store);
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
                    logger.Info("Удален архив {0}", fullArhName);
                    manifest.Delete(baks[i].BackupDate);
                    
                }
                else
                {
                    si.UpdateStatus(StatusBackup.Warning);
                    logger.Warn("Не найден архив для удаления {0}", fullArhName);
                }
            }
            return si.Status;
        }





    }
}
