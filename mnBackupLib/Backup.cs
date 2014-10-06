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

        //public Config Conf;

        /// <summary>
        /// Количество заданий
        /// </summary>
        public int Count
        {
            get { return _tasks.Count; }
        }

        public Backup()
        {
            //Conf = new Config();
            //Period per = new Period("1w");
            
            
            
        }
        /*
        public Backup(CommonSubOptions options)
        {
            //Conf = new Config();
            Config.Instance.MergeOptions(options);
            
        }
          */
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
        /// Чтение файла задания по умолчанию или заданного в параметрах запуска
        /// </summary>
        public void Read()
        {
            Read(Config.Instance.mnConfig.TaskFile);
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
            if (CheckDir(job.Source) == StatusBackup.Fatal)
            {
                logger.Error("Source dirs not founded");
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
            string FullArhName = job.GetArhName(BakType.ToString());
            string[] files = job.GetFiles(); // файлы для обработки

            

            if (files.Length > 0)
            {

                //string FullArhName = Path.Combine(job.Destination, ArhName);

                // Сжать файлы синхронно
                /*
                Compressor cmp = new Compressor(job.ArhParam);
                bool suc = cmp.CompressFiles(FullArhName, files);

                if (!suc)
                {
                    si.UpdateStatus(StatusBackup.Error);
                }
                */
                StatusBackup sb2 = StatusBackup.OK;
                if (job.Shadow)
                {
                    logger.Info("Shadow copy using");
                    try
                    {
                        if (job.SourceVolumes.Length!=1) // в источниках больше одного тома, теневое копирование на это не рассчитано
                        {
                            logger.Error("Too many volumes to shadow, use only one (or noo volumes, check source)");
                            return StatusBackup.Fatal;
                        }
                        string freeLetter = FileManage.Volumes.GetFreeLetter(Config.Instance.mnConfig.ExposeVolume);
                        if (String.IsNullOrEmpty(freeLetter))
                        {
                            logger.Error("No free letter to map snapshot");
                            return StatusBackup.Fatal;
                        }
                        using (Snapshot vss = new Snapshot(job.SourceVolumes[0], freeLetter))
                        {
                            //logger.Info("Creating shadow copy");
                            //vss.Setup(Path.GetPathRoot(job.Source),"L:");
                            vss.DoSnapshotSet();
                            
                            string[] ShadowFiles = vss.GetSnapshotPath(files);

                            // Here we use the AlphaFS library to make the copy.
                            //Alphaleonis.Win32.Filesystem.File.Copy(snap_path, backup_path);
                            
                            sb2 = DoFiles(job, FullArhName, ShadowFiles);
                            //logger.Info("Deleting shadow copy");
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error("Shadow error {0}", e.Message);
                        si.UpdateStatus(StatusBackup.Error);
                    }
                }
                else
                {

                    sb2 = DoFiles(job, FullArhName, files);
                }
                si.UpdateStatus(sb2);
                // Добавляем запись в манифест
                BakEntryInfo bakEntry = new BakEntryInfo(BakType, si.Status, FileManage.GetArhFiles(FullArhName));
                manifest.Add(bakEntry);
                // Удаляем старые архивы
                //StatusBackup sb = StatusBackup.OK;
                StatusBackup sb = DeleteOldArh(job, ref manifest);
                si.UpdateStatus(sb);

                manifest.Save();
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
            


            logger.Info("Task finished with status {0}", si.Status);

            return (StatusBackup)si.Status;
        }

        StatusBackup DoFiles(Task job,string FullArhName, string[] files)
        {

            StatusInfo<StatusBackup> si = new StatusInfo<StatusBackup>(StatusBackup.OK);
            // Сжать файлы синхронно
            Compressor cmp = new Compressor(job.ArhParam);
            bool suc = cmp.CompressFiles(FullArhName, files);

            if (!suc)
            {
                si.UpdateStatus(StatusBackup.Error);
            }
            return si.Status;
            
        }

        private StatusBackup CheckDir(string[] dirs)
        {
            if (dirs.Length < 1) return StatusBackup.Fatal;
            StatusInfo<StatusBackup> si = new StatusInfo<StatusBackup>(StatusBackup.OK);
            int i = 0;
            foreach (string dir in dirs)
            {
                // Проверка существования каталогов
                if (!Directory.Exists(dir))
                {
                    si.UpdateStatus(StatusBackup.Error);
                    logger.Error("Directory does not exist {0}", dir);
                    i++;
                }
            }
            if (i == dirs.Length) si.UpdateStatus(StatusBackup.Fatal); // Ни одного каталога нет
            
            return si.Status;

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
            int i,k;
            bool suc;
            for (i = 0; i < baks.Length; i++)
            {
                for (k = 0; k < baks[i].BackupFileNames.Length; k++)
                {

                    fullArhName = Path.Combine(manifest.ManifestDir, baks[i].BackupFileNames[k]);
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
            }
            return si.Status;
        }





    }
}
