using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Execution;

namespace Jace
{
    public class FormulaContext
    {
        public FormulaContext(Dictionary<string, double> variables,
            IFunctionRegistry functionRegistry)
        {
            this.Variables = variables;
            this.FunctionRegistry = functionRegistry;
        }

        public Dictionary<string, double> Variables { get; private set; }

        public IFunctionRegistry FunctionRegistry { get; private set; }
    }
}
