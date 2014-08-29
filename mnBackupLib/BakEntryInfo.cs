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
        public DateTime BackupDate;
        /// <summary>
        /// Тип бэкапа
        /// </summary>
        [DataMember]
        public TypeBackup TypeBackup;
        /// <summary>
        /// Чем все закончилось
        /// </summary>
        [DataMember]
        public StatusBackup Status;
        /// <summary>
        /// Имя файла/каталога с бэкапом
        /// </summary>
        [DataMember]
        public string BackupFileName;
        

        public BakEntryInfo(DateTime dt,  StatusBackup status,string backupFileName)
        {
            BackupDate = dt;
            TypeBackup = TypeBackup.Full;
            Status = status;
            BackupFileName = backupFileName;
            
        }
        
        public int CompareTo(BakEntryInfo other)
        {
            
            return BackupDate.CompareTo(other.BackupDate);
        }


    }
}
