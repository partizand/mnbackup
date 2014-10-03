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

        [VerbOption("task", HelpText = "Set task from command line.")]
        public TaskSubOptions TaskOpt { get; set; }

        [VerbOption("run", HelpText = "Run task from file.")]
        public RunSubOptions RunOpt { get; set; }

        [Option("ExposeVolume", HelpText = "Volume letter to mount snapshot")]
        public string ExposeVolume { get; set; }

        [Option("MailHost", HelpText = "Mail server name for send reports")]
        public string MailHost { get; set; }

        [Option("MailAddr", HelpText = "Email to send reports")]
        public string MailAddr { get; set; }




            /*
            [Option('r', "read", MetaValue = "FILE", Required = true, HelpText = "Input file with data to process.")]
            public string InputFile { get; set; }

            [Option('w', "write", MetaValue = "FILE", HelpText = "Output FILE with processed data (otherwise standard output).")]
            public string OutputFile { get; set; }

            [Option("calculate", HelpText = "Add results in bottom of tabular data.")]
            public bool Calculate { get; set; }

            [Option('v', MetaValue = "INT", HelpText = "Verbose level. Range: from 0 to 2.")]
            public int? VerboseLevel { get; set; }

            [Option("i", HelpText = "If file has errors don't stop processing.")]
            public bool IgnoreErrors { get; set; }

            [Option('j', "jump", MetaValue = "INT", DefaultValue = 0, HelpText = "Data processing start offset.")]
            public double StartOffset { get; set; }

            [Option("optimize", HelpText = "Optimize for Speed|Accuracy.")]
            public OptimizeFor Optimization { get; set; }

            [ValueList(typeof(List<string>))]
            public IList<string> DefinitionFiles { get; set; }

            [OptionList('o', "operators", Separator = ';', HelpText = "Operators included in processing (+;-;...)." +
                " Separate each operator with a semicolon." + " Do not include spaces between operators and separator.")]
            public IList<string> AllowedOperators { get; set; }
             */

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

    public class TaskSubOptions
    {
        [Option('s', "source", MetaValue = "Dir", Required = true, HelpText = "Source dirs delimeted ;")]
        public string Source { get; set; }

        [Option('d', "dest", MetaValue = "Dir", Required = true, HelpText = "Destination dir")]
        public string Destination { get; set; }

        [Option('t', "type", DefaultValue = TypeBackup.Full, HelpText = "Type backup Full|Differential")]
        public TypeBackup typeBackup { get; set; }

        [Option("Interval", MetaValue = "period", DefaultValue=Config.DEFAULT_FULL_INTERVAL, HelpText = "Period in days between full backup for diff backup. May use 1d,1w,1m or number days")]
        public string Interval { get; set; }

        [Option("Store", MetaValue = "period", DefaultValue = Config.DEFAULT_FULL_STORE, HelpText = "Period in days to store full backups. May use 1d,1w,1m or number days. 0 - store all backups")]
        public string Store { get; set; }

        [Option("shadow", DefaultValue =false, HelpText = "Use volume shadow copying.")]
        public bool Shadow { get; set; }

        
    }

    public class RunSubOptions
    {
        [Option('f', "file", MetaValue = "FILE", DefaultValue = Config.DEFAULT_TASK_FILENAME, HelpText = "Task file")]
        public string TaskFile { get; set; }


    }
}
