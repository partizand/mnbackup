using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using NLog;

namespace mnBackupLib
{
    /// <summary>
    /// Чтение/запись (сериализация) объектов в файлы/потоки
    /// </summary>
    public class SerialIO
    {


        /// <summary>
        /// Записать объект класса T в файл. Тип определяется по расширению. По умолчанию JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FileName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Save<T>(string FileName,T obj)
        {
            string ext = Path.GetExtension(FileName).ToLower(); // Расширение с точкой

            switch (ext)
            {
                case ".json":
                    return SaveJSON(FileName,obj);

                default:
                    return SaveJSON(FileName, obj);

            }


        }

        /// <summary>
        /// Записать объект класса T в файл JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FileName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool SaveJSON<T>(string FileName,T obj)
        {
            try
            {
                FileStream fs = new FileStream(FileName, FileMode.Create);
                SaveJSON(fs, obj);  
                fs.Close();
                return true;
            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error("Error saving to JSON file {0}. Exception {1}", FileName, e.Message);
                return false;
            }
        }
        /// <summary>
        /// Записать объект класса T в поток JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="obj"></param>
        public static void SaveJSON<T>(Stream stream, T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(stream, obj);
        }

        /// <summary>
        /// Прочитать объект класса T из файла. Тип определяется по расширению. По умолчанию JSON
        /// </summary>
        /// <typeparam name="T">Имя типа</typeparam>
        /// <param name="FileName">Имя файла</param>
        /// <returns></returns>
        public static T Read<T>(string FileName)
        {
            string ext = Path.GetExtension(FileName).ToLower(); // Расширение с точкой
            
            switch (ext)
            {
                case ".json":
                    return ReadJSON<T>(FileName);
                    
                default:
                    return ReadJSON<T>(FileName);
                    
            }
        }
        
        /// <summary>
        /// Прочитать объект класса T из файла JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static T ReadJSON<T>(string FileName)
        {
            if (File.Exists(FileName))
            {

                FileStream fs = new FileStream(FileName, FileMode.Open);
                T obj = ReadJSON<T>(fs);
                fs.Close();
                return obj;
            }
            else
            {
                
                return default(T);
            }
        }
        /// <summary>
        /// Прочитать объект класса T из потока JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T ReadJSON<T>(Stream stream)
        {
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                T obj = (T)serializer.ReadObject(stream);
                return obj;
            }
            catch
            {
                return default(T);
            }
            
        }

    }
}
