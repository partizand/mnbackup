using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using System.Runtime.Serialization;

namespace mnBackupLib
{
    /// <summary>
    /// Настройки опций командной строки
    /// </summary>
    public sealed class Options
    {

        public Options()
        {
            // Since we create this instance the parser will not overwrite it
            TaskOpt = new TaskSubOptions();// { Patch = true };
            RunOpt = new RunSubOptions();
        }

        

        [VerbOption("task", HelpText = "Run task from command line.")]
        public TaskSubOptions TaskOpt { get; set; }

        [VerbOption("run", HelpText = "Run tasks from file.")]
        public RunSubOptions RunOpt { get; set; }

        

        

                   

            //
            // Marking a property of type IParserState with ParserStateAttribute allows you to
            // receive an instance of ParserState (that contains a IList<ParsingError>).
            // This is equivalent from inheriting from CommandLineOptionsBase (of previous versions)
            // with the advantage to not propagating a type of the library.
            //
            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpVerbOption]
            public string GetUsage(string verb)
            {
                return HelpText.AutoBuild(this, verb);
            }
        /*
            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
            }
         */ 
    }
    /// <summary>
    /// Общие параметры командной строки
    /// </summary>
    abstract public class CommonSubOptions
    {
        [Option('e', "ExposeVolume", MetaValue = "DriveLetter", HelpText = "Volume letter to mount snapshot of shadow copy")]
        public string ExposeVolume { get; set; }
        
        [Option("MailHost", HelpText = "Mail server name for send reports")]
        public string MailHost { get; set; }

        [Option("MailAddr", HelpText = "Email to send reports")]
        public string MailAddr { get; set; }
    }
    /// <summary>
    /// Параметры командной строки для описания задачи из командной строки
    /// </summary>
    public sealed class TaskSubOptions:CommonSubOptions
    {
        [Option('s', "source", MetaValue = "Dir", Required = true, HelpText = "Source dirs delimeted ;")]
        public string Source { get; set; }

        [Option('d', "dest", MetaValue = "Dir", Required = true, HelpText = "Destination dir")]
        public string Destination { get; set; }

        [Option("prefix", MetaValue = "name", HelpText = "Prefix or name of task")]
        public string Prefix { get; set; }

        [Option('t', "type", DefaultValue = TypeBackup.Full, HelpText = "Type backup Full|Differential")]
        public TypeBackup typeBackup { get; set; }

        [Option("Interval", MetaValue = "period", HelpText = "Period in days between full backup for diff backup. May use 1d,1w,1m or number days")]
        public string Interval { get; set; }

        [Option("Store", MetaValue = "period", HelpText = "Period in days to store full backups. May use 1d,1w,1m or number days. 0 - store all backups")]
        public string Store { get; set; }

        [Option("shadow",  HelpText = "Use volume shadow copying.")]
        public bool Shadow { get; set; }

        
    }
    /// <summary>
    /// Параметры командной строки для запуска задач из файлов
    /// </summary>
    public sealed class RunSubOptions : CommonSubOptions
    {
        [Option('f', "file", MetaValue = "FILE", DefaultValue = Config.DEFAULT_TASK_FILENAME, HelpText = "Task file")]
        public string TaskFile { get; set; }


    }
}
