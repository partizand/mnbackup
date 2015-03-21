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

            StatusBackup status;

            

            // Проверка существования каталогов
            status = CheckDir(job.Source, false); // Источников
            si.UpdateStatus(status);
            status = CheckDir(job.Destination, true); // Приемников
            si.UpdateStatus(status);
            if (si.Status == StatusBackup.Fatal)
            {
                logger.Error("Task finished with status {0}", si.Status);
                return si.Status;
            }
            // Определение типа копирования
            Manifest manifest = new Manifest(job);
            TypeBackup BakType = manifest.GetCurrentTypeBackup(job.Plan);
            logger.Info("Backup type: {0}", BakType);
            if (BakType == TypeBackup.Differential) // Текущее копирование не полное
            {
                // Чутка подправим фильтр
                DateTime dt = manifest.GetLastFullDate();
                job.SourceFilter.NewerThan = dt;
            }

            // Имя архива
            string ShortArhName = job.GetArhName(BakType.ToString());
            string[] files = job.GetFiles(); // файлы для обработки

            if (files.Length<1) // нечего копировать
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
                logger.Error("Task finished with status {0}", si.Status);
                return si.Status;
            }

            // Архивация может идти в TempDir или в первый каталог приемника
            string arhDir = job.GetArhWorkDir();
            string FullArhName = Path.Combine(arhDir, ShortArhName);

            StatusBackup sb2 = StatusBackup.OK;
            if (job.Shadow)
            {
                logger.Info("Shadow copy using");
                try
                {
                    if (!job.isSourceVSS()) // для источников нельзя использовать теневое копирование
                    {
                        logger.Error("Source can not to be snapshoted. Check: 1. Too many volumes to shadow, use only one 2. Non fixed disk, use only local");
                        return StatusBackup.Fatal;
                    }
                    string freeLetter = FileManage.Volumes.GetFreeLetter(Config.Instance.mnConfig.ExposeVolume);
                    if (String.IsNullOrEmpty(freeLetter))
                    {
                        logger.Error("No free letter to map snapshot");
                        return StatusBackup.Fatal;
                    }
                    using (Snapshot vss = new Snapshot(job.Source[0], freeLetter))
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
            // Создаем информацию о копировании
            string[] arhFiles = FileManage.GetArhFiles(FullArhName);
            BackupInfo bakEntry = new BackupInfo(BakType, si.Status, arhFiles);
            // Копируем архив в другие приемники
            int start=String.Compare(job.Destination[0],arhDir,true)==0 ? 1:0;
            int i,k;
            string fromFile,toFile;
            for (i = start; i < job.Destination.Length; i++)
            {
                // Копируются файлы
                for (k = 0; k < arhFiles.Length; k++)
                {
                    fromFile = Path.Combine(arhDir, arhFiles[k]);
                    toFile = Path.Combine(job.Destination[i], arhFiles[k]);
                    FileManage.FileCopy(fromFile, toFile);
                }
                
                
            }
            
            // Добавляем запись в манифест
            manifest.Add(bakEntry);
            // Удаляем старые архивы
            //StatusBackup sb = StatusBackup.OK;
            StatusBackup sb = manifest.DeleteOld(job.Plan.Store);
            si.UpdateStatus(sb);

            manifest.Save();
            
            
            


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

        
        /// <summary>
        /// Проверка существования каталогов источника (isDestination-false) или приемника (isDestination-true)
        /// Попытка создания не существующих, запись в лог
        /// </summary>
        /// <param name="dirs"></param>
        /// <param name="isDestination">false-источник, true-приемник</param>
        /// <returns></returns>
        private StatusBackup CheckDir(string[] dirs,bool isDestination)
        {
            if (dirs.Length < 1) return StatusBackup.Fatal;
            StatusInfo<StatusBackup> si = new StatusInfo<StatusBackup>(StatusBackup.OK);
            int i = 0;
            foreach (string dir in dirs)
            {
                // Проверка существования каталогов
                if (!Directory.Exists(dir))
                {
                    if (isDestination)
                    {
                        // Попробовать создать каталог
                        bool cr = FileManage.DirectoryCreate(dir);
                        if (!cr)
                        {
                            logger.Error("Directory does not exist {0}", dir);
                            si.UpdateStatus(StatusBackup.Error);
                            i++;
                        }
                    }
                    else
                    {
                        si.UpdateStatus(StatusBackup.Error);
                        logger.Error("Directory does not exist {0}", dir);
                        i++;
                    }
                }
            }
            if (i == dirs.Length)
            {
                si.UpdateStatus(StatusBackup.Fatal); // Ни одного каталога нет
                logger.Error("Task finished with status {0}", si.Status);
            }
            
            return si.Status;

        }

        





    }
}
