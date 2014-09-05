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
    partial class Program
    {
        static void Main(string[] args)
        {
            Backup bak = new Backup();

            

            // Тест
            Task job = new Task("test","d:\\temp\\testmn\\source","d:\\temp\\testmn\\dest");
            //job.SourceFilter.IncludeFileMask = "*";


            //System.Configuration.
            //ConfigurationManager.

            Options options = new Options ();
            ParserSettings set = new ParserSettings();

            options = Options.Read("");

            Parser parser = new CommandLine.Parser  ( with => with.HelpWriter = Console.Error);

            if (parser.ParseArgumentsStrict(args, options))
            {
                Run(options);
            }
            else
            {
                Environment.Exit(-2);
            }

            //bak.Add(job);
            
            //bak.Start();
            
            
        }
        static void Run(Options options)
        {
            /*
            if (String.IsNullOrEmpty(options.ConfigFile))
            {
                options.ConfigFile = "conf.json";
            }
             */ 
            Console.WriteLine("Conf file is " +options.ConfigFile);

            options.Save("NewOpt.json");

        }
    }
}
