using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mnBackupLib;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using CommandLine;
using CommandLine.Text;

namespace mnBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            

            

            // Тест
            //Task job = new Task("test","d:\\temp\\testmn\\source","d:\\temp\\testmn\\dest");
            //job.SourceFilter.IncludeFileMask = "*";


            //System.Configuration.
            //ConfigurationManager.

            Options options = new Options ();
            ParserSettings set = new ParserSettings();

            

            Parser parser = new CommandLine.Parser  ( with => with.HelpWriter = Console.Error);


            string invokedVerb="";
            object invokedVerbInstance=new TaskSubOptions();

            //OnVerbT verb=new OnVerbT(on

            //CommandLine.Parser.Default.ParseArguments(args, options,

            bool a = CommandLine.Parser.Default.ParseArguments(args, options,
              (verb, subOptions) =>
              {
                  // if parsing succeeds the verb name and correct instance
                  // will be passed to onVerbCommand delegate (string,object)
                  invokedVerb = verb;
                  invokedVerbInstance = subOptions;

              });

            if (!a)
            {
                
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }
            
            Backup bak = new Backup();

            if (invokedVerb == "task") // запуск одного задания из командной строки
            {
                var commitSubOptions = (TaskSubOptions)invokedVerbInstance;
                
                Config.Instance.MergeOptions(commitSubOptions);
                //Console.WriteLine("Source is " + commitSubOptions.Source);
                //Console.WriteLine("Source2 is " + options.TaskOpt.Source);

                Task job = new Task(commitSubOptions);
                

                bak.StartTask(job);
            }

            if (invokedVerb == "run") // запуск заданий из файла
            {
                var commitSubOptions = (RunSubOptions)invokedVerbInstance;
                Config.Instance.MergeOptions(commitSubOptions);
                bak.Read(commitSubOptions.TaskFile);
                bak.Start();
            }
            
            
            
            
        }
        
    
        
        
    }
}
