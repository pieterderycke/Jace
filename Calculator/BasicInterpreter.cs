using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.Operations;

namespace Calculator
{
    public class BasicInterpreter : IInterpreter
    {
        public int Execute(Operation<int> operation)
        {
            return Execute(operation, new Dictionary<string, int>());
        }

        public int Execute(Operation<int> operation, Dictionary<string, int> variables)
        {
            if (operation == null)
                throw new ArgumentException("operation");

            if (operation.GetType() == typeof(Constant<int>))
            {
                Constant<int> constant = (Constant<int>)operation;
                return constant.Value;
            }
            else if (operation.GetType() == typeof(Multiplication<int>))
            {
                Multiplication<int> multiplication = (Multiplication<int>)operation;
                return Execute(multiplication.Argument1, variables) * Execute(multiplication.Argument2, variables);
            }
            else if (operation.GetType() == typeof(Addition<int>))
            {
                Addition<int> addition = (Addition<int>)operation;
                return Execute(addition.Argument1, variables) + Execute(addition.Argument2, variables);
            }
            else if (operation.GetType() == typeof(Substraction<int>))
            {
                Substraction<int> addition = (Substraction<int>)operation;
                return Execute(addition.Argument1, variables) - Execute(addition.Argument2, variables);
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }
    }
}
