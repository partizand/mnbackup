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
            options = new Options();
        }

        public Config(Options opt)
        {
            options = opt;
            ReadOptions();
        }

        private void ReadOptions()
        {
            if (!String.IsNullOrEmpty(options.RunOpt.TaskFile))
            {
                this._taskFileName = options.RunOpt.TaskFile;
            }
        }

        public string GetValue(string Key)
        {
            // DefaultTaskFileName
            return ConfigurationManager.AppSettings[Key];
        }
    }
}
