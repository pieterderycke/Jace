using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.Operations;
using Calculator.Tokenizer;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            CalculationEngine engine = new CalculationEngine();
            Func<int, double, double> function = (Func<int, double, double>)engine.Function("var1+2*(3*age)")
                .Parameter("var1", DataType.Integer)
                .Parameter("var2", DataType.FloatingPoint)
                .Result(DataType.FloatingPoint).Build();

            double result = function(2, 2.3);

            //TokenReader tokenReader = new TokenReader();
            //List<object> tokens = tokenReader.Read("(42+8)* 2");

            //AstBuilder builder = new AstBuilder();
            //Operation operation = builder.Build(tokens);

            //IInterpreter interpreter = new BasicInterpreter();
            //double result = interpreter.Execute(operation);

            //Console.WriteLine("Result: {0}", result);

            //Console.WriteLine("Press enter to exit...");
            //Console.ReadLine();
        }
    }
}
