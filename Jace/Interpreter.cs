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
        public Func<Dictionary<string, double>, double> BuildFunction(Operation operation, 
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

                switch (function.FunctionType)
                { 
                    case FunctionType.Sine:
                        return Math.Sin(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Cosine:
                        return Math.Cos(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Secant:
                        return MathUtil.Sec(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Cosecant:
                        return MathUtil.Csc(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Arcsine:
                        return Math.Asin(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Arccosine:
                        return Math.Acos(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Tangent:
                        return Math.Tan(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Cotangent:
                        return MathUtil.Cot(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Arctangent:
                        return Math.Atan(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Arccotangent:
                        return MathUtil.Acot(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Loge:
                        return Math.Log(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Log10:
                        return Math.Log10(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.Logn:
                        return Math.Log(Execute(function.Arguments[0], functionRegistry, variables), Execute(function.Arguments[1], functionRegistry, variables));
                    case FunctionType.SquareRoot:
                        return Math.Sqrt(Execute(function.Arguments[0], functionRegistry, variables));
                    case FunctionType.AbsoluteValue:
                        return Math.Abs(Execute(function.Arguments[0], functionRegistry, variables));
                    default:
                        throw new ArgumentException(string.Format("Unsupported function \"{0}\".", function.FunctionType), "operation");
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }
    }
}
