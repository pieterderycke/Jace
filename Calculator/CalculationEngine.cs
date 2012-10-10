using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.Execution;
using Calculator.Operations;
using Calculator.Tokenizer;

namespace Calculator
{
    public class CalculationEngine
    {
        private readonly IInterpreter interpreter;

        public CalculationEngine()
            : this(ExecutionMode.Interpreted)
        {
        }

        public CalculationEngine(ExecutionMode executionMode)
        {
            if (executionMode == ExecutionMode.Interpreted)
                interpreter = new BasicInterpreter();
            else
                throw new ArgumentException("Unsupported execution mode", "executionMode");
        }

        public double Calculate(string function)
        {
            return Calculate(function, new Dictionary<string, double>());
        }

        public double Calculate(string function, Dictionary<string, double> variables)
        {
            Operation operation = BuildAbstractSyntaxTree(function);

            return interpreter.Execute(operation, variables);
        }

        public FunctionBuilder Function(string functionText)
        {
            return new FunctionBuilder(functionText, this);
        }

        public Func<Dictionary<string, double>, double> Build(string functionText)
        {
            Operation operation = BuildAbstractSyntaxTree(functionText);

            return variables => interpreter.Execute(operation, variables);
        }

        /// <summary>
        /// Build the abstract syntax tree for a given function. The function string will
        /// be first tokenized.
        /// </summary>
        /// <param name="functionText">A string containing the mathematical formula that must be converted into an abstract syntax tree.</param>
        /// <returns>The abstract syntax tree of the formula.</returns>
        private Operation BuildAbstractSyntaxTree(string functionText)
        {
            TokenReader tokenReader = new TokenReader();
            List<object> tokens = tokenReader.Read(functionText);

            AstBuilder astBuilder = new AstBuilder();
            return astBuilder.Build(tokens);
        }
    }
}
