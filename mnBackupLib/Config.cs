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

        /// Вызовет защищенный конструктор класса Singleton
        private Config() { Init(); }
        
        /// <summary>
        /// Чтение конфигурации из файла
        /// </summary>
        private void Init()
        {
            
            Configuration conf;

            ExeConfigurationFileMap emap=new ExeConfigurationFileMap();
            
            emap.ExeConfigFilename="mnBackup.config";

            conf = ConfigurationManager.OpenMappedExeConfiguration(emap, ConfigurationUserLevel.None); 

            _mnConfig = conf.GetSection("mnBackup") as  ConfSection;

            
            if (_mnConfig == null)
            {
                _mnConfig = new ConfSection();
                
            }

            
        }
        /// <summary>
        /// Перекрыть настройки опциями командной строки
        /// </summary>
        /// <param name="options"></param>
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
                   
                }
            }

           
        }

        
        

    }
}
