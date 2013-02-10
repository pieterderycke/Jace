using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;
using Jace.Execution;

namespace Jace
{
    public class Optimizer
    {
        private readonly IExecutor executor;

        public Optimizer(IExecutor executor)
        {
            this.executor = executor;
        }

        public Operation Optimize(Operation operation, IFunctionRegistry functionRegistry)
        {
            if (!operation.DependsOnVariables && operation.GetType() != typeof(IntegerConstant)
                && operation.GetType() != typeof(FloatingPointConstant))
            {
                double result = executor.Execute(operation, functionRegistry);
                return new FloatingPointConstant(result);
            }
            else
            {
                if (operation.GetType() == typeof(Addition))
                {
                    Addition addition = (Addition)operation;
                    addition.Argument1 = Optimize(addition.Argument1, functionRegistry);
                    addition.Argument2 = Optimize(addition.Argument2, functionRegistry);
                }
                else if (operation.GetType() == typeof(Subtraction))
                {
                    Subtraction substraction = (Subtraction)operation;
                    substraction.Argument1 = Optimize(substraction.Argument1, functionRegistry);
                    substraction.Argument2 = Optimize(substraction.Argument2, functionRegistry);
                }
                else if (operation.GetType() == typeof(Multiplication))
                {
                    Multiplication multiplication = (Multiplication)operation;
                    multiplication.Argument1 = Optimize(multiplication.Argument1, functionRegistry);
                    multiplication.Argument2 = Optimize(multiplication.Argument2, functionRegistry);
                }
                else if (operation.GetType() == typeof(Division))
                {
                    Division division = (Division)operation;
                    division.Dividend = Optimize(division.Dividend, functionRegistry);
                    division.Divisor = Optimize(division.Divisor, functionRegistry);
                }
                else if (operation.GetType() == typeof(Exponentiation))
                {
                    Exponentiation division = (Exponentiation)operation;
                    division.Base = Optimize(division.Base, functionRegistry);
                    division.Exponent = Optimize(division.Exponent, functionRegistry);
                }

                return operation;
            }
        }
    }
}
