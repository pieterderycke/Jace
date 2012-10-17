using System;
using System.Collections.Concurrent;
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
        private readonly Optimizer optimizer;
        private readonly CultureInfo cultureInfo;
        private readonly ConcurrentDictionary<string, Func<Dictionary<string, double>, double>> executionFunctionCache;
        private readonly bool cacheEnabled;
        private readonly bool optimizerEnabled;

        public CalculationEngine()
            : this(CultureInfo.CurrentCulture, ExecutionMode.Compiled)
        {
        }

        public CalculationEngine(CultureInfo cultureInfo)
            : this(cultureInfo, ExecutionMode.Compiled)
        {
        }

        public CalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode)
            : this(cultureInfo, executionMode, true, true) 
        {
        }

        public CalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode, bool cacheEnabled, bool optimizerEnabled)
        {
            this.executionFunctionCache = new ConcurrentDictionary<string, Func<Dictionary<string, double>, double>>();
            this.cultureInfo = cultureInfo;
            this.cacheEnabled = cacheEnabled;
            this.optimizerEnabled = optimizerEnabled;

            if (executionMode == ExecutionMode.Interpreted)
                executor = new Interpreter();
            else if (executionMode == ExecutionMode.Compiled)
                executor = new DynamicCompiler();
            else
                throw new ArgumentException(string.Format("Unsupported execution mode \"\".", executionMode), 
                    "executionMode");

            optimizer = new Optimizer();
        }

        public double Calculate(string functionText)
        {
            return Calculate(functionText, new Dictionary<string, double>());
        }

        public double Calculate(string functionText, Dictionary<string, double> variables)
        {
            if (string.IsNullOrEmpty(functionText))
                throw new ArgumentNullException("functionText");

            if (variables == null)
                throw new ArgumentNullException("variables");

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
            if (string.IsNullOrEmpty(functionText))
                throw new ArgumentNullException("functionText");

            return new FunctionBuilder(functionText, this);
        }

        public Func<Dictionary<string, double>, double> Build(string functionText)
        {
            if (string.IsNullOrEmpty(functionText))
                throw new ArgumentNullException("functionText");

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
            Operation operation = astBuilder.Build(tokens);

            if (optimizerEnabled)
                return optimizer.Optimize(operation);
            else
                return operation;
        }

        private Func<Dictionary<string, double>, double> BuildFunction(string functionText, Operation operation)
        {
            return executionFunctionCache.GetOrAdd(functionText, v => executor.BuildFunction(operation));
        }

        private bool IsInFunctionCache(string functionText)
        {
            return cacheEnabled && executionFunctionCache.ContainsKey(functionText);
        }
    }
}
