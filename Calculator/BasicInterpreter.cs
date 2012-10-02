using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.Operations;

namespace Calculator
{
    public class BasicInterpreter : IInterpreter
    {
        public double Execute(Operation operation)
        {
            return Execute(operation, new Dictionary<string, int>());
        }

        public double Execute(Operation operation, Dictionary<string, int> variables)
        {
            if (operation == null)
                throw new ArgumentException("operation");

            if (operation.GetType() == typeof(IntegerConstant))
            {
                IntegerConstant constant = (IntegerConstant)operation;
                return constant.Value;
            }
            else if (operation.GetType() == typeof(Multiplication))
            {
                Multiplication multiplication = (Multiplication)operation;
                return Execute(multiplication.Argument1, variables) * Execute(multiplication.Argument2, variables);
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition)operation;
                return Execute(addition.Argument1, variables) + Execute(addition.Argument2, variables);
            }
            else if (operation.GetType() == typeof(Substraction))
            {
                Substraction addition = (Substraction)operation;
                return Execute(addition.Argument1, variables) - Execute(addition.Argument2, variables);
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division)operation;
                return Execute(division.Dividend, variables) / Execute(division.Divisor, variables);
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }
    }
}
