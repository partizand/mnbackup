using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using CommandLine;

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

            // Все ключи конфигурации
            string[] allkeys = conf.AppSettings.Settings.AllKeys;
            System.Reflection.PropertyInfo[] props=options.GetType().GetProperties();

            foreach (string key in allkeys)
            {
                // Ищем свойство
                System.Reflection.PropertyInfo prop=props.FirstOrDefault(obj => String.Compare(obj.Name, key, true) == 0);
                if (prop != null)
                {
                    var value=prop.GetValue(null,null);
                    if (value != null)
                    {
                        
                        string str=Properties.Settings.Default.TaskFileName;
                        //ConfigurationManager. .AppSettings["t"].
                        //conf.AppSettings.Settings[key]. Value = prop.GetValue(null, null);
                    }
                }
            }

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
        public T Get<T>(string key)
        {
            var appSetting = ConfigurationManager.AppSettings[key];
            if (String.IsNullOrWhiteSpace(appSetting)) throw new AppSettingNotFoundException(key);

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)(converter.ConvertFromInvariantString(appSetting));
        }

    }
}
