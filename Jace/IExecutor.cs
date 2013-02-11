using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Execution;
using Jace.Operations;

namespace Jace
{
    public interface IExecutor
    {
        double Execute(Operation operation, IFunctionRegistry functionRegistry);
        double Execute(Operation operation, IFunctionRegistry functionRegistry, Dictionary<string, double> variables);

        Func<Dictionary<string, double>, double> BuildFormula(Operation operation, IFunctionRegistry functionRegistry);
    }
}
