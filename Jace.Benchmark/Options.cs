using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jace.Benchmark
{
    class Options
    {
        [Option("case-sensitivity", HelpText = "Execute in case sensitive mode, case insensitive mode or execute both.")]
        public CaseSensitivity CaseSensitivity { get; set; }

        [Option('f', "file", HelpText = "The file to store the output results in.", Required = true)]
        public string FileName { get; set; }
    }
}
