using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

//using CommandLine;
//using CommandLine.Text;

namespace mnBackupLib
{
    /// <summary>
    /// Конфигурация
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Имя файла с заданиями по умолчанию
        /// </summary>
        public const string DEFAULT_TASK_FILENAME = "mnbackup.json";
        /// <summary>
        /// Интервал между полными копиями по умолчанию
        /// </summary>
        public const string DEFAULT_FULL_INTERVAL = "1w";
        /// <summary>
        /// время хранения полных копий
        /// </summary>
        public const string DEFAULT_FULL_STORE = "1m";

        Configuration conf;

        /// <summary>
        /// Файл с заданиями
        /// </summary>
        public string TaskFileName
        {
            get { return _taskFileName; }
        }
        private string _taskFileName;

        /// <summary>
        /// Опции командной строки
        /// </summary>
        public Options options { get; set; }

        public Config()
        {
            Init();
            options = new Options();
        }

        public Config(Options opt)
        {
            Init();
            options = opt;
            ReadOptions();
        }

        private void Init()
        {
            conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        private void ReadOptions()
        {
            
            // Менять в рантайме так:
            //config.AppSettings.Settings["CurrentPromoId"].Value = promo_id.ToString();
            


            if (!String.IsNullOrEmpty(options.RunOpt.TaskFile))
            {
                conf.AppSettings.Settings["TaskFile"].Value = options.RunOpt.TaskFile;
                //this._taskFileName = options.RunOpt.TaskFile;
            }
        }

        public var GetValue(string Key)
        {
            // DefaultTaskFileName
            return conf.AppSettings.Settings[Key];
        }
    }
}
