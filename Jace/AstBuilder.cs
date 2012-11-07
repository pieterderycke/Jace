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
                        PopOperations(true, token);
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
                                    resultStack.Push(Convert(operation2Token));
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

            PopOperations(false, null);

            VerifyResultStack();

            return resultStack.First();
        }

        private void PopOperations(bool untillLeftBracket, Token? currentToken)
        {
            if (untillLeftBracket && !currentToken.HasValue)
                throw new ArgumentNullException("currentToken", "If the parameter \"untillLeftBracket\" is set to true, " +
                    "the parameter \"currentToken\" cannot be null.");

            while (operatorStack.Count > 0 && operatorStack.Peek().TokenType != TokenType.LeftBracket)
            {
                Token token = operatorStack.Pop();

                switch (token.TokenType)
                {
                    case TokenType.Operation:
                        resultStack.Push(Convert(token));
                        break;
                    case TokenType.Text:
                        string function = (string)token.Value;
                        resultStack.Push(Convert(function));
                        break;
                }
            }

            if (untillLeftBracket)
            {
                if (operatorStack.Count > 0 && operatorStack.Peek().TokenType == TokenType.LeftBracket)
                    operatorStack.Pop();
                else
                    throw new ParseException(string.Format("No matching left bracket found for the right " +
                        "bracket at position {0}.", currentToken.Value.StartPosition));
            }
            else
            {
                if (operatorStack.Count > 0 && operatorStack.Peek().TokenType == TokenType.LeftBracket)
                    throw new ParseException(string.Format("No matching right bracket found for the left " +
                        "bracket at position {0}.", operatorStack.Peek().StartPosition));
            }
        }

        private Operation Convert(Token operationToken)
        {
            try
            {
                DataType dataType;
                Operation argument1;
                Operation argument2;

                switch ((char)operationToken.Value)
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
                        throw new ArgumentException(string.Format("Unknown operation \"{0}\".", operationToken), "operation");
                }
            }
            catch (InvalidOperationException)
            {
                // If we encounter a Stack empty issue this means there is a syntax issue in 
                // the mathematical function
                throw new ParseException(string.Format("There is a syntax issue for the operation \"{0}\" at position {1}. " +
                    "The number of arguments does not match with what is expected.", operationToken.Value, operationToken.StartPosition));
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

        private void VerifyResultStack()
        {
            if(resultStack.Count > 1)
            {
                Operation[] operations = resultStack.ToArray();

                for (int i = 1; i < operations.Length; i++)
                {
                    Operation operation = operations[i];

                    if (operation.GetType() == typeof(IntegerConstant))
                    {
                        IntegerConstant constant = (IntegerConstant)operation;
                        throw new ParseException(string.Format("Unexpected integer constant \"{0}\" found.", constant.Value));
                    }
                    else if (operation.GetType() == typeof(FloatingPointConstant))
                    {
                        FloatingPointConstant constant = (FloatingPointConstant)operation;
                        throw new ParseException(string.Format("Unexpected floating point constant \"{0}\" found.", constant.Value)); 
                    }
                }

                throw new ParseException("The syntax of the provided function is not valid.");
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
