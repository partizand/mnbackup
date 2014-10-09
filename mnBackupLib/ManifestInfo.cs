using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NLog;

namespace mnBackupLib
{
    /// <summary>
    /// Информация об одном манифесте
    /// </summary>
    public class ManifestInfo
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Записи о бэкапах
        /// </summary>
        List<BackupInfo> Lines;

        /// <summary>
        /// Имя файла с манифестом
        /// </summary>
        public string ManifestFileName
        {
            get { return _ManifestFile; }
        }
        /// <summary>
        /// Имя файла с манифестом private
        /// </summary>
        private string _ManifestFile;

        /// <summary>
        /// Каталоги приемники с архивами и манифестами
        /// </summary>
        public string DestDir { get {return _DestDir;} }
        string _DestDir;

        public ManifestInfo(string manifestFile)
        {
            _ManifestFile = manifestFile;
            _DestDir = Path.GetDirectoryName(manifestFile);

            //Lines =  new List<BakEntryInfo>();

            Lines = SerialIO.Read<List<BackupInfo>>(_ManifestFile);
            if (Lines == null) Lines = new List<BackupInfo>();

            Lines.Sort();
        }
        /// <summary>
        /// Запись манифеста
        /// </summary>
        public bool Save()
        {
            return SerialIO.Save(_ManifestFile, Lines);
            
        }
        /// <summary>
        /// Добавить запись об одном копировании
        /// </summary>
        /// <param name="bakEntry"></param>
        public void Add(BackupInfo bakEntry)
        {
            
                Lines.Add(bakEntry);
                Lines.Sort();
            
        }

        /// <summary>
        /// Удалить информацию о копировании
        /// </summary>
        /// <param name="dateBackup"></param>
        public void Delete(DateTime dateBackup)
        {
           
                int k = Lines.FindIndex(obj => obj.BackupDate == dateBackup);
                if (k > -1) Lines.RemoveAt(k);
            
        }

        public StatusBackup DeleteOld(TimePeriod store)
        {
            // Какие архивы нужно удалить
            //StatusBackup st = StatusBackup.OK;
            StatusInfo<StatusBackup> si = new StatusInfo<StatusBackup>(StatusBackup.OK);
            BackupInfo[] baks = GetAllBeforePeriod(store); // Записи к удалению
            if (baks == null) return si.Status;
            if (baks.Length == 0) return si.Status;
            string fullArhName;
            int i, k;
            bool suc;
            for (i = 0; i < baks.Length; i++) // Перебираются все строчки, которые нужно удалить
            {
                

                    for (k = 0; k < baks[i].BackupFileNames.Length; k++) // Перебираются все файлы записи
                    {

                        fullArhName = Path.Combine(DestDir, baks[i].BackupFileNames[k]);
                        suc = FileManage.FileDelete(fullArhName);
                        if (suc)
                        {
                            logger.Info("Deleted archive {0}", fullArhName);
                            Delete(baks[i].BackupDate);

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

        /// <summary>
        /// Дата последнего полного копирования
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastFullDate()
        {
            List<BackupInfo> full = Lines.FindAll(obj => obj.TypeBackup == TypeBackup.Full);
            return full.Max(obj => obj.BackupDate);
        }
        /// <summary>
        /// Возвращает все копирования не попадающие в период (для удаления)
        /// Учитывается полное копирование. Разностные копирования удаляются вместе с полным, 
        /// даже если они попадают в период
        /// </summary>
        /// <param name="FullIntervalSave"></param>
        /// <returns></returns>
        public BackupInfo[] GetAllBeforePeriod(TimePeriod FullIntervalSave)
        {
            if (FullIntervalSave.isEmpty()) return null;
            DateTime dtBefore = FullIntervalSave.SubFromDate(DateTime.Today);
            // Индекс первого полного копирования которое нужно оставить
            int iLastFullToStay = Lines.FindIndex(obj => obj.BackupDate >= dtBefore && obj.TypeBackup == TypeBackup.Full);
            // Все за этим индексом удалить
            if (iLastFullToStay > -1)
            {
                List<BackupInfo> fullToDelete = Lines.GetRange(0, iLastFullToStay);
                if (fullToDelete == null) return null;
                return fullToDelete.ToArray();
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// Возвращает тип копирования который нужно сделать
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        public TypeBackup GetCurrentTypeBackup(BackupPlan backupPlan)
        {
            if (backupPlan.Type == TypeBackup.Full) return TypeBackup.Full;
            bool has = backupPlan.Interval.IsInInterval(GetLastFullDate());
            if (has) return TypeBackup.Full;
            else return TypeBackup.Differential;

        }




    }
}
