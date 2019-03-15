using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jace.Execution;

namespace Jace.Benchmark
{
    class Program
    {
        private const int NumberOfTests = 1000000;
        private const int NumberOfFunctionsToGenerate = 10000;
        private const int NumberExecutionsPerRandomFunction = 1000;

        static void Main(string[] args)
        {
            ConsoleColor defaultForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Jace.NET Benchmark Application");
            Console.ForegroundColor = defaultForegroundColor;
            Console.WriteLine();

            Console.WriteLine("--------------------");
            Console.WriteLine("Function: {0}", "2+3*7/23");
            Console.WriteLine("Number Of Tests: {0}", NumberOfTests.ToString("N0"));
            Console.WriteLine();

            //Console.WriteLine("Interpreted Mode:");
            //CalculationEngine interpretedEngine = new CalculationEngine(CultureInfo.CurrentCulture, ExecutionMode.Interpreted, true, true, true);
            //BenchMarkCalculationEngine(interpretedEngine, "2+3*7");

            //Console.WriteLine("Interpreted Mode(Case Sensitive):");
            //CalculationEngine interpretedEngineCaseSensitive = new CalculationEngine(CultureInfo.CurrentCulture, ExecutionMode.Interpreted, true, true, false);
            //BenchMarkCalculationEngine(interpretedEngineCaseSensitive, "2+3*7");

            Console.WriteLine("Compiled Mode:");
            CalculationEngine compiledEngine = new CalculationEngine(CultureInfo.CurrentCulture, ExecutionMode.Compiled, true, true, true);
            BenchMarkCalculationEngine(compiledEngine, "2+3*7");

            Console.WriteLine("Compiled Mode(Case Sensitive):");
            CalculationEngine compiledEngineCaseSensitive = new CalculationEngine(CultureInfo.CurrentCulture, ExecutionMode.Compiled, true, true, false);
            BenchMarkCalculationEngine(compiledEngineCaseSensitive, "2+3*7");

            //Console.WriteLine("--------------------");
            //Console.WriteLine("Function: {0}", "(var1 + var2 * 3)/(2+3) - something");
            //Console.WriteLine("Number Of Tests: {0}", NumberOfTests.ToString("N0"));
            //Console.WriteLine();

            //Console.WriteLine("Interpreted Mode:");
            //BenchMarkCalculationEngineFunctionBuild(interpretedEngine, "(var1 + var2 * 3)/(2+3) - something");

            //Console.WriteLine("Interpreted Mode(Case Sensitive):");
            //BenchMarkCalculationEngineFunctionBuild(interpretedEngineCaseSensitive, "(var1 + var2 * 3)/(2+3) - something");

            Console.WriteLine("Compiled Mode:");
            BenchMarkCalculationEngineFunctionBuild(compiledEngine, "(var1 + var2 * 3)/(2+3) - something");

            Console.WriteLine("Compiled Mode(Case Sensitive):");
            BenchMarkCalculationEngineFunctionBuild(compiledEngineCaseSensitive, "(var1 + var2 * 3)/(2+3) - something");

            //Console.WriteLine("--------------------");
            //Console.WriteLine("Random Generated Functions: {0}", NumberOfFunctionsToGenerate.ToString("N0"));
            //Console.WriteLine("Number Of Variables Of Each Function: {0}", 3);
            //Console.WriteLine("Number Of Executions For Each Function: {0}", NumberExecutionsPerRandomFunction.ToString("N0"));
            //Console.WriteLine("Total Number Of Executions: {0}", (NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate).ToString("N0"));
            //Console.WriteLine("Parallel: {0}", true);
            //Console.WriteLine();

            List<string> functions = GenerateRandomFunctions(NumberOfFunctionsToGenerate);

            //Console.WriteLine("Interpreted Mode:");
            //BenchMarkCalculationEngineRandomFunctionBuild(interpretedEngine, functions, NumberExecutionsPerRandomFunction);

            //Console.WriteLine("Interpreted Mode(Case Sensitive):");
            //BenchMarkCalculationEngineRandomFunctionBuild(interpretedEngineCaseSensitive, functions, NumberExecutionsPerRandomFunction);

            Console.WriteLine("Compiled Mode:");
            BenchMarkCalculationEngineRandomFunctionBuild(compiledEngine, functions, NumberExecutionsPerRandomFunction);

            Console.WriteLine("Compiled Mode(Case Sensitive):");
            BenchMarkCalculationEngineRandomFunctionBuild(compiledEngineCaseSensitive, functions, NumberExecutionsPerRandomFunction);

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static void BenchMarkCalculationEngine(CalculationEngine engine, string functionText)
        {
            DateTime start = DateTime.Now;

            for (int i = 0; i < NumberOfTests; i++)
            {
                engine.Calculate(functionText);
            }

            DateTime end = DateTime.Now;

            Console.WriteLine("Total duration: {0}", end - start);
        }

        private static void BenchMarkCalculationEngineFunctionBuild(CalculationEngine engine, string functionText)
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

            Console.WriteLine("Total duration: {0}", end - start);
        }

        private static List<string> GenerateRandomFunctions(int numberOfFunctions)
        {
            List<string> result = new List<string>();
            FunctionGenerator generator = new FunctionGenerator();

            for (int i = 0; i < numberOfFunctions; i++)
               result.Add(generator.Next());

            return result;
        }

        private static void BenchMarkCalculationEngineRandomFunctionBuild(CalculationEngine engine, List<string> functions, 
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

            Console.WriteLine("Total duration: {0}", end - start);
        }
    }
}
