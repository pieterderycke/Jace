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
    public abstract class GenericCalculationEngine<T> : ICalculationEngine<T>
    {
        private readonly IExecutor<T> executor;
        private readonly Optimizer<T> optimizer;
        private readonly CultureInfo cultureInfo;
        private readonly MemoryCache<string, Func<IDictionary<string, T>, T>> executionFormulaCache;
        private readonly bool cacheEnabled;
        private readonly bool optimizerEnabled;
        private readonly bool caseSensitive;
        protected readonly Random random;


        /// <summary>
        /// Creates a new instance of the <see cref="CalculationEngine"/> class.
        /// </summary>
        /// <param name="options">The <see cref="JaceOptions"/> to configure the behaviour of the engine.</param>
        public GenericCalculationEngine(JaceOptions options)
        {
            this.executionFormulaCache = new MemoryCache<string, Func<IDictionary<string, T>, T>>(options.CacheMaximumSize, options.CacheReductionSize);
            this.FunctionRegistry = new FunctionRegistry<T>(false);
            this.ConstantRegistry = new ConstantRegistry<T>(false);
            this.cultureInfo = options.CultureInfo;
            this.cacheEnabled = options.CacheEnabled;
            this.optimizerEnabled = options.OptimizerEnabled;
            this.caseSensitive = options.CaseSensitive;

            this.random = new Random();

            if (options.ExecutionMode == ExecutionMode.Interpreted)
                executor = CreateGenericInterpreter(caseSensitive);
            else if (options.ExecutionMode == ExecutionMode.Compiled)
                executor = new DynamicCompiler<T>(caseSensitive);
            else
                throw new ArgumentException(string.Format("Unsupported execution mode \"{0}\".", options.ExecutionMode),
                    "executionMode");

            optimizer = new Optimizer<T>(CreateGenericInterpreter(caseSensitive)); // We run the optimizer with the interpreter 

            // Register the default constants of Jace.NET into the constant registry
            if (options.DefaultConstants)
                RegisterDefaultConstants();

            // Register the default functions of Jace.NET into the function registry
            if (options.DefaultFunctions)
                RegisterDefaultFunctions();
        }

        private IExecutor<T> CreateGenericInterpreter(bool? caseSensitive = null)
        {
            IExecutor<T> genericExecutor = null;

            if (typeof(T) == typeof(double))
            {
                genericExecutor = (IExecutor<T>)new Interpreter<double>(DoubleNumericalOperations.Instance, caseSensitive ?? false);
            }
            else
            {
                genericExecutor = (IExecutor<T>)new Interpreter<decimal>(DecimalNumericalOperations.Instance, caseSensitive ?? false);
            }

            return genericExecutor;
        }

        protected abstract void RegisterDefaultConstants();

        protected abstract void RegisterDefaultFunctions();

        public IFunctionRegistry<T> FunctionRegistry { get; private set; }

        public IConstantRegistry<T> ConstantRegistry { get; private set; }

        public IEnumerable<FunctionInfo> Functions { get { return FunctionRegistry; } }

        public IEnumerable<ConstantInfo<T>> Constants { get { return ConstantRegistry; } }

        public T Calculate(string formulaText)
        {
            return Calculate(formulaText, new Dictionary<string, T>());
        }

        public T Calculate(string formulaText, IDictionary<string, T> variables)
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
            foreach (ConstantInfo<T> constant in ConstantRegistry)
                variables.Add(constant.ConstantName, constant.Value);

            if (IsInFormulaCache(formulaText, null, out var function))
            {
                return function(variables);
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(formulaText, new ConstantRegistry<T>(caseSensitive));
                function = BuildFormula(formulaText, null, operation);
                return function(variables);
            }
        }

        public FormulaBuilder<T> Formula(string formulaText)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            return new FormulaBuilder<T>(formulaText, caseSensitive, this);
        }

        /// <summary>
        /// Build a .NET func for the provided formula.
        /// </summary>
        /// <param name="formulaText">The formula that must be converted into a .NET func.</param>
        /// <returns>A .NET func for the provided formula.</returns>
        public Func<IDictionary<string, T>, T> Build(string formulaText)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");

            if (IsInFormulaCache(formulaText, null, out var result))
            {
                return result;
            }
            else
            {
                Operation operation = BuildAbstractSyntaxTree(formulaText, new ConstantRegistry<T>(caseSensitive));
                return BuildFormula(formulaText, null, operation);
            }
        }

        /// <summary>
        /// Build a .NET func for the provided formula.
        /// </summary>
        /// <param name="formulaText">The formula that must be converted into a .NET func.</param>
        /// <param name="constants">Constant values for variables defined into the formula. They variables will be replaced by the constant value at pre-compilation time.</param>
        /// <returns>A .NET func for the provided formula.</returns>
        public Func<IDictionary<string, T>, T> Build(string formulaText, IDictionary<string, T> constants)
        {
            if (string.IsNullOrEmpty(formulaText))
                throw new ArgumentNullException("formulaText");


            ConstantRegistry<T> compiledConstants = new ConstantRegistry<T>(caseSensitive);
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
        public void AddFunction(string functionName, Func<T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true); 
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        /// <summary>
        /// Add a function to the calculation engine.
        /// </summary>
        /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
        /// <param name="function">The implemenation of the function.</param>
        /// <param name="isIdempotent">Does the function provide the same result when it is executed multiple times.</param>
        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
        }

        public void AddFunction(string functionName, DynamicFunc<T, T> functionDelegate, bool isIdempotent = true)
        {
            FunctionRegistry.RegisterFunction(functionName, functionDelegate, isIdempotent, true);
        }

        /// <summary>
        /// Add a constant to the calculation engine.
        /// </summary>
        /// <param name="constantName">The name of the constant. This name can be used in mathematical formulas.</param>
        /// <param name="value">The value of the constant.</param>
        public void AddConstant(string constantName, T value)
        {
            ConstantRegistry.RegisterConstant(constantName, value);
        }



        /// <summary>
        /// Build the abstract syntax tree for a given formula. The formula string will
        /// be first tokenized.
        /// </summary>
        /// <param name="formulaText">A string containing the mathematical formula that must be converted 
        /// into an abstract syntax tree.</param>
        /// <returns>The abstract syntax tree of the formula.</returns>
        private Operation BuildAbstractSyntaxTree(string formulaText, ConstantRegistry<T> compiledConstants)
        {
            ITokenReader<T> tokenReader = null;
            if (typeof(T) == typeof(double))
            {
                tokenReader = (ITokenReader<T>)new TokenReader<double>(cultureInfo, DoubleNumericalOperations.Instance);
            }
            else
            {
                tokenReader = (ITokenReader<T>)new TokenReader<decimal>(cultureInfo, DecimalNumericalOperations.Instance);
            }

            List<Token> tokens = tokenReader.Read(formulaText);
            
            AstBuilder<T> astBuilder = new AstBuilder<T>(FunctionRegistry, caseSensitive, compiledConstants);
            Operation operation = astBuilder.Build(tokens);

            if (optimizerEnabled)
                return optimizer.Optimize(operation, this.FunctionRegistry, this.ConstantRegistry);
            else
                return operation;
        }

        private Func<IDictionary<string, T>, T> BuildFormula(string formulaText, ConstantRegistry<T> compiledConstants, Operation operation)
        {
            return executionFormulaCache.GetOrAdd(GenerateFormulaCacheKey(formulaText, compiledConstants), v => executor.BuildFormula(operation, this.FunctionRegistry, this.ConstantRegistry));
        }

        private bool IsInFormulaCache(string formulaText, ConstantRegistry<T> compiledConstants, out Func<IDictionary<string, T>, T> function)
        {
            function = null;
            return cacheEnabled && executionFormulaCache.TryGetValue(GenerateFormulaCacheKey(formulaText, compiledConstants), out function);
        }

        private string GenerateFormulaCacheKey(string formulaText, ConstantRegistry<T> compiledConstants)
        {
            return (compiledConstants != null && compiledConstants.Any()) ? $"{formulaText}@{String.Join(",", compiledConstants?.Select(x => $"{x.ConstantName}:{x.Value}"))}" : formulaText;
        }

        /// <summary>
        /// Verify a collection of variables to ensure that all the variable names are valid.
        /// Users are not allowed to overwrite reserved variables or use function names as variables.
        /// If an invalid variable is detected an exception is thrown.
        /// </summary>
        /// <param name="variables">The colletion of variables that must be verified.</param>
        public void VerifyVariableNames(IDictionary<string, T> variables)
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

    public class DoubleCalculationEngine : GenericCalculationEngine<double>
    {
        public DoubleCalculationEngine(JaceOptions options) : base(options)
        {
        }

        protected override void RegisterDefaultFunctions()
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
            FunctionRegistry.RegisterFunction("max", (DynamicFunc<double, double>)((a) => a.Max()), true, false);
            FunctionRegistry.RegisterFunction("min", (DynamicFunc<double, double>)((a) => a.Min()), true, false);
            FunctionRegistry.RegisterFunction("avg", (DynamicFunc<double, double>)((a) => a.Average()), true, false);
            FunctionRegistry.RegisterFunction("median", (DynamicFunc<double, double>)((a) => MathExtended.Median(a)), true, false);

            // Non Idempotent Functions
            FunctionRegistry.RegisterFunction("random", (Func<double>) random.NextDouble, false, false);
        }

        protected override void RegisterDefaultConstants()
        {
            ConstantRegistry.RegisterConstant("e", Math.E, false);
            ConstantRegistry.RegisterConstant("pi", Math.PI, false);
        }
    }

    public class DecimalCalculationEngine : GenericCalculationEngine<decimal>
    {
        public DecimalCalculationEngine(JaceOptions options) : base(options)
        {
        }

        protected override void RegisterDefaultFunctions()
        {
            FunctionRegistry.RegisterFunction("sin", (Func<decimal, decimal>)((a) => (decimal)Math.Sin((double)a)), true, false);
            FunctionRegistry.RegisterFunction("cos", (Func<decimal, decimal>)((a) => (decimal)Math.Cos((double)a)), true, false);
            FunctionRegistry.RegisterFunction("csc", (Func<decimal, decimal>)((a) => (decimal)MathUtil.Csc((double)a)), true, false);
            FunctionRegistry.RegisterFunction("sec", (Func<decimal, decimal>)((a) => (decimal)MathUtil.Sec((double)a)), true, false);
            FunctionRegistry.RegisterFunction("asin", (Func<decimal, decimal>)((a) => (decimal)Math.Asin((double)a)), true, false);
            FunctionRegistry.RegisterFunction("acos", (Func<decimal, decimal>)((a) => (decimal)Math.Acos((double)a)), true, false);
            FunctionRegistry.RegisterFunction("tan", (Func<decimal, decimal>)((a) => (decimal)Math.Tan((double)a)), true, false);
            FunctionRegistry.RegisterFunction("cot", (Func<decimal, decimal>)((a) => (decimal)MathUtil.Cot((double)a)), true, false);
            FunctionRegistry.RegisterFunction("atan", (Func<decimal, decimal>)((a) => (decimal)Math.Atan((double)a)), true, false);
            FunctionRegistry.RegisterFunction("acot", (Func<decimal, decimal>)((a) => (decimal)MathUtil.Acot((double)a)), true, false);
            FunctionRegistry.RegisterFunction("loge", (Func<decimal, decimal>)((a) => (decimal)Math.Log((double)a)), true, false);
            FunctionRegistry.RegisterFunction("log10", (Func<decimal, decimal>)((a) => (decimal)Math.Log10((double)a)), true, false);
            FunctionRegistry.RegisterFunction("logn", (Func<decimal, decimal, decimal>)((a, b) => (decimal)Math.Log((double)a, (double)b)), true, false);
            FunctionRegistry.RegisterFunction("sqrt", (Func<decimal, decimal>)((a) => (decimal)Math.Sqrt((double)a)), true, false);
            FunctionRegistry.RegisterFunction("abs", (Func<decimal, decimal>)((a) => Math.Abs(a)), true, false);            
            FunctionRegistry.RegisterFunction("if", (Func<decimal, decimal, decimal, decimal>)((a, b, c) => (a != 0.0m ? b : c)), true, false);
            FunctionRegistry.RegisterFunction("ifless", (Func<decimal, decimal, decimal, decimal, decimal>)((a, b, c, d) => (a < b ? c : d)), true, false);
            FunctionRegistry.RegisterFunction("ifmore", (Func<decimal, decimal, decimal, decimal, decimal>)((a, b, c, d) => (a > b ? c : d)), true, false);
            FunctionRegistry.RegisterFunction("ifequal", (Func<decimal, decimal, decimal, decimal, decimal>)((a, b, c, d) => (a == b ? c : d)), true, false);
            FunctionRegistry.RegisterFunction("ceiling", (Func<decimal, decimal>)((a) => Math.Ceiling(a)), true, false);
            FunctionRegistry.RegisterFunction("floor", (Func<decimal, decimal>)((a) => Math.Floor(a)), true, false);
            FunctionRegistry.RegisterFunction("truncate", (Func<decimal, decimal>)((a) => Math.Truncate(a)), true, false);
            FunctionRegistry.RegisterFunction("round", (Func<decimal, decimal>)Math.Round, true, false);

            // Dynamic based arguments Functions
            FunctionRegistry.RegisterFunction("max", (DynamicFunc<decimal, decimal>)((a) => a.Max()), true, false);
            FunctionRegistry.RegisterFunction("min", (DynamicFunc<decimal, decimal>)((a) => a.Min()), true, false);
            FunctionRegistry.RegisterFunction("avg", (DynamicFunc<decimal, decimal>)((a) => a.Average()), true, false);
            FunctionRegistry.RegisterFunction("median", (DynamicFunc<decimal, decimal>)((a) => MathExtended.Median(a)), true, false);

            // Non Idempotent Functions
            FunctionRegistry.RegisterFunction("random", (Func<decimal>)(() => (decimal) random.NextDouble() ) , false, false);

        }

        protected override void RegisterDefaultConstants()
        {
            ConstantRegistry.RegisterConstant("e", (decimal) Math.E, false);
            ConstantRegistry.RegisterConstant("pi", (decimal) Math.PI, false);
        }
    }



}
