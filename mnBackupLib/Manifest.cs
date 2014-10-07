﻿using System;
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


        /// <summary>
        /// Каталог с файлами бэкапа данного манифеста, расположение манифеста
        /// </summary>
        public string ManifestDir { get { return _ManifestDir; } }
        /// <summary>
        /// Каталог с файлами бэкапа данного манифеста, расположение манифеста private
        /// </summary>
        private string _ManifestDir;
        
        /// <summary>
        /// Записи о бэкапах
        /// </summary>
        List<BackupInfo> Lines;

        /// <summary>
        /// Имя файла с манифестом
        /// </summary>
        public string ManifestFileName
        {
            get {return ManifestFile;}
        }
        /// <summary>
        /// Имя файла с манифестом private
        /// </summary>
        private string ManifestFile;

        #endregion

        public Manifest(string manifestFile)
        {
            ManifestFile = manifestFile;
            _ManifestDir = Path.GetDirectoryName(manifestFile);
            
            //Lines =  new List<BakEntryInfo>();
            Lines = SerialIO.Read<List<BackupInfo>>(ManifestFile);
            if (Lines == null) Lines = new List<BackupInfo>();
            
            Lines.Sort();
        }
        
        /// <summary>
        /// Запись манифеста
        /// </summary>
        public bool Save()
        {
            return SerialIO.Save(ManifestFile, Lines);
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
            int i=Lines.FindIndex(obj => obj.BackupDate == dateBackup);
            if (i > -1) Lines.RemoveAt(i);
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
        public BackupInfo[] GetAllBeforePeriod(Period FullIntervalSave)
        {
            if (FullIntervalSave.IntervalValue == 0) return null;
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
            
            //if (fullToDelete.Count == 0) return null;


           

            

           
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
