using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Jace.Execution;
using Jace.Operations;
using Jace.Tokenizer;
using Jace.Util;

namespace Jace
{
    public class CalculationEngine
    {
        private readonly IExecutor executor;
        private readonly Optimizer optimizer;
        private readonly CultureInfo cultureInfo;
        private readonly MemoryCache<string, Func<Dictionary<string, double>, double>> executionFormulaCache;
        private readonly bool cacheEnabled;
        private readonly bool optimizerEnabled;

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class with
        /// default parameters.
        /// </summary>
        public CalculationEngine()
            : this(CultureInfo.CurrentCulture, ExecutionMode.Compiled)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class. The dynamic compiler
        /// is used for formula execution and the optimizer and cache are enabled.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating poin numbers.
        /// </param>
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
            this.executionFormulaCache = new MemoryCache<string, Func<Dictionary<string, double>, double>>();
            this.cultureInfo = cultureInfo;
            this.cacheEnabled = cacheEnabled;
            this.optimizerEnabled = optimizerEnabled;

            if (executionMode == ExecutionMode.Interpreted)
                executor = new Interpreter();
            else if (executionMode == ExecutionMode.Compiled)
                executor = new DynamicCompiler();
            else
                throw new ArgumentException(string.Format("Unsupported execution mode \"{0}\".", executionMode), 
                    "executionMode");

            optimizer = new Optimizer(new Interpreter()); // We run the optimizer with the interpreter 
        }

        public double Calculate(string formulaText)
        {
            return Calculate(formulaText, new Dictionary<string, double>());
        }

        public double Calculate(string formulaText, Dictionary<string, double> variables)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            if (variables == null)
                throw new ArgumentNullException("variables");

            if (IsInFormulaCache(formulaText))
            {
                Func<Dictionary<string, double>, double> formula = executionFormulaCache[formulaText];
                return formula(variables);
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(formulaText);
                Func<Dictionary<string, double>, double> function = BuildFormula(formulaText, operation);

                return function(variables);
            }
        }

        public FormulaBuilder Formula(string formulaText)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            return new FormulaBuilder(formulaText, this);
        }

        public Func<Dictionary<string, double>, double> Build(string formulaText)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            if (IsInFormulaCache(formulaText))
            {
                return executionFormulaCache[formulaText];
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(formulaText);
                return BuildFormula(formulaText, operation);
            }
        }

        /// <summary>
        /// Build the abstract syntax tree for a given formula. The formula string will
        /// be first tokenized.
        /// </summary>
        /// <param name="formulaText">A string containing the mathematical formula that must be converted 
        /// into an abstract syntax tree.</param>
        /// <returns>The abstract syntax tree of the formula.</returns>
        private Operation BuildAbstractSyntaxTree(string formulaText)
        {
            TokenReader tokenReader = new TokenReader(cultureInfo);
            List<Token> tokens = tokenReader.Read(formulaText);

            AstBuilder astBuilder = new AstBuilder();
            Operation operation = astBuilder.Build(tokens);

            if (optimizerEnabled)
                return optimizer.Optimize(operation);
            else
                return operation;
        }

        private Func<Dictionary<string, double>, double> BuildFormula(string formulaText, Operation operation)
        {
            return executionFormulaCache.GetOrAdd(formulaText, v => executor.BuildFunction(operation));
        }

        private bool IsInFormulaCache(string formulaText)
        {
            return cacheEnabled && executionFormulaCache.ContainsKey(formulaText);
        }
    }
}
