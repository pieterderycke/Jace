using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Execution;

namespace Jace
{
    public class FormulaContext<T>
    {
        public FormulaContext(IDictionary<string, T> variables,
            IFunctionRegistry<T> functionRegistry,
            IConstantRegistry<T> constantRegistry)
        {
            this.Variables = variables;
            this.FunctionRegistry = functionRegistry;
            this.ConstantRegistry = constantRegistry;
        }

        public IDictionary<string, T> Variables { get; private set; }

        public IFunctionRegistry<T> FunctionRegistry { get; private set; }
        public IConstantRegistry<T> ConstantRegistry { get; private set; }
    }
}
