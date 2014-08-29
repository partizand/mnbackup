using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace mnBackupLib
{
    /// <summary>
    /// Набор заданий
    /// </summary>
    public class TaskList
    {
        public List<Task> Tasks;

        public TaskList()
        {
            Tasks = new List<Task>();
        }


        public void Clear()
        {
            Tasks.Clear();
        }

        /// <summary>
        /// Записывает список заданий в файл (JSON)
        /// </summary>
        /// <param name="FileName"></param>
        public void Save(string FileName)
        {
            
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Task>));
            try
            {
                FileStream fs = new FileStream(FileName, FileMode.Create);
                serializer.WriteObject(fs, Tasks);
                fs.Close();
            }
            catch
            {

            }
        }
        /// <summary>
        /// Читает список заданий из файла (JSON)
        /// Текущий список не очищается
        /// </summary>
        /// <param name="FileName"></param>
        public void Read(string FileName)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Task>));
            if (File.Exists(FileName))
            {
                FileStream fs = new FileStream(FileName, FileMode.Open);
                //List<Task> t=new 
                Tasks.AddRange((List<Task>)serializer.ReadObject(fs));
                fs.Close();
            }
        }
    }
}
