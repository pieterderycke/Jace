using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Execution;

namespace Jace
{
    public class FormulaContext
    {
        public FormulaContext(IDictionary<string, double> variables,
            IFunctionRegistry functionRegistry,
            IConstantRegistry constantRegistry)
        {
            this.Variables = variables;
            this.FunctionRegistry = functionRegistry;
            this.ConstantRegistry = constantRegistry;
        }

        public IDictionary<string, double> Variables { get; private set; }

        public IFunctionRegistry FunctionRegistry { get; private set; }
        public IConstantRegistry ConstantRegistry { get; private set; }
    }
}
