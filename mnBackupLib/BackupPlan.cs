﻿using System;
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
        public TimePeriod Interval { get; set; }
        /// <summary>
        /// Период между полными бэкапами
        /// </summary>
        [DataMember]
        public TimePeriod Store { get; set; }

        public BackupPlan()
        {
            Type = TypeBackup.Full;
            Interval = new TimePeriod(Config.Instance.mnConfig.Interval);
            Store = new TimePeriod(Config.Instance.mnConfig.Store);
        }
        
    

    }
}
