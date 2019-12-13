using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Jace.Execution;
using OfficeOpenXml;

namespace Jace.Benchmark
{
    class Program
    {
        private const int NumberOfTests = 1000000;
        //private const int NumberOfTests = 1000;
        private const int NumberOfFunctionsToGenerate = 10000;
        private const int NumberExecutionsPerRandomFunction = 1000;

        private static DataTable table;

        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => {
                    table = new DataTable();
                    table.Columns.Add("Mode");
                    table.Columns.Add("Case Sensitive");
                    table.Columns.Add("Formula");
                    table.Columns.Add("Iterations per Random Formula", typeof(int));
                    table.Columns.Add("Total Iteration", typeof(int));
                    table.Columns.Add("Total Duration");

                    Benchmark(options.CaseSensitivity);

                    using (var excel = new ExcelPackage())
                    {
                        var worksheet = excel.Workbook.Worksheets.Add("Results");
                        worksheet.Cells.LoadFromDataTable(table, true);

                        worksheet.Cells["A1:F1"].Style.Font.Bold = true;

                        for (int i = 0; i < table.Columns.Count; i++)
                            worksheet.Column(i + 1).AutoFit();

                        excel.SaveAs(new FileInfo(options.FileName));
                    }
            });
        }

        private static void Benchmark(CaseSensitivity caseSensitivity)
        { 
            TimeSpan duration;

            CalculationEngine interpretedEngine = new CalculationEngine(CultureInfo.CurrentCulture, ExecutionMode.Interpreted, true, true, true);
            CalculationEngine interpretedEngineCaseSensitive = new CalculationEngine(CultureInfo.CurrentCulture, ExecutionMode.Interpreted, true, true, false);
            CalculationEngine compiledEngine = new CalculationEngine(CultureInfo.CurrentCulture, ExecutionMode.Compiled, true, true, true);
            CalculationEngine compiledEngineCaseSensitive = new CalculationEngine(CultureInfo.CurrentCulture, ExecutionMode.Compiled, true, true, false);

            //Console.WriteLine("Mode;Formula;Iterations per Random Formula;Total Iterations;Total Duration");

            BenchMarkOperation[] benchmarks = {
                new BenchMarkOperation() { Formula = "2+3*7", BenchMarkDelegate = BenchMarkCalculationEngine },
                new BenchMarkOperation() { Formula = "(var1 + var2 * 3)/(2+3) - something", BenchMarkDelegate = BenchMarkCalculationEngineFunctionBuild },
            };

            string[] formulas = { "2+3*7" };

            foreach(BenchMarkOperation benchmark in benchmarks)
            {
                if (caseSensitivity == CaseSensitivity.All || caseSensitivity == CaseSensitivity.CaseInSensitive)
                {
                    duration = benchmark.BenchMarkDelegate(interpretedEngine, benchmark.Formula);
                    //Console.WriteLine("Interpreted Mode;{0};;{1};{2}", benchmark.Formula, NumberOfTests, duration);
                    AddBenchmarkRecord("Interpreted", false, benchmark.Formula, null, NumberOfTests, duration);
                }

                if (caseSensitivity == CaseSensitivity.All || caseSensitivity == CaseSensitivity.CaseSensitive)
                {
                    duration = benchmark.BenchMarkDelegate(interpretedEngineCaseSensitive, benchmark.Formula);
                    //Console.WriteLine("Interpreted Mode (Case Sensitive);{0};;{1};{2}", benchmark.Formula, NumberOfTests, duration);
                    AddBenchmarkRecord("Interpreted", true, benchmark.Formula, null, NumberOfTests, duration);
                }

                if (caseSensitivity == CaseSensitivity.All || caseSensitivity == CaseSensitivity.CaseInSensitive)
                {
                    duration = benchmark.BenchMarkDelegate(compiledEngine, benchmark.Formula);
                    //Console.WriteLine("Compiled Mode;{0};;{1};{2}", benchmark.Formula, NumberOfTests, duration);
                    AddBenchmarkRecord("Compiled", false, benchmark.Formula, null, NumberOfTests, duration);
                }

                if (caseSensitivity == CaseSensitivity.All || caseSensitivity == CaseSensitivity.CaseSensitive)
                {
                    duration = benchmark.BenchMarkDelegate(compiledEngineCaseSensitive, benchmark.Formula);
                    //Console.WriteLine("Compiled Mode (Case Sensitive);{0};;{1};{2}", benchmark.Formula, NumberOfTests, duration);
                    AddBenchmarkRecord("Compiled", true, benchmark.Formula, null, NumberOfTests, duration);
                }
            }

            //List<string> functions = GenerateRandomFunctions(NumberOfFunctionsToGenerate);

            ////Interpreted Mode
            //duration = BenchMarkCalculationEngineRandomFunctionBuild(interpretedEngine, functions, NumberExecutionsPerRandomFunction);
            ////Console.WriteLine("Interpreted Mode;Random Mode {0} functions 3 variables;{1};{2};{3}", NumberOfFunctionsToGenerate, NumberExecutionsPerRandomFunction,
            ////    NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration, NumberOfFunctionsToGenerate);
            //AddBenchmarkRecord("Interpreted", false, string.Format("Random Mode {0} functions 3 variables", NumberOfFunctionsToGenerate),
            //    NumberExecutionsPerRandomFunction, NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration);


            ////Interpreted Mode(Case Sensitive)
            //duration = BenchMarkCalculationEngineRandomFunctionBuild(interpretedEngineCaseSensitive, functions, NumberExecutionsPerRandomFunction);
            //Console.WriteLine("Interpreted Mode (Case Sensitive);Random Mode {0} functions 3 variables;{1};{2};{3}", NumberOfFunctionsToGenerate, NumberExecutionsPerRandomFunction,
            //    NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration, NumberOfFunctionsToGenerate);
            //AddBenchmarkRecord("Interpreted", true, string.Format("Random Mode {0} functions 3 variables", NumberOfFunctionsToGenerate),
            //    NumberExecutionsPerRandomFunction, NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration);

            ////Compiled Mode
            //duration = BenchMarkCalculationEngineRandomFunctionBuild(compiledEngine, functions, NumberExecutionsPerRandomFunction);
            ////Console.WriteLine("Compiled Mode;Random Mode {0} functions 3 variables;{1};{2};{3}", NumberOfFunctionsToGenerate, NumberExecutionsPerRandomFunction,
            ////    NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration, NumberOfFunctionsToGenerate);
            //AddBenchmarkRecord("Compiled", false, string.Format("Random Mode {0} functions 3 variables", NumberOfFunctionsToGenerate),
            //    NumberExecutionsPerRandomFunction, NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration);

            ////Compiled Mode(Case Sensitive)
            //duration = BenchMarkCalculationEngineRandomFunctionBuild(compiledEngineCaseSensitive, functions, NumberExecutionsPerRandomFunction);
            ////Console.WriteLine("Compiled Mode (Case Sensitive;Random Mode {0} functions 3 variables;{1};{2};{3}", NumberOfFunctionsToGenerate, NumberExecutionsPerRandomFunction,
            ////    NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration, NumberOfFunctionsToGenerate);
            //AddBenchmarkRecord("Compiled", true, string.Format("Random Mode {0} functions 3 variables", NumberOfFunctionsToGenerate),
            //    NumberExecutionsPerRandomFunction, NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration);
        }

        private static TimeSpan BenchMarkCalculationEngine(CalculationEngine engine, string functionText)
        {
            DateTime start = DateTime.Now;

            for (int i = 0; i < NumberOfTests; i++)
            {
                engine.Calculate(functionText);
            }

            DateTime end = DateTime.Now;

            return end - start;
        }

        private static TimeSpan BenchMarkCalculationEngineFunctionBuild(CalculationEngine engine, string functionText)
        {
            DateTime start = DateTime.Now;

            Func<int, int, int, double> function = (Func<int, int, int, double>)engine.Formula(functionText)
                .Parameter("var1", DataType.Integer)
                .Parameter("var2", DataType.Integer)
                .Parameter("something", DataType.Integer)
                .Result(DataType.FloatingPoint)
                .Build();

            Random random = new Random();

            for (int i = 0; i < NumberOfTests; i++)
            {
                function(random.Next(), random.Next(), random.Next());
            }

            DateTime end = DateTime.Now;

            return end - start;
        }

        private static List<string> GenerateRandomFunctions(int numberOfFunctions)
        {
            List<string> result = new List<string>();
            FunctionGenerator generator = new FunctionGenerator();

            for (int i = 0; i < numberOfFunctions; i++)
               result.Add(generator.Next());

            return result;
        }

        private static TimeSpan BenchMarkCalculationEngineRandomFunctionBuild(CalculationEngine engine, List<string> functions, 
            int numberOfTests)
        {
            Random random = new Random();

            DateTime start = DateTime.Now;

            Parallel.ForEach(functions,(functionText)=>
                {
                    Func<int, int, int, double> function = (Func<int, int, int, double>)engine.Formula(functionText)
                        .Parameter("var1", DataType.Integer)
                        .Parameter("var2", DataType.Integer)
                        .Parameter("var3", DataType.Integer)
                        .Result(DataType.FloatingPoint)
                        .Build();

                    for (int i = 0; i < numberOfTests; i++)
                    {
                        function(random.Next(), random.Next(), random.Next());
                    }
                });

            DateTime end = DateTime.Now;

            return end - start;
        }

        private static void AddBenchmarkRecord(string mode, bool caseSensitive, string formula, int? iterationsPerRandom, int totalIterations, TimeSpan duration)
        {
            table.Rows.Add(mode, caseSensitive, formula, iterationsPerRandom, totalIterations, duration);
        }
    }
}
