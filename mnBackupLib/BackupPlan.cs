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
        public TypeBackup Type { get; set; }
        /// <summary>
        /// Интервал в днях между полными копиями
        /// </summary>
        [DataMember]
        public Period FullIntervalMake { get; set; }
        /// <summary>
        /// Период между полными бэкапами
        /// </summary>
        [DataMember]
        public Period FullIntervalSave { get; set; }

        public BackupPlan()
        {
            Type = TypeBackup.Full;
            FullIntervalMake = new Period(Period.PeriodName.Day);
            FullIntervalSave = new Period(Period.PeriodName.Week);
        }
        
    

    }
}
