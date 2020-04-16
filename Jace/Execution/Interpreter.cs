using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Jace.Operations;
using Jace.Util;

namespace Jace.Execution
{
    public class Interpreter<T> : IExecutor<T>
    {
        private readonly bool caseSensitive;

        private readonly INumericalOperations<T> numericalOperations;

        public Interpreter(INumericalOperations<T> numericalOperations) : this(numericalOperations, false) { }

        public Interpreter(INumericalOperations<T> numericalOperations, bool caseSensitive)
        {
            this.numericalOperations = numericalOperations;
            this.caseSensitive = caseSensitive;
        }
        public Func<IDictionary<string, T>, T> BuildFormula(Operation operation, 
            IFunctionRegistry<T> functionRegistry,
            IConstantRegistry<T> constantRegistry)
        {
            return caseSensitive
              ? (Func<IDictionary<string, T>, T>)(variables =>
              {
                  return Execute(operation, functionRegistry, constantRegistry, variables);
              })
              : (Func<IDictionary<string, T>, T>)(variables =>
              {
                  variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
                  return Execute(operation, functionRegistry, constantRegistry, variables);
              });
        }

        public T Execute(Operation operation, IFunctionRegistry<T> functionRegistry, IConstantRegistry<T> constantRegistry)
        {
            return Execute(operation, functionRegistry, constantRegistry, new Dictionary<string, T>());
        }

