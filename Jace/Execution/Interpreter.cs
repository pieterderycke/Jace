using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Jace.Operations;
using Jace.Util;

namespace Jace.Execution
{
    public class Interpreter : IExecutor
    {
        private readonly bool caseSensitive;

        public Interpreter(): this(false) { }

        public Interpreter(bool caseSensitive)
        {
            this.caseSensitive = caseSensitive;
        }
        public Func<IDictionary<string, double>, double> BuildFormula(Operation operation, 
            IFunctionRegistry functionRegistry,
            IConstantRegistry constantRegistry)
        {
            return caseSensitive
              ? (Func<IDictionary<string, double>, double>)(variables =>
              {
                  return Execute(operation, functionRegistry, constantRegistry, variables);
              })
              : (Func<IDictionary<string, double>, double>)(variables =>
              {
                  variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
                  return Execute(operation, functionRegistry, constantRegistry, variables);
              });
        }

        public double Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry)
        {
            return Execute(operation, functionRegistry, constantRegistry, new Dictionary<string, double>());
        }

        public double Execute(Operation operation,
            IFunctionRegistry functionRegistry,
            IConstantRegistry constantRegistry, 
            IDictionary<string, double> variables)
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

                double value;
                bool variableFound = variables.TryGetValue(variable.Name, out value);

