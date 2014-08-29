using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace mnBackupLib
{
    /// <summary>
    /// План архивирования
    /// </summary>
    [DataContract]
    public class BackupPlan
    {
        /// <summary>
        /// Тип копирования
        /// </summary>
        [DataMember]
        public TypeBackup Type;
        /// <summary>
        /// Интервал в днях между полными копиями
        /// </summary>
        [DataMember]
        public Period FullIntervalMake;
        /// <summary>
        /// Колчичество хранимых полных бэкапов
        /// </summary>
        [DataMember]
        public Period FullIntervalSave;

        public BackupPlan()
        {
            Type = TypeBackup.Full;
            FullIntervalMake = new Period(Period.PeriodName.Day);
            FullIntervalSave = new Period(Period.PeriodName.Week);
        }
        /// <summary>
        /// Возвращает тип копирования который нужно сделать
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        public TypeBackup GetCurrentTypeBackup(Manifest manifest)
        {
            if (Type == TypeBackup.Full) return TypeBackup.Full;
            bool has=FullIntervalMake.IsInInterval(manifest.GetLastFullDate());
            if (has) return TypeBackup.Full;
            else return TypeBackup.Differential;
            /*
            DateTime nextFull = manifest.GetLastFullDate().AddDays(FullInterval);
            if (nextFull > DateTime.Now)
                return TypeBackup.Differential;
            else 
                return TypeBackup.Full;
             */ 
        }

    }
}
