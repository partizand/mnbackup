using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using mnBackupLib;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Json;
using System.IO;

namespace mnBackup
{
    partial class Program
    {
        /*
        private enum OptimizeFor
        {
            Unspecified,
            Speed,
            Accuracy
        }
        */
        [DataContract]
        private sealed class Options
        {
            //public const string DefaultConfigFileName = "conf.json";

            [DataMember]
            [Option('c', "conf", MetaValue = "FILE", DefaultValue = Backup.DefaultConfigFileName, HelpText = "Config file.")]
            public string ConfigFile { get; set; }

            [Option('r', "run", Required = true, HelpText = "Run backup.")]
            public bool RunBackup { get; set; }

            


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

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
            }

            /// <summary>
            /// Записать настройки в файл 
            /// </summary>
            /// <param name="FileName"></param>
            public bool Save(string FileName)
            {

                return SerialIO.Save(FileName, this);
                
            }
            /// <summary>
            /// Прочитать настройки из файла.
            /// </summary>
            /// <param name="FileName"></param>
            public static Options Read(string FileName)
            {
                string fName = FileName;
                if (String.IsNullOrEmpty(FileName)) fName = "conf.json";
                Options opt = SerialIO.Read<Options>(fName);
                if (opt == null)
                    opt = new Options();
                return opt;
                
            }
            

            

        }
    }
}
