using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace mnBackupLib
{
    /// <summary>
    /// Секция конфигурации в файле настроек
    /// </summary>
    public class ConfSection : ConfigurationSection
    {
        #region Default values

        /// <summary>
        /// Имя файла с заданиями по умолчанию
        /// </summary>
        public const string DEFAULT_TASK_FILENAME = "mnbackup.json";
        /// <summary>
        /// Интервал между полными копиями по умолчанию
        /// </summary>
        public const string DEFAULT_INTERVAL = "1w";
        /// <summary>
        /// время хранения полных копий по умолчанию
        /// </summary>
        public const string DEFAULT_STORE = "1m";

        #endregion

        #region Constructors
        /// <summary>
        /// Predefines the valid properties and prepares
        /// the property collection.
        /// </summary>
        static ConfSection()
        {
            // Predefine properties here
            // Predefine properties here
            
           
            s_propExposeVolume = new ConfigurationProperty(
                "ExposeVolume",
                typeof(string),
                String.Empty,
                ConfigurationPropertyOptions.None
                
            );

            s_propInterval = new ConfigurationProperty (
                "Interval",
                typeof(string),
                DEFAULT_INTERVAL,
                ConfigurationPropertyOptions.None
                
            );

            s_propStore = new ConfigurationProperty(
                "Store",
                typeof(string),
                DEFAULT_STORE,
                ConfigurationPropertyOptions.None

            );

            s_propTaskFile = new ConfigurationProperty(
                "TaskFile",
                typeof(string),
                DEFAULT_TASK_FILENAME,
                ConfigurationPropertyOptions.None

            );

            s_propString = new ConfigurationProperty(
                "stringValue",
                typeof(string),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            s_propBool = new ConfigurationProperty(
                "boolValue",
                typeof(bool),
                false,
                ConfigurationPropertyOptions.None
            );

            s_propTimeSpan = new ConfigurationProperty(
                "timeSpanValue",
                typeof(TimeSpan),
                null,
                ConfigurationPropertyOptions.None
            );

            s_properties = new ConfigurationPropertyCollection();
            s_properties.Add(s_propExposeVolume);
            s_properties.Add(s_propInterval);
            s_properties.Add(s_propStore);
            s_properties.Add(s_propTaskFile);

            s_properties.Add(s_propString);
            s_properties.Add(s_propBool);
            s_properties.Add(s_propTimeSpan);



            
           

        }
        #endregion

        #region Static Fields
        /// <summary>
        /// Диск, свободный в системе для подключения shadowcopy тома
        /// </summary>
        private static ConfigurationProperty s_propExposeVolume;
        private static ConfigurationProperty s_propInterval;
        private static ConfigurationProperty s_propStore;
        private static ConfigurationProperty s_propTaskFile;

        private static ConfigurationProperty s_propString;
        private static ConfigurationProperty s_propBool;
        private static ConfigurationProperty s_propTimeSpan;

        private static ConfigurationPropertyCollection s_properties;
        #endregion


        #region Properties
        /// <summary>
        /// Диск, свободный в системе для подключения shadowcopy тома
        /// </summary>
        [ConfigurationProperty("ExposeVolume")]
        public string ExposeVolume
        {
            get { return (string)base[s_propExposeVolume]; }
            set { base[s_propExposeVolume] = value; }
        }
        /// <summary>
        /// Интервал между полными копиями по умолчанию
        /// </summary>
        [ConfigurationProperty("Interval")]
        public string Interval
        {
            get { return (string)base[s_propInterval]; }
            set { base[s_propInterval] = value; }
        }
        /// <summary>
        /// время хранения полных копий по умолчанию
        /// </summary>
        [ConfigurationProperty("Store")]
        public string Store
        {
            get { return (string)base[s_propInterval]; }
            set { base[s_propInterval] = value; }
        }
        /// <summary>
        /// Имя файла с заданиями по умолчанию
        /// </summary>
        [ConfigurationProperty("TaskFile")]
        public string TaskFile
        {
            get { return (string)base[s_propInterval]; }
            set { base[s_propInterval] = value; }
        }

        /// <summary>
        /// Gets the StringValue setting.
        /// </summary>
        [ConfigurationProperty("stringValue", IsRequired = true)]
        public string StringValue
        {
            get { return (string)base[s_propString]; }
        }

        /// <summary>
        /// Gets the BooleanValue setting.
        /// </summary>
        [ConfigurationProperty("boolValue")]
        public bool BooleanValue
        {
            get { return (bool)base[s_propBool]; }
        }

        /// <summary>
        /// Gets the TimeSpanValue setting.
        /// </summary>
        [ConfigurationProperty("timeSpanValue")]
        public TimeSpan TimeSpanValue
        {
            get { return (TimeSpan)base[s_propTimeSpan]; }
        }

        /// <summary>
        /// Override the Properties collection and return our custom one.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return s_properties; }
        }
        #endregion
    }
}
