using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.Operations;

namespace Calculator
{
    public interface IInterpreter
    {
        int Execute(Operation<int> operation, Dictionary<string, int> variables);
        int Execute(Operation<int> operation);
    }
}
