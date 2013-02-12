using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;
using Jace.Util;
using Jace.Execution;

namespace Jace
{
    public class Interpreter : IExecutor
    {
        public Func<Dictionary<string, double>, double> BuildFormula(Operation operation, 
            IFunctionRegistry functionRegistry)
        { 
            return variables => Execute(operation, functionRegistry, variables);
        }

        public double Execute(Operation operation, IFunctionRegistry functionRegistry)
        {
            return Execute(operation, functionRegistry, new Dictionary<string, double>());
        }

        public double Execute(Operation operation, IFunctionRegistry functionRegistry, 
            Dictionary<string, double> variables)
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
                return Execute(multiplication.Argument1, functionRegistry, variables) * Execute(multiplication.Argument2, functionRegistry, variables);
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition)operation;
                return Execute(addition.Argument1, functionRegistry, variables) + Execute(addition.Argument2, functionRegistry, variables);
            }
            else if (operation.GetType() == typeof(Subtraction))
            {
                Subtraction addition = (Subtraction)operation;
                return Execute(addition.Argument1, functionRegistry, variables) - Execute(addition.Argument2, functionRegistry, variables);
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division)operation;
                return Execute(division.Dividend, functionRegistry, variables) / Execute(division.Divisor, functionRegistry, variables);
            }
            else if (operation.GetType() == typeof(Modulo))
            {
                Modulo division = (Modulo)operation;
                return Execute(division.Dividend, functionRegistry, variables) % Execute(division.Divisor, functionRegistry, variables);
            }
            else if (operation.GetType() == typeof(Exponentiation))
            {
                Exponentiation exponentiation = (Exponentiation)operation;
                return Math.Pow(Execute(exponentiation.Base, functionRegistry, variables), Execute(exponentiation.Exponent, functionRegistry, variables));
            }
            else if (operation.GetType() == typeof(Function))
            {
                Function function = (Function)operation;

                FunctionInfo functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);

                object[] arguments = new object[functionInfo.NumberOfParameters];
                for (int i = 0; i < arguments.Length; i++)
                    arguments[i] = Execute(function.Arguments[i], functionRegistry, variables);

                return (double)functionInfo.Function.DynamicInvoke(arguments);
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }
    }
}
