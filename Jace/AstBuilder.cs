using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;

namespace Jace
{
    public class AstBuilder
    {
        private Dictionary<char, int> operationPrecedence = new Dictionary<char, int>();
        private Stack<Operation> resultStack = new Stack<Operation>();
        private Stack<object> operatorStack = new Stack<object>();

        public AstBuilder()
        {
            operationPrecedence.Add('(', 0);
            operationPrecedence.Add('+', 1);
            operationPrecedence.Add('-', 1);
            operationPrecedence.Add('*', 2);
            operationPrecedence.Add('/', 2);
        }

        public Operation Build(IList<object> tokens)
        {
            resultStack.Clear();
            operatorStack.Clear();

            foreach (object token in tokens)
            {
                if (token.GetType() == typeof(int))
                    resultStack.Push(new IntegerConstant((int)token));
                else if (token.GetType() == typeof(double))
                    resultStack.Push(new FloatingPointConstant((double)token));
                else if (token.GetType() == typeof(string))
                    resultStack.Push(new Variable((string)token));
                else if (token.GetType() == typeof(char))
                {
                    char character = (char)token;

                    if (IsOperation(character))
                    {
                        char operation1 = character;

                        if (operatorStack.Count == 0)
                        {
                            operatorStack.Push(operation1);
                        }
                        else
                        {
                            char operation2 = (char)operatorStack.Peek();

                            if ((IsLeftAssociativeOperation(operation1) && operationPrecedence[operation1] <= operationPrecedence[operation2]) ||
                                (operationPrecedence[operation1] < operationPrecedence[operation2]))
                            {
                                operatorStack.Pop();
                                operatorStack.Push(operation1);
                                resultStack.Push(Convert(operation2));
                            }
                            else
                            {
                                operatorStack.Push(operation1);
                            }
                        }
                    }
                    else if (character == '(')
                    {
                        operatorStack.Push(character);
                    }
                    else if (character == ')')
                    {
                        PopOperations();
                    }
                }
            }

            PopOperations();

            return resultStack.First();
        }

        private void PopOperations()
        {
            while (operatorStack.Count > 0 && (char)operatorStack.Peek() != '(')
            {
                char operation = (char)operatorStack.Pop();
                resultStack.Push(Convert(operation));
            }

            if (operatorStack.Count > 0 && (char)operatorStack.Peek() == '(')
                operatorStack.Pop();
        }

        private Operation Convert(char operation)
        {
            DataType dataType;
            Operation argument1;
            Operation argument2;

            switch (operation)
            {
                case '+':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);

                    return new Addition(dataType, argument1, argument2);
                case '-':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);

                    return new Substraction(dataType, argument1, argument2);
                case '*':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);

                    return new Multiplication(dataType, argument1, argument2);
                case '/':
                    Operation divisor = resultStack.Pop();
                    Operation divident = resultStack.Pop();

                    return new Division(DataType.FloatingPoint, divident, divisor);
                default:
                    throw new ArgumentException(string.Format("Unknown operation \"{0}\".", operation), "operation");
            }
        }

        private bool IsOperation(char character)
        {
            return character == '*' || character == '+' || character == '-' || character == '/' || character == '^';
        }

        private bool IsLeftAssociativeOperation(char character)
        {
            return character == '*' || character == '+' || character == '-' || character == '/';
        }

        private DataType RequiredDataType(Operation argument1, Operation argument2)
        {
            return (argument1.DataType == DataType.FloatingPoint || argument2.DataType == DataType.FloatingPoint) ? DataType.FloatingPoint : DataType.Integer;
        }
    }
}
