using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Jace.Operations;
using Jace.Util;

namespace Jace.Execution
{
    public class FormulaBuilder
    {
        private readonly CalculationEngine engine;

        private string formulaText;
        private DataType? resultDataType;
        private List<ParameterInfo> parameters;

        /// <summary>
        /// Creates a new instance of the FormulaBuilder class.
        /// </summary>
        /// <param name="formulaText">
        /// A calculation engine instance that can be used for interpreting and executing 
        /// the formula.
        /// </param>
        internal FormulaBuilder(string formulaText, CalculationEngine engine)
        {
            this.parameters = new List<ParameterInfo>();
            this.formulaText = formulaText;
            this.engine = engine;
        }

        /// <summary>
        /// Add a new parameter to the formula being constructed. Parameters are
        /// added in the order of which they are defined.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dataType">The date type of the parameter.</param>
        /// <returns>The <see cref="FormulaBuilder"/> instance.</returns>
        public FormulaBuilder Parameter(string name, DataType dataType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (engine.FunctionRegistry.IsFunctionName(name))
                throw new ArgumentException(string.Format("The name \"{0}\" is a function name. Parameters cannot have this name.", name), "name");

            if (parameters.Any(p => p.Name == name))
                throw new ArgumentException(string.Format("A parameter with the name \"{0}\" was already defined.", name), "name");

            parameters.Add(new ParameterInfo() {Name = name, DataType = dataType});
            return this;
        }

        /// <summary>
        /// Define the result data type for the formula.
        /// </summary>
        /// <param name="dataType">The result data type for the formula.</param>
        /// <returns>The <see cref="FormulaBuilder"/> instance.</returns>
        public FormulaBuilder Result(DataType dataType)
        {
            if (resultDataType.HasValue)
                throw new InvalidOperationException("The result can only be defined once for a given formula.");

            resultDataType = dataType;
            return this;
        }

        /// <summary>
        /// Build the formula defined. This will create a func delegate matching with the parameters
        /// and the return type specified.
        /// </summary>
        /// <returns>The func delegate for the defined formula.</returns>
        public Delegate Build()
        {
            if (!resultDataType.HasValue)
                throw new Exception("Please define a result data type for the formula.");

            Func<Dictionary<string, double>, double> formula = engine.Build(formulaText);

            FuncAdapter adapter = new FuncAdapter();
            return adapter.Wrap(parameters, variables => formula(variables));
        }
    }
}
