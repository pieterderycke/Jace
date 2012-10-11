using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Calculator.Execution;
using Calculator.Operations;
using Calculator.Tokenizer;

namespace Calculator
{
    public class CalculationEngine
    {
        private readonly IExecutor executor;
        private readonly CultureInfo cultureInfo;

        public CalculationEngine()
            : this(CultureInfo.CurrentCulture, ExecutionMode.Interpreted)
        {
        }

        public CalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode)
        {
            this.cultureInfo = cultureInfo;

            if (executionMode == ExecutionMode.Interpreted)
                executor = new Interpreter();
            else if (executionMode == ExecutionMode.Compiled)
                executor = new DynamicCompiler();
            else
                throw new ArgumentException(string.Format("Unsupported execution mode \"\".", executionMode), 
                    "executionMode");
        }

        public double Calculate(string function)
        {
            return Calculate(function, new Dictionary<string, double>());
        }

        public double Calculate(string function, Dictionary<string, double> variables)
        {
            Operation operation = BuildAbstractSyntaxTree(function);

            return executor.Execute(operation, variables);
        }

        public FunctionBuilder Function(string functionText)
        {
            return new FunctionBuilder(functionText, this);
        }

        public Func<Dictionary<string, double>, double> Build(string functionText)
        {
            Operation operation = BuildAbstractSyntaxTree(functionText);

            return variables => executor.Execute(operation, variables);
        }

        /// <summary>
        /// Build the abstract syntax tree for a given function. The function string will
        /// be first tokenized.
        /// </summary>
        /// <param name="functionText">A string containing the mathematical formula that must be converted into an abstract syntax tree.</param>
        /// <returns>The abstract syntax tree of the formula.</returns>
        private Operation BuildAbstractSyntaxTree(string functionText)
        {
            TokenReader tokenReader = new TokenReader(cultureInfo);
            List<object> tokens = tokenReader.Read(functionText);

            AstBuilder astBuilder = new AstBuilder();
            return astBuilder.Build(tokens);
        }
    }
}
