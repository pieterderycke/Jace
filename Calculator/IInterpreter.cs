using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.Operations;

namespace Calculator
{
    public interface IInterpreter
    {
        double Execute(Operation operation, Dictionary<string, int> variables);
        double Execute(Operation operation);
    }
}
