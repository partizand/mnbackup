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
        {
            get { return _backupDate; }
        }
        /// <summary>
        /// Дата записи
        /// </summary>
        private DateTime _backupDate;

        /// <summary>
        /// Тип бэкапа
        /// </summary>
        [DataMember]
        public TypeBackup TypeBackup
        {
            get { return _typeBackup; }
        }
        /// <summary>
        /// Тип бэкапа
        /// </summary>
        private TypeBackup _typeBackup;

        /// <summary>
        /// Чем все закончилось
        /// </summary>
        [DataMember]
        public StatusBackup Status
        {
            get { return _status; }
        }
        /// <summary>
        /// Чем все закончилось
        /// </summary>
        private StatusBackup _status;

        /// <summary>
        /// Имя файла/каталога с бэкапом
        /// </summary>
        [DataMember]
        public string BackupFileName
        {
            get { return _backupFileName; }
        }
        /// <summary>
        /// Имя файла/каталога с бэкапом
        /// </summary>
        private string _backupFileName;

        
        public BakEntryInfo(DateTime dt,TypeBackup typeBackup, StatusBackup status,string backupFileName)
        {
            _backupDate = dt;
            _typeBackup = typeBackup;
            _status = status;
            _backupFileName = backupFileName;
            
        }

        public BakEntryInfo(TypeBackup typeBackup, StatusBackup status, string backupFileName)
        {
            _backupDate = DateTime.Today;
            _typeBackup = typeBackup;
            _status = status;
            _backupFileName = backupFileName;

        }
        
        public int CompareTo(BakEntryInfo other)
        {
            
            return BackupDate.CompareTo(other.BackupDate);
        }


    }
}
