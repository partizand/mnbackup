using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace mnBackupLib
{
    /// <summary>
    /// Запись об одном копировании
    /// </summary>
    [DataContract]
    public class BakEntryInfo : IComparable<BakEntryInfo>

    {
        /// <summary>
        /// Дата записи
        /// </summary>
        [DataMember]
        public DateTime BackupDate
        {get;set;}
        

        /// <summary>
        /// Тип бэкапа
        /// </summary>
        [DataMember]
        public TypeBackup TypeBackup
        { get; set; }
        
        /// <summary>
        /// Чем все закончилось
        /// </summary>
        [DataMember]
        public StatusBackup Status
        { get; set; }
        

        /// <summary>
        /// Имя файла/каталога с бэкапом
        /// </summary>
        [DataMember]
        public string BackupFileName
        { get; set; }
        

        
        public BakEntryInfo(DateTime dt,TypeBackup typeBackup, StatusBackup status,string backupFileName)
        {
            this.BackupDate = dt;
            this.TypeBackup = typeBackup;
            this.Status = status;
            this.BackupFileName = backupFileName;
            
        }

        public BakEntryInfo(TypeBackup typeBackup, StatusBackup status, string backupFileName)
        {
            this.BackupDate = DateTime.Now;
            this.TypeBackup = typeBackup;
            this.Status = status;
            this.BackupFileName = backupFileName;

        }
        
        public int CompareTo(BakEntryInfo other)
        {
            
            return BackupDate.CompareTo(other.BackupDate);
        }


    }
}
