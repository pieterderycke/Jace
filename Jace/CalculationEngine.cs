using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Jace.Execution;
using Jace.Operations;
using Jace.Tokenizer;

namespace Jace
{
    public class CalculationEngine
    {
        private readonly IExecutor executor;
        private readonly CultureInfo cultureInfo;
        private readonly Dictionary<string, Func<Dictionary<string, double>, double>> executionFunctionCache;
        private readonly bool cacheEnabled;

        public CalculationEngine()
            : this(CultureInfo.CurrentCulture, ExecutionMode.Interpreted)
        {
        }

        public CalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode)
            : this(cultureInfo, executionMode, true) 
        {
        }

        public CalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode, bool cacheEnabled)
        {
            this.executionFunctionCache = new Dictionary<string, Func<Dictionary<string, double>, double>>();
            this.cultureInfo = cultureInfo;
            this.cacheEnabled = cacheEnabled;

            if (executionMode == ExecutionMode.Interpreted)
                executor = new Interpreter();
            else if (executionMode == ExecutionMode.Compiled)
                executor = new DynamicCompiler();
            else
                throw new ArgumentException(string.Format("Unsupported execution mode \"\".", executionMode), 
                    "executionMode");
        }

        public double Calculate(string functionText)
        {
            return Calculate(functionText, new Dictionary<string, double>());
        }

        public double Calculate(string functionText, Dictionary<string, double> variables)
        {
            if (IsInFunctionCache(functionText))
            {
                Func<Dictionary<string, double>, double> function = executionFunctionCache[functionText];
                return function(variables);
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(functionText);
                Func<Dictionary<string, double>, double> function = BuildFunction(functionText, operation);

                return function(variables);
            }
        }

        public FunctionBuilder Function(string functionText)
        {
            return new FunctionBuilder(functionText, this);
        }

        public Func<Dictionary<string, double>, double> Build(string functionText)
        {
            if (IsInFunctionCache(functionText))
            {
                return executionFunctionCache[functionText];
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(functionText);
                return BuildFunction(functionText, operation);
            }
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

        private Func<Dictionary<string, double>, double> BuildFunction(string functionText, Operation operation)
        {
            lock (this)
            {
                if (!IsInFunctionCache(functionText))
                {
                    Func<Dictionary<string, double>, double> function = executor.BuildFunction(operation);
                    executionFunctionCache.Add(functionText, function);
                }

                return executionFunctionCache[functionText];
            }
        }

        private bool IsInFunctionCache(string functionText)
        {
            return cacheEnabled && executionFunctionCache.ContainsKey(functionText);
        }
    }
}