        public T Execute(Operation operation,
            IFunctionRegistry<T> functionRegistry,
            IConstantRegistry<T> constantRegistry, 
            IDictionary<string, T> variables)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            if (operation.GetType() == typeof(IntegerConstant))
            {
                IntegerConstant constant = (IntegerConstant)operation;
                return numericalOperations.ConvertFromInt32(constant.Value);
            }
            else if (operation.GetType() == typeof(FloatingPointConstant<T>))
            {
                FloatingPointConstant<T> constant = (FloatingPointConstant<T>)operation;
                return constant.Value;
            }
            else if (operation.GetType() == typeof(Variable))
            {
                Variable variable = (Variable)operation;

                T value;
                bool variableFound = variables.TryGetValue(variable.Name, out value);

                if (variableFound)
                    return value;
                else
                    throw new VariableNotDefinedException(string.Format("The variable \"{0}\" used is not defined.", variable.Name));
            }
            else if (operation.GetType() == typeof(Multiplication))
            {
                Multiplication multiplication = (Multiplication)operation;
                return numericalOperations.Multiply(Execute(multiplication.Argument1, functionRegistry, constantRegistry,  variables), Execute(multiplication.Argument2, functionRegistry, constantRegistry,  variables));
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition)operation;
                return numericalOperations.Add(Execute(addition.Argument1, functionRegistry, constantRegistry,  variables), Execute(addition.Argument2, functionRegistry, constantRegistry,  variables));
            }
            else if (operation.GetType() == typeof(Subtraction))
            {
                Subtraction addition = (Subtraction)operation;
                return numericalOperations.Subtract(Execute(addition.Argument1, functionRegistry, constantRegistry,  variables), Execute(addition.Argument2, functionRegistry, constantRegistry,  variables));
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division)operation;
                return numericalOperations.Divide(Execute(division.Dividend, functionRegistry, constantRegistry,  variables), Execute(division.Divisor, functionRegistry, constantRegistry,  variables));
            }
            else if (operation.GetType() == typeof(Modulo))
            {
                Modulo division = (Modulo)operation;
                return numericalOperations.Modulo(Execute(division.Dividend, functionRegistry, constantRegistry,  variables) , Execute(division.Divisor, functionRegistry, constantRegistry,  variables));
            }
            else if (operation.GetType() == typeof(Exponentiation))
            {
                Exponentiation exponentiation = (Exponentiation)operation;
                return  numericalOperations.Pow(Execute(exponentiation.Base, functionRegistry, constantRegistry,  variables), Execute(exponentiation.Exponent, functionRegistry, constantRegistry,  variables));
            }
            else if (operation.GetType() == typeof(UnaryMinus))
            {
                UnaryMinus unaryMinus = (UnaryMinus)operation;
                return numericalOperations.Negate(Execute(unaryMinus.Argument, functionRegistry, constantRegistry,  variables));
            }
            else if (operation.GetType() == typeof(And))
            {
                And and = (And)operation;                
                return numericalOperations.And(Execute(and.Argument1, functionRegistry, constantRegistry, variables), Execute(and.Argument2, functionRegistry, constantRegistry, variables));
            }
            else if (operation.GetType() == typeof(Or))
            {
                Or or = (Or)operation;
                return numericalOperations.Or(Execute(or.Argument1, functionRegistry, constantRegistry, variables), Execute(or.Argument2, functionRegistry, constantRegistry, variables));
            }
            else if(operation.GetType() == typeof(LessThan))
            {
                LessThan lessThan = (LessThan)operation;
                return numericalOperations.LessThan(Execute(lessThan.Argument1, functionRegistry, constantRegistry, variables), Execute(lessThan.Argument2, functionRegistry, constantRegistry, variables));                
            }
            else if (operation.GetType() == typeof(LessOrEqualThan))
            {
                LessOrEqualThan lessOrEqualThan = (LessOrEqualThan)operation;
                return numericalOperations.LessOrEqualThan(Execute(lessOrEqualThan.Argument1, functionRegistry, constantRegistry, variables), Execute(lessOrEqualThan.Argument2, functionRegistry, constantRegistry, variables));                
            }
            else if (operation.GetType() == typeof(GreaterThan))
            {
                GreaterThan greaterThan = (GreaterThan)operation;
                return numericalOperations.GreaterThan(Execute(greaterThan.Argument1, functionRegistry, constantRegistry, variables), Execute(greaterThan.Argument2, functionRegistry, constantRegistry, variables));                
            }
            else if (operation.GetType() == typeof(GreaterOrEqualThan))
            {
                GreaterOrEqualThan greaterOrEqualThan = (GreaterOrEqualThan)operation;
                return numericalOperations.GreaterOrEqualThan(Execute(greaterOrEqualThan.Argument1, functionRegistry, constantRegistry,  variables) , Execute(greaterOrEqualThan.Argument2, functionRegistry, constantRegistry,  variables));
            }
            else if (operation.GetType() == typeof(Equal))
            {
                Equal equal = (Equal)operation;
                return numericalOperations.Equal(Execute(equal.Argument1, functionRegistry, constantRegistry,  variables), Execute(equal.Argument2, functionRegistry, constantRegistry,  variables));
            }
            else if (operation.GetType() == typeof(NotEqual))
            {
                NotEqual notEqual = (NotEqual)operation;
                return  numericalOperations.NotEqual(Execute(notEqual.Argument1, functionRegistry, constantRegistry,  variables), Execute(notEqual.Argument2, functionRegistry, constantRegistry,  variables));
            }
            else if (operation.GetType() == typeof(Function))
            {
                Function function = (Function)operation;

                FunctionInfo functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);

                T[] arguments = new T[functionInfo.IsDynamicFunc ? function.Arguments.Count : functionInfo.NumberOfParameters];
                for (int i = 0; i < arguments.Length; i++)
                    arguments[i] = Execute(function.Arguments[i], functionRegistry, constantRegistry,  variables);

                return Invoke(functionInfo.Function, arguments);
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }

        private T Invoke(Delegate function, T[] arguments)
        {
            // DynamicInvoke is slow, so we first try to convert it to a Func
            if (function is Func<T>)
            {
                return ((Func<T>)function).Invoke();
            }
            else if (function is Func<T, T>)
            {
                return ((Func<T, T>)function).Invoke(arguments[0]);
            }
            else if (function is Func<T, T, T>)
            {
                return ((Func<T, T, T>)function).Invoke(arguments[0], arguments[1]);
            }
            else if (function is Func<T, T, T, T>)
            {
                return ((Func<T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2]);
            }
            else if (function is Func<T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3]);
            }
            else if (function is Func<T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
            }
            else if (function is Func<T, T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
            }
            else if (function is Func<T, T, T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6]);
            }
            else if (function is Func<T, T, T, T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7]);
            }
            else if (function is Func<T, T, T, T, T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8]);
            }
            else if (function is Func<T, T, T, T, T, T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9]);
            }
            else if (function is Func<T, T, T, T, T, T, T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T, T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10]);
            }
            else if (function is Func<T, T, T, T, T, T, T, T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T, T, T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11]);
            }
            else if (function is Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12]);
            }
            else if (function is Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13]);
            }
            else if (function is Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13], arguments[14]);
            }
            else if (function is Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T>)
            {
                return ((Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T>)function).Invoke(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13], arguments[14], arguments[15]);
            }
            else if (function is DynamicFunc<T, T>)
            {
                return ((DynamicFunc<T, T>)function).Invoke(arguments);
            }
            else
            {
                return (T)function.DynamicInvoke((from s in arguments select (object)s).ToArray());
            }
        }
    }
}
