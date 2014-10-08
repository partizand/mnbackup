using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Runtime.Serialization.Json;
using System.IO;

namespace mnBackupLib
{
    /// <summary>
    /// Чтение/запись информации о бэкапах
    /// </summary>
    public class Manifest
    {

        #region Properties

        private ManifestInfo[] Mans;
       
        #endregion
        /*
        public Manifest(string manifestFile,string[] DestDirs)
        {
            ManifestFile = manifestFile;
            _ManifestDir = Path.GetDirectoryName(manifestFile);
            
            //Lines =  new List<BakEntryInfo>();
            
            Lines = SerialIO.Read<List<BackupInfo>>(ManifestFile);
            if (Lines == null) Lines = new List<BackupInfo>();
            
            Lines.Sort();
        }
         */ 
        public Manifest(Task job)
        {
            string ShortManifestFile = job.GetManifestFile();
            //_ManifestDir = job.Destination[0];

            

            string fullManFile;

            //Lines =  new List<BakEntryInfo>();
            //Lines = Array.CreateInstance(typeof(List<BackupInfo>),2) ;
            
            int i;
            List<ManifestInfo> LMans = new List<ManifestInfo>();
            for (i = 0; i < job.Destination.Length; i++)
            {
                fullManFile = Path.Combine(job.Destination[i], ShortManifestFile);
                LMans.Add(new ManifestInfo(fullManFile));
            }
            Mans = LMans.ToArray();
            
        }
        
        /// <summary>
        /// Запись манифеста
        /// </summary>
        public bool Save()
        {
            
            StatusInfo<bool> si = new StatusInfo<bool>(false);
            bool ret;
            for (int i = 0; i < Mans.Length; i++)
            {
                
                ret = Mans[i].Save();
                si.UpdateStatus(!ret);
                
            }
            return !si.Status;
        }
        /// <summary>
        /// Добавить запись об одном копировании
        /// </summary>
        /// <param name="bakEntry"></param>
        public void Add(BackupInfo bakEntry)
        {
            for (int i = 0; i < Mans.Length; i++)
            {
                Mans[i].Add(bakEntry);
                
            }
        }
        
        

        /// <summary>
        /// Удалить информацию о копировании
        /// </summary>
        /// <param name="dateBackup"></param>
        public void Delete(DateTime dateBackup)
        {
            for (int i = 0; i < Mans.Length; i++)
            {
                Mans[i].Delete(dateBackup);
            }
        }

        /// <summary>
        /// Удалить старые архивы
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public StatusBackup DeleteOld(Period store)
        {
            int i;
            StatusInfo<StatusBackup> si = new StatusInfo<StatusBackup>(StatusBackup.OK);
            
            for (i = 0; i < Mans.Length; i++)
            {
                si.UpdateStatus(Mans[i].DeleteOld(store));
            }

            return si.Status;
        }


        /// <summary>
        /// Дата последнего полного копирования
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastFullDate()
        {
            return Mans[0].GetLastFullDate();
        }
        /// <summary>
        /// Возвращает все копирования не попадающие в период (для удаления)
        /// Учитывается полное копирование. Разностные копирования удаляются вместе с полным, 
        /// даже если они попадают в период
        /// </summary>
        /// <param name="FullIntervalSave"></param>
        /// <returns></returns>
        public BackupInfo[] GetAllBeforePeriod(Period FullIntervalSave)
        {
            return Mans[0].GetAllBeforePeriod(FullIntervalSave);
   
        }
        /// <summary>
        /// Возвращает тип копирования который нужно сделать
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        public TypeBackup GetCurrentTypeBackup(BackupPlan backupPlan)
        {
            return Mans[0].GetCurrentTypeBackup(backupPlan);

        }
        
        

    }

}
