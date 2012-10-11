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
            Console.WriteLine("Function: {0}", "2+3*7/23");
            Console.WriteLine("Number Of Tests: {0}", NumberOfTests);

            Console.WriteLine("Interpreted Mode:");
            CalculationEngine interpretedEngine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            BenchMarkCalculationEngine(interpretedEngine, "2+3*7");

            Console.WriteLine("Compiled Mode:");
            CalculationEngine compiledEngine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            BenchMarkCalculationEngine(compiledEngine, "2+3*7");

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
    }
}
