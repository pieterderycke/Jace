using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;

namespace Jace.Execution
{
    public interface IExecutor<T>
    {
        T Execute(Operation operation, IFunctionRegistry<T> functionRegistry, IConstantRegistry<T> constantRegistry);
        T Execute(Operation operation, IFunctionRegistry<T> functionRegistry, IConstantRegistry<T> constantRegistry, IDictionary<string, T> variables);

        Func<IDictionary<string, T>, T> BuildFormula(Operation operation, IFunctionRegistry<T> functionRegistry, IConstantRegistry<T> constantRegistry);
    }
}
