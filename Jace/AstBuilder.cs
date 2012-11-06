using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;
using Jace.Tokenizer;

namespace Jace
{
    public class AstBuilder
    {
        private Dictionary<char, int> operationPrecedence = new Dictionary<char, int>();
        private Stack<Operation> resultStack = new Stack<Operation>();
        private Stack<Token> operatorStack = new Stack<Token>();

        public AstBuilder()
        {
            operationPrecedence.Add('(', 0);
            operationPrecedence.Add('+', 1);
            operationPrecedence.Add('-', 1);
            operationPrecedence.Add('*', 2);
            operationPrecedence.Add('/', 2);
            operationPrecedence.Add('^', 3);
        }

        public Operation Build(IList<Token> tokens)
        {
            resultStack.Clear();
            operatorStack.Clear();

            foreach (Token token in tokens)
            {
                object value = token.Value;

                switch (token.TokenType)
                {
                    case TokenType.Integer:
                        resultStack.Push(new IntegerConstant((int)token.Value));
                        break;
                    case TokenType.FloatingPoint:
                        resultStack.Push(new FloatingPointConstant((double)token.Value));
                        break;
                    case TokenType.Text:
                        if (IsFunctionName((string)token.Value))
                        {
                            operatorStack.Push(token);
                        }
                        else
                        {
                            resultStack.Push(new Variable((string)token.Value));
                        }
                        break;
                    case TokenType.LeftBracket:
                        operatorStack.Push(token);
                        break;
                    case TokenType.RightBracket:
                        PopOperations();
                        break;
                    case TokenType.Operation:
                        Token operation1Token = token;
                        char operation1 = (char)operation1Token.Value;

                        if (operatorStack.Count == 0)
                        {
                            operatorStack.Push(operation1Token);
                        }
                        else
                        {
                            Token operation2Token = operatorStack.Peek();
                            bool isFunctionOnTopOfStack = operation2Token.TokenType == TokenType.Text;

                            if (!isFunctionOnTopOfStack)
                            {
                                char operation2 = (char)operation2Token.Value;

                                if ((IsLeftAssociativeOperation(operation1) && operationPrecedence[operation1] <= operationPrecedence[operation2]) ||
                                    (operationPrecedence[operation1] < operationPrecedence[operation2]))
                                {
                                    operatorStack.Pop();
                                    operatorStack.Push(operation1Token);
                                    resultStack.Push(Convert(operation2));
                                }
                                else
                                {
                                    operatorStack.Push(operation1Token);
                                }
                            }
                            else
                            {
                                string function = (string)operation2Token.Value;

                                operatorStack.Pop();
                                operatorStack.Push(operation1Token);
                                resultStack.Push(Convert(function));
                            }
                        }

                        break;
                }
            }

            PopOperations();

            return resultStack.First();
        }

        private void PopOperations()
        {
            while (operatorStack.Count > 0 && operatorStack.Peek().TokenType != TokenType.LeftBracket)
            {
                Token token = operatorStack.Pop();

                switch (token.TokenType)
                {
                    case TokenType.Operation:
                        char operation = (char)token.Value;
                        resultStack.Push(Convert(operation));
                        break;
                    case TokenType.Text:
                        string function = (string)token.Value;
                        resultStack.Push(Convert(function));
                        break;
                }
            }

            if (operatorStack.Count > 0 && operatorStack.Peek().TokenType == TokenType.LeftBracket)
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
                case '^':
                    Operation exponent = resultStack.Pop();
                    Operation @base = resultStack.Pop();

                    return new Exponentiation(DataType.FloatingPoint, @base, exponent);
                default:
                    throw new ArgumentException(string.Format("Unknown operation \"{0}\".", operation), "operation");
            }
        }

        private Operation Convert(string function)
        {
            switch (function)
            {
                case "sin":
                    return new Function(DataType.FloatingPoint, FunctionType.Sine, new Operation[] { resultStack.Pop() });
                case "cos":
                    return new Function(DataType.FloatingPoint, FunctionType.Cosine, new Operation[] { resultStack.Pop() });
                case "loge":
                    return new Function(DataType.FloatingPoint, FunctionType.Loge, new Operation[] { resultStack.Pop() });
                case "log10":
                    return new Function(DataType.FloatingPoint, FunctionType.Log10, new Operation[] { resultStack.Pop() });
                case "logn":
                    Operation[] operations = new Operation[2];
                    operations[1] = resultStack.Pop();
                    operations[0] = resultStack.Pop();
                    return new Function(DataType.FloatingPoint, FunctionType.Logn, operations);
                default:
                    throw new ArgumentException(string.Format("Unknown function \"{0}\".", function), "function");
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

        private bool IsFunctionName(string text)
        {
            switch (text.ToLowerInvariant())
            {
                case "sin":
                case "cos":
                case "loge":
                case "log10":
                case "logn":
                    return true;
                default:
                    return false;
            }
        }
    }
}
