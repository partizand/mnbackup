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
        public string[] BackupFileNames
        { get; set; }
        

        /// <summary>
        /// Запись о информации с копированием
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="typeBackup"></param>
        /// <param name="status"></param>
        /// <param name="backupFileNames"></param>
        public BakEntryInfo(DateTime dt,TypeBackup typeBackup, StatusBackup status,string[] backupFileNames)
        {
            this.BackupDate = dt;
            this.TypeBackup = typeBackup;
            this.Status = status;
            BackupFileNames = new string[backupFileNames.Length];
            backupFileNames.CopyTo(this.BackupFileNames,0);
            //this.BackupFileNames. = new string[(backupFileNames)];
            
        }
        /// <summary>
        /// Создать запись о информации с копированием с текущей датой
        /// </summary>
        /// <param name="typeBackup"></param>
        /// <param name="status"></param>
        /// <param name="backupFileNames"></param>
        public BakEntryInfo(TypeBackup typeBackup, StatusBackup status, string[] backupFileNames)
        {
            this.BackupDate = DateTime.Now;
            this.TypeBackup = typeBackup;
            this.Status = status;
            BackupFileNames = new string[backupFileNames.Length];
            backupFileNames.CopyTo(this.BackupFileNames, 0);
            //this.BackupFileNames = new List<string>(backupFileNames);

        }

        #region Equals ovveride

        public int CompareTo(BakEntryInfo other)
        {
            
            return BackupDate.CompareTo(other.BackupDate);
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            BakEntryInfo p = obj as BakEntryInfo;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (BackupDate == p.BackupDate);
        }

        public bool Equals(BakEntryInfo p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (BackupDate == p.BackupDate);
        }

        public override int GetHashCode()
        {
            return BackupDate.GetHashCode();
        }

        #endregion

    }
}
