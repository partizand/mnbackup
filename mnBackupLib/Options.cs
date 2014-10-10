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
            SaveOpt = new SaveSubOptions();
        }

        

        [VerbOption("task", HelpText = "Run task from command line.")]
        public TaskSubOptions TaskOpt { get; set; }

        [VerbOption("run", HelpText = "Run tasks from file.")]
        public RunSubOptions RunOpt { get; set; }

        [VerbOption("save", HelpText = "Save task to file")]
        public SaveSubOptions SaveOpt { get; set; }

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
        [Option("ExposeVolume", MetaValue = "DriveLetter", HelpText = "Volume letter to mount snapshot of shadow copy")]
        public string ExposeVolume { get; set; }

        [Option("TempDir", HelpText = "Temp dir for create archives")]
        public string TempDir { get; set; }
        
        /*
        [Option("MailHost", HelpText = "Mail server name for send reports")]
        public string MailHost { get; set; }

        [Option("MailAddr", HelpText = "Email to send reports")]
        public string MailAddr { get; set; }
         */ 
    }

    public class SaveSubOptions : TaskSubOptions
    {
        [Option('f', "file", MetaValue = "FILE", Required = true, HelpText = "Filename to save task")]
        public string TaskFile { get; set; }
    }

    /// <summary>
    /// Параметры командной строки для описания задачи из командной строки
    /// </summary>
    public class TaskSubOptions:CommonSubOptions
    {
        [Option('s', "source", MetaValue = "Dir", Required = true, HelpText = "Source dirs delimeted ;")]
        public string Source { get; set; }

        [Option('d', "dest", MetaValue = "Dir", Required = true, HelpText = "Destination dir")]
        public string Destination { get; set; }

        [Option('i',"include", MetaValue = "Mask", HelpText = "Include file masks, delimeted ;")]
        public string IncludeFileMask { get; set; }

        [Option('e', "exclude", MetaValue = "Mask", HelpText = "Exclude file masks, delimeted ;")]
        public string ExcludeFileMask { get; set; }

        [Option("older", MetaValue = "Date", HelpText = "Select files older than")]
        public DateTime? OlderThan { get; set; }

        [Option("newer", MetaValue = "Date", HelpText = "Select files newer than")]
        public DateTime? NewerThan { get; set; }

        [Option("prefix", MetaValue = "name", HelpText = "Prefix or name of task")]
        public string Prefix { get; set; }

        [Option('t', "type", DefaultValue = TypeBackup.Full, HelpText = "Type backup Full|Differential")]
        public TypeBackup typeBackup { get; set; }

        [Option('v', "VolumeSize", HelpText = "Sets the size in bytes of an archive volume (0 for no volumes, default)")]
        public int? VolumeSize { get; set; }

        [Option("Interval", MetaValue = "period", HelpText = "Period in days between full backup for diff backup. May use 1d,1w,1m or number days")]
        public string Interval { get; set; }

        [Option("Store", MetaValue = "period", HelpText = "Period in days to store full backups. May use 1d,1w,1m or number days. 0 - store all backups")]
        public string Store { get; set; }

        [Option("shadow",  HelpText = "Use volume shadow copying.")]
        public bool Shadow { get; set; }

        [Option('p',"pass", MetaValue = "password", HelpText = "Set password on archive")]
        public string Password { get; set; }

        
    }
    /// <summary>
    /// Параметры командной строки для запуска задач из файлов
    /// </summary>
    public sealed class RunSubOptions : CommonSubOptions
    {
        [Option('f', "file", MetaValue = "FILE", HelpText = "Task file")]
        public string TaskFile { get; set; }


    }
}
