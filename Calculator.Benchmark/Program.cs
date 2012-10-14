using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Calculator.Benchmark
{
    class Program
    {
        private const int NumberOfTests = 1000000;

        static void Main(string[] args)
        {
            ConsoleColor defaultForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Jace Benchmark Application");
            Console.ForegroundColor = defaultForegroundColor;
            Console.WriteLine();

            Console.WriteLine("--------------------");
            Console.WriteLine("Function: {0}", "2+3*7/23");
            Console.WriteLine("Number Of Tests: {0}", NumberOfTests);
            Console.WriteLine();

            Console.WriteLine("Interpreted Mode:");
            CalculationEngine interpretedEngine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            BenchMarkCalculationEngine(interpretedEngine, "2+3*7");

            Console.WriteLine("Compiled Mode:");
            CalculationEngine compiledEngine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            BenchMarkCalculationEngine(compiledEngine, "2+3*7");

            Console.WriteLine("--------------------");
            Console.WriteLine("Function: {0}", "(var1 + var2 * 3)/(2+3) - something");
            Console.WriteLine("Number Of Tests: {0}", NumberOfTests);
            Console.WriteLine();

            Console.WriteLine("Interpreted Mode:");
            BenchMarkCalculationEngineFunctionBuild(interpretedEngine, "(var1 + var2 * 3)/(2+3) - something");

            Console.WriteLine("Compiled Mode:");
            BenchMarkCalculationEngineFunctionBuild(compiledEngine, "(var1 + var2 * 3)/(2+3) - something");


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

            Func<int, int, int, double> function = (Func<int, int, int, double>)engine.Function(functionText)
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
    }
}
