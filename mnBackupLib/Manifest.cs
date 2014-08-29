using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;

namespace mnBackupLib
{
    /// <summary>
    /// Чтение/запись информации о бэкапах
    /// </summary>
    public class Manifest
    {
        
        //public List<ManifestLine> Lines;

        List<BakEntryInfo> Lines;
        //List<BakEntryInfo> DiffLines;



        string ManifestFile;

        public Manifest(string manifestFile)
        {
            ManifestFile = manifestFile;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<BakEntryInfo>));
            Lines = new List<BakEntryInfo>();
            if (File.Exists(ManifestFile))
            {
                FileStream fs = new FileStream(ManifestFile, FileMode.Open);
                //List<Task> t=new 
                Lines.AddRange((List<BakEntryInfo>)serializer.ReadObject(fs));
                fs.Close();
            }
            //FullLines=new List<BakEntryInfo>(Lines.FindAll(obj => obj.TypeBackup == TypeBackup.Full));
            //DiffLines=new List<BakEntryInfo>(Lines.FindAll(obj => obj.TypeBackup != TypeBackup.Full));
            Lines.Sort();
        }
        /*
        public Manifest(string DestDir)
        {
            ManifestFile = Path.Combine(DestDir, Backup.ManifestFile);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<ManifestLine>));
            Lines = new List<ManifestLine>();
            if (File.Exists(ManifestFile))
            {
                FileStream fs = new FileStream(ManifestFile, FileMode.Open);
                //List<Task> t=new 
                Lines.AddRange((List<ManifestLine>)serializer.ReadObject(fs));
                fs.Close();
            }
        }
         */ 
        /// <summary>
        /// Запись манифеста
        /// </summary>
        public void Save()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<BakEntryInfo>));
            //List<BakEntryInfo> Lines = new List<BakEntryInfo>(FullLines);
            //Lines.AddRange(DiffLines);
            try
            {
                FileStream fs = new FileStream(ManifestFile, FileMode.Create);
                serializer.WriteObject(fs, Lines);
                fs.Close();
            }
            catch
            {

            }
        }
        /// <summary>
        /// Добавить информацию об одном бэкапе
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="typeBackup"></param>
        /// <param name="status"></param>
        public void Add(DateTime dt, TypeBackup typeBackup, StatusBackup status,string backupFileName)
        {
            
            
            Lines.Add(new BakEntryInfo(dt, status, backupFileName));
            
            //Lines.Add(new ManifestLine(dt,typeBackup,status,backupFileName));
        }
        /// <summary>
        /// Добавить информацию об одном бэкапе с текущим временем
        /// </summary>
        /// <param name="typeBackup"></param>
        /// <param name="status"></param>
        public void Add(TypeBackup typeBackup, StatusBackup status,string backupFileName)
        {
            Add(DateTime.Now, typeBackup, status, backupFileName);
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
            List<BakEntryInfo> full = Lines.FindAll(obj => obj.TypeBackup == TypeBackup.Full);
            return full.Max(obj => obj.BackupDate);
        }
        /// <summary>
        /// Возвращает все копирования не попадающие в период (для удаления)
        /// Учитывается полное копирование. Разностные копирования удаляются вместе с полным, 
        /// даже если они попадают в период
        /// </summary>
        /// <param name="FullIntervalSave"></param>
        /// <returns></returns>
        public BakEntryInfo[] GetAllBeforePeriod(Period FullIntervalSave)
        {
            if (FullIntervalSave.IntervalValue == 0) return null;
            DateTime dtBefore = FullIntervalSave.SubFromDate(DateTime.Today);
            // Индекс первого полного копирования которое нужно оставить
            int iLastFullToStay = Lines.FindIndex(obj => obj.BackupDate >= dtBefore && obj.TypeBackup == TypeBackup.Full);
            // Все за этим индексом удалить
            List<BakEntryInfo> fullToDelete = Lines.GetRange(0, iLastFullToStay );
            
            if (fullToDelete == null) return null;
            if (fullToDelete.Count == 0) return null;


            return fullToDelete.ToArray();

            

           
        }
        
        /// <summary>
        /// Возвращает инфу о копировании по дате
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        /*
        private ManifestLine GetByDate2(DateTime dt)
        {
            return Lines.Find(obj => obj.BackupDate == dt);
        }
         */ 

    }

}