                if (variableFound)
                    return value;
                else
                    throw new VariableNotDefinedException(string.Format("The variable \"{0}\" used is not defined.", variable.Name));
            }
            else if (operation.GetType() == typeof(Multiplication))
            {
                Multiplication multiplication = (Multiplication)operation;
                return Execute(multiplication.Argument1, functionRegistry, constantRegistry,  variables) * Execute(multiplication.Argument2, functionRegistry, constantRegistry,  variables);
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition)operation;
                return Execute(addition.Argument1, functionRegistry, constantRegistry,  variables) + Execute(addition.Argument2, functionRegistry, constantRegistry,  variables);
            }
            else if (operation.GetType() == typeof(Subtraction))
            {
                Subtraction addition = (Subtraction)operation;
                return Execute(addition.Argument1, functionRegistry, constantRegistry,  variables) - Execute(addition.Argument2, functionRegistry, constantRegistry,  variables);
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division)operation;
                return Execute(division.Dividend, functionRegistry, constantRegistry,  variables) / Execute(division.Divisor, functionRegistry, constantRegistry,  variables);
            }
            else if (operation.GetType() == typeof(Modulo))
            {
                Modulo division = (Modulo)operation;
                return Execute(division.Dividend, functionRegistry, constantRegistry,  variables) % Execute(division.Divisor, functionRegistry, constantRegistry,  variables);
            }
            else if (operation.GetType() == typeof(Exponentiation))
            {
                Exponentiation exponentiation = (Exponentiation)operation;
                return Math.Pow(Execute(exponentiation.Base, functionRegistry, constantRegistry,  variables), Execute(exponentiation.Exponent, functionRegistry, constantRegistry,  variables));
            }
            else if (operation.GetType() == typeof(UnaryMinus))
            {
                UnaryMinus unaryMinus = (UnaryMinus)operation;
                return -Execute(unaryMinus.Argument, functionRegistry, constantRegistry,  variables);
            }
            else if (operation.GetType() == typeof(And))
            {
                And and = (And)operation;
                var operation1 = Execute(and.Argument1, functionRegistry, constantRegistry,  variables) != 0;
                var operation2 = Execute(and.Argument2, functionRegistry, constantRegistry,  variables) != 0;

                return (operation1 && operation2) ? 1.0 : 0.0;
            }
            else if (operation.GetType() == typeof(Or))
            {
                Or or = (Or)operation;
                var operation1 = Execute(or.Argument1, functionRegistry, constantRegistry,  variables) != 0;
                var operation2 = Execute(or.Argument2, functionRegistry, constantRegistry,  variables) != 0;

                return (operation1 || operation2) ? 1.0 : 0.0;
            }
            else if(operation.GetType() == typeof(LessThan))
            {
                LessThan lessThan = (LessThan)operation;
                return (Execute(lessThan.Argument1, functionRegistry, constantRegistry,  variables) < Execute(lessThan.Argument2, functionRegistry, constantRegistry,  variables)) ? 1.0 : 0.0;
            }
            else if (operation.GetType() == typeof(LessOrEqualThan))
            {
                LessOrEqualThan lessOrEqualThan = (LessOrEqualThan)operation;
                return (Execute(lessOrEqualThan.Argument1, functionRegistry, constantRegistry,  variables) <= Execute(lessOrEqualThan.Argument2, functionRegistry, constantRegistry,  variables)) ? 1.0 : 0.0;
            }
            else if (operation.GetType() == typeof(GreaterThan))
            {
                GreaterThan greaterThan = (GreaterThan)operation;
                return (Execute(greaterThan.Argument1, functionRegistry, constantRegistry,  variables) > Execute(greaterThan.Argument2, functionRegistry, constantRegistry,  variables)) ? 1.0 : 0.0;
            }
            else if (operation.GetType() == typeof(GreaterOrEqualThan))
            {
                GreaterOrEqualThan greaterOrEqualThan = (GreaterOrEqualThan)operation;
                return (Execute(greaterOrEqualThan.Argument1, functionRegistry, constantRegistry,  variables) >= Execute(greaterOrEqualThan.Argument2, functionRegistry, constantRegistry,  variables)) ? 1.0 : 0.0;
            }
            else if (operation.GetType() == typeof(Equal))
            {
                Equal equal = (Equal)operation;
                return (Execute(equal.Argument1, functionRegistry, constantRegistry,  variables) == Execute(equal.Argument2, functionRegistry, constantRegistry,  variables)) ? 1.0 : 0.0;
            }
            else if (operation.GetType() == typeof(NotEqual))
            {
                NotEqual notEqual = (NotEqual)operation;
                return (Execute(notEqual.Argument1, functionRegistry, constantRegistry,  variables) != Execute(notEqual.Argument2, functionRegistry, constantRegistry,  variables)) ? 1.0 : 0.0;
            }
            else if (operation.GetType() == typeof(Function))
            {
                Function function = (Function)operation;

                FunctionInfo functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);

                double[] arguments = new double[functionInfo.IsDynamicFunc ? function.Arguments.Count : functionInfo.NumberOfParameters];
                for (int i = 0; i < arguments.Length; i++)
                    arguments[i] = Execute(function.Arguments[i], functionRegistry, constantRegistry,  variables);

                return Invoke(functionInfo.Function, arguments);
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }

        private double Invoke(Delegate function, double[] arguments)
        {
            // DynamicInvoke is slow, so we first try to convert it to a Func
            if (function is Func<double>)
            {
                return ((Func<double>)function).Invoke();
            }
            else if (function is Func<double, double>)
            {
                return ((Func<double, double>)function).Invoke(arguments[0]);
            }
            else if (function is Func<double, double, double>)
            {
                return ((Func<double, double, double>)function).Invoke(arguments[0], arguments[1]);
            }
            else if (function is Func<double, double, double, double>)
            {
                return ((Func<double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2]);
            }
            else if (function is Func<double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3]);
            }
            else if (function is Func<double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
            }
            else if (function is Func<double, double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
            }
            else if (function is Func<double, double, double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6]);
            }
            else if (function is Func<double, double, double, double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7]);
            }
            else if (function is Func<double, double, double, double, double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8]);
            }
            else if (function is Func<double, double, double, double, double, double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9]);
            }
            else if (function is Func<double, double, double, double, double, double, double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double, double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10]);
            }
            else if (function is Func<double, double, double, double, double, double, double, double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double, double, double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11]);
            }
            else if (function is Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12]);
            }
            else if (function is Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13]);
            }
            else if (function is Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13], arguments[14]);
            }
            else if (function is Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double>)
            {
                return ((Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13], arguments[14], arguments[15]);
            }
            else if (function is DynamicFunc<double, double>)
            {
                return ((DynamicFunc<double, double>)function).Invoke(arguments);
            }
            else
            {
                return (double)function.DynamicInvoke((from s in arguments select (object)s).ToArray());
            }
        }
    }
}
