using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;
using Jace.Execution;

namespace Jace
{
    public class Optimizer<T>
    {
        private readonly IExecutor<T> executor;

        public Optimizer(IExecutor<T> executor)
        {
            this.executor = executor;
        }

        public Operation Optimize(Operation operation, IFunctionRegistry<T> functionRegistry, IConstantRegistry<T> constantRegistry)
        {
            if (!operation.DependsOnVariables && operation.IsIdempotent && operation.GetType() != typeof(IntegerConstant)
                && operation.GetType() != typeof(FloatingPointConstant<T>))
            {
                T result = executor.Execute(operation, functionRegistry, constantRegistry);
                return new FloatingPointConstant<T>(result);
            }
            else
            {
                if (operation.GetType() == typeof(Addition))
                {
                    Addition addition = (Addition)operation;
                    addition.Argument1 = Optimize(addition.Argument1, functionRegistry, constantRegistry);
                    addition.Argument2 = Optimize(addition.Argument2, functionRegistry, constantRegistry);
                }
                else if (operation.GetType() == typeof(Subtraction))
                {
                    Subtraction substraction = (Subtraction)operation;
                    substraction.Argument1 = Optimize(substraction.Argument1, functionRegistry, constantRegistry);
                    substraction.Argument2 = Optimize(substraction.Argument2, functionRegistry, constantRegistry);
                }
                else if (operation.GetType() == typeof(Multiplication))
                {
                    Multiplication multiplication = (Multiplication)operation;
                    multiplication.Argument1 = Optimize(multiplication.Argument1, functionRegistry, constantRegistry);
                    multiplication.Argument2 = Optimize(multiplication.Argument2, functionRegistry, constantRegistry);

                    if (typeof(T) == typeof(double))
                    {
                        if ((multiplication.Argument1.GetType() == typeof(FloatingPointConstant<double>) && ((FloatingPointConstant<double>)multiplication.Argument1).Value == 0.0)
                       || (multiplication.Argument2.GetType() == typeof(FloatingPointConstant<double>) && ((FloatingPointConstant<double>)multiplication.Argument2).Value == 0.0))
                        {
                            return new FloatingPointConstant<double>(0.0);
                        }
                    }
                    else
                    {
                        if ((multiplication.Argument1.GetType() == typeof(FloatingPointConstant<decimal>) && ((FloatingPointConstant<decimal>)multiplication.Argument1).Value == 0.0m)
                       || (multiplication.Argument2.GetType() == typeof(FloatingPointConstant<decimal>) && ((FloatingPointConstant<decimal>)multiplication.Argument2).Value == 0.0m))
                        {
                            return new FloatingPointConstant<decimal>(0.0m);
                        }
                    }
                   
                }
                else if (operation.GetType() == typeof(Division))
                {
                    Division division = (Division)operation;
                    division.Dividend = Optimize(division.Dividend, functionRegistry, constantRegistry);
                    division.Divisor = Optimize(division.Divisor, functionRegistry, constantRegistry);
                }
                else if (operation.GetType() == typeof(Exponentiation))
                {
                    Exponentiation division = (Exponentiation)operation;
                    division.Base = Optimize(division.Base, functionRegistry, constantRegistry);
                    division.Exponent = Optimize(division.Exponent, functionRegistry, constantRegistry);
                }
                else if(operation.GetType() == typeof(Function))
                {
                    Function function = (Function)operation;
                    IList<Operation> arguments = function.Arguments.Select(a => Optimize(a, functionRegistry, constantRegistry)).ToList();
                    function.Arguments = arguments;
                }

                return operation;
            }
        }
    }
}
