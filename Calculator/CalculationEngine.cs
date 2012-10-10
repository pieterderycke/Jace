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
        public double Calculate(string function)
        {
            return Calculate(function, new Dictionary<string, double>());
        }

        public double Calculate(string function, Dictionary<string, double> variables)
        {
            Operation operation = BuildAbstractSyntaxTree(function);

            IInterpreter interpreter = new BasicInterpreter();
            return interpreter.Execute(operation, variables);
        }

        public FunctionBuilder Function(string functionText)
        {
            return new FunctionBuilder(functionText, this);
        }

        /// <summary>
        /// Build the abstract syntax tree for a given function. The function string will
        /// be first tokenized.
        /// </summary>
        /// <param name="functionText">A string containing the mathematical formula that must be converted into an abstract syntax tree.</param>
        /// <returns>The abstract syntax tree of the formula.</returns>
        internal Operation BuildAbstractSyntaxTree(string functionText)
        {
            TokenReader tokenReader = new TokenReader();
            List<object> tokens = tokenReader.Read(functionText);

            AstBuilder astBuilder = new AstBuilder();
            return astBuilder.Build(tokens);
        }
    }
}
