using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;

namespace Jace
{
    public interface IExecutor
    {
        double Execute(Operation operation);
        double Execute(Operation operation, Dictionary<string, int> variables);
        double Execute(Operation operation, Dictionary<string, double> variables);

        Func<Dictionary<string, double>, double> BuildFunction(Operation operation);
    }
}
