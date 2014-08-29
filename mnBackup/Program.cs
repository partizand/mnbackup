using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mnBackupLib;
using System.Runtime.Serialization.Json;
using System.IO;

namespace mnBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            Backup bak = new Backup();


            // Тест
            Task job = new Task("test","d:\\temp\\testmn\\source","d:\\temp\\testmn\\dest");
            //job.SourceFilter.IncludeFileMask = "*";

            bak.Tasks.Tasks.Add(job);
            
            bak.Start();
            
            
        }
    }
}
