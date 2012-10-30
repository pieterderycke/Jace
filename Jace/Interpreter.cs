using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;

namespace Jace
{
    public class Interpreter : IExecutor
    {
        public Func<Dictionary<string, double>, double> BuildFunction(Operation operation)
        { 
            return variables => Execute(operation, variables);
        }

        public double Execute(Operation operation)
        {
            return Execute(operation, new Dictionary<string, double>());
        }

        public double Execute(Operation operation, Dictionary<string, int> variables)
        {
            Dictionary<string, double> doubleVariables = new Dictionary<string, double>();
            foreach (string key in variables.Keys)
                doubleVariables.Add(key, variables[key]);

            return Execute(operation, doubleVariables);
        }

        public double Execute(Operation operation, Dictionary<string, double> variables)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            if (operation.GetType() == typeof(IntegerConstant))
            {
                IntegerConstant constant = (IntegerConstant)operation;
                return constant.Value;
            }
            else if (operation.GetType() == typeof(FloatingPointConstant))
            {
                FloatingPointConstant constant = (FloatingPointConstant)operation;
                return constant.Value;
            }
            else if (operation.GetType() == typeof(Variable))
            {
                Variable variable = (Variable)operation;
                if (variables.ContainsKey(variable.Name))
                    return variables[variable.Name];
                else
                    throw new VariableNotDefinedException(string.Format("The variable \"{0}\" used is not defined.", variable.Name));
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
            else if (operation.GetType() == typeof(Exponentiation))
            {
                Exponentiation exponentiation = (Exponentiation)operation;
                return Math.Pow(Execute(exponentiation.Base, variables), Execute(exponentiation.Exponent, variables));
            }
            else if (operation.GetType() == typeof(Function))
            {
                Function function = (Function)operation;

                switch (function.FunctionType)
                { 
                    case FunctionType.Sine:
                        return Math.Sin(Execute(function.Arguments[0]));
                    case FunctionType.Cosine:
                        return Math.Cos(Execute(function.Arguments[0]));
                    default:
                        throw new Exception(string.Format("Unsupported function \"{0}\".", function.FunctionType));
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }
    }
}
