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
    public delegate TResult DynamicFunc<T, TResult>(params T[] values);

    /// <summary>
    /// The CalculationEngine class is the main class of Jace.NET to convert strings containing
    /// mathematical formulas into .NET Delegates and to calculate the result.
    /// It can be configured to run in a number of modes based on the constructor parameters choosen.
    /// </summary>
    public class CalculationEngine
    {
        private readonly IExecutor executor;
        private readonly Optimizer optimizer;
        private readonly CultureInfo cultureInfo;
        private readonly MemoryCache<string, Func<IDictionary<string, double>, double>> executionFormulaCache;
        private readonly bool cacheEnabled;
        private readonly bool optimizerEnabled;
        private readonly bool caseSensitive;

        private readonly Random random;

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class with
        /// default parameters.
        /// </summary>
        public CalculationEngine()
            : this(new JaceOptions())
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
            : this(new JaceOptions() { CultureInfo = cultureInfo })
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class. The optimizer and 
        /// cache are enabled.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating poin numbers.
        /// </param>
        /// <param name="executionMode">The execution mode that must be used for formula execution.</param>
        public CalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode)
            : this (new JaceOptions() { CultureInfo = cultureInfo, ExecutionMode = executionMode })
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating poin numbers.
        /// </param>
        /// <param name="executionMode">The execution mode that must be used for formula execution.</param>
        /// <param name="cacheEnabled">Enable or disable caching of mathematical formulas.</param>
        /// <param name="optimizerEnabled">Enable or disable optimizing of formulas.</param>
        /// <param name="adjustVariableCaseEnabled">Enable or disable auto lowercasing of variables.</param>
        [Obsolete]
        public CalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode, bool cacheEnabled, bool optimizerEnabled, bool adjustVariableCaseEnabled)
            : this(new JaceOptions() { CultureInfo = cultureInfo, ExecutionMode = executionMode, CacheEnabled = cacheEnabled, OptimizerEnabled = optimizerEnabled, CaseSensitive = !adjustVariableCaseEnabled })
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class.
        /// </summary>
        /// <param name="cultureInfo">
        /// The <see cref="CultureInfo"/> required for correctly reading floating poin numbers.
        /// </param>
        /// <param name="executionMode">The execution mode that must be used for formula execution.</param>
        /// <param name="cacheEnabled">Enable or disable caching of mathematical formulas.</param>
        /// <param name="optimizerEnabled">Enable or disable optimizing of formulas.</param>
        /// <param name="adjustVariableCaseEnabled">Enable or disable converting to lower case.</param>
        /// <param name="defaultFunctions">Enable or disable the default functions.</param>
        /// <param name="defaultConstants">Enable or disable the default constants.</param>
        /// <param name="cacheMaximumSize">Configure the maximum cache size for mathematical formulas.</param>
        /// <param name="cacheReductionSize">Configure the cache reduction size for mathematical formulas.</param>
        [Obsolete]
        public CalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode, bool cacheEnabled,
            bool optimizerEnabled, bool adjustVariableCaseEnabled, bool defaultFunctions, bool defaultConstants, int cacheMaximumSize, int cacheReductionSize)
            : this(new JaceOptions() { CultureInfo = cultureInfo, ExecutionMode = executionMode, CacheEnabled = cacheEnabled, OptimizerEnabled = optimizerEnabled,
                CaseSensitive = !adjustVariableCaseEnabled, DefaultFunctions = defaultFunctions, DefaultConstants = defaultConstants, 
                CacheMaximumSize = cacheMaximumSize, CacheReductionSize = cacheReductionSize })
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class.
        /// </summary>
        /// <param name="options">The <see cref="JaceOptions"/> to configure the behaviour of the engine.</param>
        public CalculationEngine(JaceOptions options)
        {
            this.executionFormulaCache = new MemoryCache<string, Func<IDictionary<string, double>, double>>(options.CacheMaximumSize, options.CacheReductionSize);
            this.FunctionRegistry = new FunctionRegistry(false);
            this.ConstantRegistry = new ConstantRegistry(false);
            this.cultureInfo = options.CultureInfo;
            this.cacheEnabled = options.CacheEnabled;
            this.optimizerEnabled = options.OptimizerEnabled;
            this.caseSensitive = options.CaseSensitive;

            this.random = new Random();

            if (options.ExecutionMode == ExecutionMode.Interpreted)
                executor = new Interpreter(caseSensitive);
            else if (options.ExecutionMode == ExecutionMode.Compiled)
                executor = new DynamicCompiler(caseSensitive);
            else
                throw new ArgumentException(string.Format("Unsupported execution mode \"{0}\".", options.ExecutionMode),
                    "executionMode");

            optimizer = new Optimizer(new Interpreter()); // We run the optimizer with the interpreter 

            // Register the default constants of Jace.NET into the constant registry
            if (options.DefaultConstants)
                RegisterDefaultConstants();

            // Register the default functions of Jace.NET into the function registry
            if (options.DefaultFunctions)
                RegisterDefaultFunctions();
        }

        internal IFunctionRegistry FunctionRegistry { get; private set; }

        internal IConstantRegistry ConstantRegistry { get; private set; }

        public IEnumerable<FunctionInfo> Functions { get { return FunctionRegistry; } }

        public IEnumerable<ConstantInfo> Constants { get { return ConstantRegistry; } }

        public double Calculate(string formulaText)
        {
            return Calculate(formulaText, new Dictionary<string, double>());
        }

        public double Calculate(string formulaText, IDictionary<string, double> variables)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            if (variables == null)
                throw new ArgumentNullException("variables");

            if (!caseSensitive)
            {
                variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
            }
            VerifyVariableNames(variables);

            // Add the reserved variables to the dictionary
            foreach (ConstantInfo constant in ConstantRegistry)
                variables.Add(constant.ConstantName, constant.Value);

            if (IsInFormulaCache(formulaText, null, out var function))
            {
                return function(variables);
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(formulaText, new ConstantRegistry(caseSensitive));
                function = BuildFormula(formulaText, null, operation);
                return function(variables);
            }
        }

        public FormulaBuilder Formula(string formulaText)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            return new FormulaBuilder(formulaText, caseSensitive, this);
        }

        /// <summary>
        /// Build a .NET func for the provided formula.
        /// </summary>
        /// <param name="formulaText">The formula that must be converted into a .NET func.</param>
        /// <returns>A .NET func for the provided formula.</returns>
        public Func<IDictionary<string, double>, double> Build(string formulaText)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            if (IsInFormulaCache(formulaText, null, out var result))
            {
                return result;
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(formulaText, new ConstantRegistry(caseSensitive));
                return BuildFormula(formulaText, null, operation);
            }
        }

        /// <summary>
        /// Build a .NET func for the provided formula.
        /// </summary>
        /// <param name="formulaText">The formula that must be converted into a .NET func.</param>
        /// <param name="constants">Constant values for variables defined into the formula. They variables will be replaced by the constant value at pre-compilation time.</param>
        /// <returns>A .NET func for the provided formula.</returns>
        public Func<IDictionary<string, double>, double> Build(string formulaText, IDictionary<string, double> constants)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");


            ConstantRegistry compiledConstants = new ConstantRegistry(caseSensitive);
            if (constants != null)
            {
                foreach (var constant in constants)
                {
                    compiledConstants.RegisterConstant(constant.Key, constant.Value);
                }
            }

            if (IsInFormulaCache(formulaText, compiledConstants, out var result))
            {
                return result;
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(formulaText, compiledConstants);
                return BuildFormula(formulaText, compiledConstants,  operation);
            }
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true); 
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, DynamicFunc<double, double> functionDelegate, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, functionDelegate, isIdempotent, true);
        }

        /// <summary>
        /// Add a constant to the calculation engine.
        /// </summary>
        /// <param name="constantName">The name of the constant. This name can be used in mathematical formulas.</param>
        /// <param name="value">The value of the constant.</param>
        public void AddConstant(string constantName, double value)
        {
            ConstantRegistry.RegisterConstant(constantName, value);
        }

        private void RegisterDefaultFunctions()
        {
            FunctionRegistry.RegisterFunction("sin", (Func<double, double>)Math.Sin, true, false);
            FunctionRegistry.RegisterFunction("cos", (Func<double, double>)Math.Cos, true, false);
            FunctionRegistry.RegisterFunction("csc", (Func<double, double>)MathUtil.Csc, true, false);
            FunctionRegistry.RegisterFunction("sec", (Func<double, double>)MathUtil.Sec, true, false);
            FunctionRegistry.RegisterFunction("asin", (Func<double, double>)Math.Asin, true, false);
            FunctionRegistry.RegisterFunction("acos", (Func<double, double>)Math.Acos, true, false);
            FunctionRegistry.RegisterFunction("tan", (Func<double, double>)Math.Tan, true, false);
            FunctionRegistry.RegisterFunction("cot", (Func<double, double>)MathUtil.Cot, true, false);
            FunctionRegistry.RegisterFunction("atan", (Func<double, double>)Math.Atan, true, false);
            FunctionRegistry.RegisterFunction("acot", (Func<double, double>)MathUtil.Acot, true, false);
            FunctionRegistry.RegisterFunction("loge", (Func<double, double>)Math.Log, true, false);
            FunctionRegistry.RegisterFunction("log10", (Func<double, double>)Math.Log10, true, false);
            FunctionRegistry.RegisterFunction("logn", (Func<double, double, double>)((a, b) => Math.Log(a, b)), true, false);
            FunctionRegistry.RegisterFunction("sqrt", (Func<double, double>)Math.Sqrt, true, false);
            FunctionRegistry.RegisterFunction("abs", (Func<double, double>)Math.Abs, true, false);
            FunctionRegistry.RegisterFunction("if", (Func<double, double, double, double>)((a, b, c) => (a != 0.0 ? b : c)), true, false);
            FunctionRegistry.RegisterFunction("ifless", (Func<double, double, double, double, double>)((a, b, c, d) => (a < b ? c : d)), true, false);
            FunctionRegistry.RegisterFunction("ifmore", (Func<double, double, double, double, double>)((a, b, c, d) => (a > b ? c : d)), true, false);
            FunctionRegistry.RegisterFunction("ifequal", (Func<double, double, double, double, double>)((a, b, c, d) => (a == b ? c : d)), true, false);
            FunctionRegistry.RegisterFunction("ceiling", (Func<double, double>)Math.Ceiling, true, false);
            FunctionRegistry.RegisterFunction("floor", (Func<double, double>)Math.Floor, true, false);
            FunctionRegistry.RegisterFunction("truncate", (Func<double, double>)Math.Truncate, true, false);
            FunctionRegistry.RegisterFunction("round", (Func<double, double>)Math.Round, true, false);

            // Dynamic based arguments Functions
            FunctionRegistry.RegisterFunction("max",  (DynamicFunc<double, double>)((a) => a.Max()), true, false);
            FunctionRegistry.RegisterFunction("min", (DynamicFunc<double, double>)((a) => a.Min()), true, false);
            FunctionRegistry.RegisterFunction("avg", (DynamicFunc<double, double>)((a) => a.Average()), true, false);
            FunctionRegistry.RegisterFunction("median", (DynamicFunc<double, double>)((a) => MathExtended.Median(a)), true, false);

            // Non Idempotent Functions
            FunctionRegistry.RegisterFunction("random", (Func<double>)random.NextDouble, false, false);
        }

        private void RegisterDefaultConstants()
        {
            ConstantRegistry.RegisterConstant("e", Math.E, false);
            ConstantRegistry.RegisterConstant("pi", Math.PI, false);
        }

        /// <summary>
        /// Build the abstract syntax tree for a given formula. The formula string will
        /// be first tokenized.
        /// </summary>
        /// <param name="formulaText">A string containing the mathematical formula that must be converted 
        /// into an abstract syntax tree.</param>
        /// <returns>The abstract syntax tree of the formula.</returns>
        private Operation BuildAbstractSyntaxTree(string formulaText, ConstantRegistry compiledConstants)
        {
            TokenReader tokenReader = new TokenReader(cultureInfo);
            List<Token> tokens = tokenReader.Read(formulaText);
            
            AstBuilder astBuilder = new AstBuilder(FunctionRegistry, caseSensitive, compiledConstants);
            Operation operation = astBuilder.Build(tokens);

            if (optimizerEnabled)
                return optimizer.Optimize(operation, this.FunctionRegistry, this.ConstantRegistry);
            else
                return operation;
        }

        private Func<IDictionary<string, double>, double> BuildFormula(string formulaText, ConstantRegistry compiledConstants, Operation operation)
        {
            return executionFormulaCache.GetOrAdd(GenerateFormulaCacheKey(formulaText, compiledConstants), v => executor.BuildFormula(operation, this.FunctionRegistry, this.ConstantRegistry));
        }

        private bool IsInFormulaCache(string formulaText, ConstantRegistry compiledConstants, out Func<IDictionary<string, double>, double> function)
        {
            function = null;
            return cacheEnabled && executionFormulaCache.TryGetValue(GenerateFormulaCacheKey(formulaText, compiledConstants), out function);
        }

        private string GenerateFormulaCacheKey(string formulaText, ConstantRegistry compiledConstants)
        {
            return (compiledConstants != null && compiledConstants.Any()) ? $"{formulaText}@{String.Join(",", compiledConstants?.Select(x => $"{x.ConstantName}:{x.Value}"))}" : formulaText;
        }

        /// <summary>
        /// Verify a collection of variables to ensure that all the variable names are valid.
        /// Users are not allowed to overwrite reserved variables or use function names as variables.
        /// If an invalid variable is detected an exception is thrown.
        /// </summary>
        /// <param name="variables">The colletion of variables that must be verified.</param>
        internal void VerifyVariableNames(IDictionary<string, double> variables)
        {
            foreach (string variableName in variables.Keys)
            {
                if(ConstantRegistry.IsConstantName(variableName) && !ConstantRegistry.GetConstantInfo(variableName).IsOverWritable)
                    throw new ArgumentException(string.Format("The name \"{0}\" is a reservered variable name that cannot be overwritten.", variableName), "variables");

                if (FunctionRegistry.IsFunctionName(variableName))
                    throw new ArgumentException(string.Format("The name \"{0}\" is a function name. Parameters cannot have this name.", variableName), "variables");
            }
        }
    }
}
