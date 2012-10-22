using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;

namespace Jace
{
    public class Optimizer
    {
        private readonly IExecutor executor;

        public Optimizer(IExecutor executor)
        {
            this.executor = executor;
        }

        public Operation Optimize(Operation operation)
        {
            if (!operation.DependsOnVariables && operation.GetType() != typeof(IntegerConstant)
                && operation.GetType() != typeof(FloatingPointConstant))
            {
                double result = executor.Execute(operation);
                return new FloatingPointConstant(result);
            }
            else
            {
                if (operation.GetType() == typeof(Addition))
                {
                    Addition addition = (Addition)operation;
                    addition.Argument1 = Optimize(addition.Argument1);
                    addition.Argument2 = Optimize(addition.Argument2);
                }
                else if (operation.GetType() == typeof(Substraction))
                {
                    Substraction substraction = (Substraction)operation;
                    substraction.Argument1 = Optimize(substraction.Argument1);
                    substraction.Argument2 = Optimize(substraction.Argument2);
                }
                else if (operation.GetType() == typeof(Multiplication))
                {
                    Multiplication multiplication = (Multiplication)operation;
                    multiplication.Argument1 = Optimize(multiplication.Argument1);
                    multiplication.Argument2 = Optimize(multiplication.Argument2);
                }
                else if (operation.GetType() == typeof(Division))
                {
                    Division division = (Division)operation;
                    division.Dividend = Optimize(division.Dividend);
                    division.Divisor = Optimize(division.Divisor);
                }
                else if (operation.GetType() == typeof(Exponentiation))
                {
                    Exponentiation division = (Exponentiation)operation;
                    division.Base = Optimize(division.Base);
                    division.Exponent = Optimize(division.Exponent);
                }

                return operation;
            }
        }
    }
}
