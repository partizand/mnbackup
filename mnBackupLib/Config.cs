using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using CommandLine;
//using Nini.Config;

//using CommandLine;
//using CommandLine.Text;

namespace mnBackupLib
{
    /// <summary>
    /// Конфигурация
    /// </summary>
    public class Config:Singleton<Config>
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

        public ConfSection mnConfig{get {return _mnConfig;}}

        ConfSection _mnConfig;

        //IConfigSource NConf;


        
        /// <summary>
        /// Опции командной строки
        /// </summary>
        //public Options options { get; set; }

        /// Вызовет защищенный конструктор класса Singleton
        private Config() { Init(); }
        /*
        public Config()
        {
            Init();
            //options = new Options();
            
        }
         */ 
        /*
        public Config(Options opt)
        {
            Init();
            options = opt;
            ReadOptions();
        }
        */
        private void Init()
        {
            //IConfigSource source = new IniConfigSource("mnBackup.ini");

            //source.Configs[0].

            Configuration conf;

            ExeConfigurationFileMap emap=new ExeConfigurationFileMap();
            
            emap.ExeConfigFilename="mnBackup.config";

            conf = ConfigurationManager.OpenMappedExeConfiguration(emap, ConfigurationUserLevel.None); // OpenExeConfiguration( OpenExeConfiguration(ConfigurationUserLevel.None);

            _mnConfig = conf.GetSection("mnBackup") as  ConfSection;

            //ConfSection section = ConfigurationManager.GetSection("example") as ConfSection;
            if (_mnConfig == null)
            {
                _mnConfig = new ConfSection();
                
            }

            //conf.AppSettings.Settings.
        }

        


        public void MergeOptions(CommonSubOptions options)
        {
            
            // Менять в рантайме так:
            //config.AppSettings.Settings["CurrentPromoId"].Value = promo_id.ToString();

            // Все ключи конфигурации
            System.Reflection.PropertyInfo[] propsXML = _mnConfig.GetType().GetProperties();// SectionInformation.GetType().GetProperties();
            System.Reflection.PropertyInfo[] propsOpt=options.GetType().GetProperties();

            int i;

            for (i=0;i<propsXML.Length;i++)
            {
            
            
                // Ищем свойство
                System.Reflection.PropertyInfo propOpt = propsOpt.FirstOrDefault(obj => String.Compare(obj.Name, propsXML[i].Name, true) == 0);
                if (propOpt != null)
                {
                    var obj = propOpt.GetValue(options, null);
                    if (obj != null)
                    {
                        propsXML[i].SetValue(_mnConfig, obj, null);
                    }
                    /*
                    var value=prop.GetValue(null,null);
                    if (value != null)
                    {
                        
                        string str=Properties.Settings.Default.TaskFileName;
                        
                        //ConfigurationManager. .AppSettings["t"].
                        conf.AppSettings.Settings[key].Value = prop.GetValue(null, null).ToString();
                    }
                     */ 
                }
            }

            /*
            if (!String.IsNullOrEmpty(options.RunOpt.TaskFile))
            {
                conf.AppSettings.Settings["TaskFile"].Value = options.RunOpt.TaskFile;
                //this._taskFileName = options.RunOpt.TaskFile;
            }
            */
        }

        
        

    }
}
