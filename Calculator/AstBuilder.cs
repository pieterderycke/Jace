using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.Operations;

namespace Calculator
{
    public class AstBuilder
    {
        private Dictionary<char, int> operationPrecedence = new Dictionary<char, int>();

        public AstBuilder()
        {
            operationPrecedence.Add('(', 0);
            operationPrecedence.Add('+', 1);
            operationPrecedence.Add('-', 1);
            operationPrecedence.Add('*', 2);
        }

        public Operation<int> Build(IList<object> tokens)
        {
            Stack<Operation<int>> resultStack = new Stack<Operation<int>>();
            Stack<object> operatorStack = new Stack<object>();

            foreach (object token in tokens)
            {
                if (token.GetType() == typeof(int))
                    resultStack.Push(new Constant<int>((int)token));
                else if(token.GetType() == typeof(char))
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

                            if (operationPrecedence[operation1] > operationPrecedence[operation2])
                            {
                                operatorStack.Push(operation1);
                            }
                            else
                            {
                                operatorStack.Pop();
                                operatorStack.Push(operation1);
                                resultStack.Push(Convert(operation1, resultStack));
                            }
                        }
                    }
                    else if (character == '(')
                    {
                        operatorStack.Push(character);
                    }
                    else if (character == ')')
                    {
                        PopOperations(operatorStack, resultStack);
                    }
                }
            }

            PopOperations(operatorStack, resultStack);

            return resultStack.First();
        }

        private void PopOperations(Stack<object> operatorStack, Stack<Operation<int>> resultStack)
        {
            while (operatorStack.Count > 0 && (char)operatorStack.Peek() != '(')
            {
                char operation = (char)operatorStack.Pop();
                resultStack.Push(Convert(operation, resultStack));
            }
        }

        private Operation<int> Convert(char operation, Stack<Operation<int>> resultStack)
        {
            switch (operation)
            {
                case '+':
                    return new Addition<int>(resultStack.Pop(), resultStack.Pop());
                case '-':
                    return new Substraction<int>(resultStack.Pop(), resultStack.Pop());
                case '*':
                    return new Multiplication<int>(resultStack.Pop(), resultStack.Pop());
                default:
                    throw new ArgumentException(string.Format("Unknown operation \"{0}\".", operation), "operation");
            }
        }

        private bool IsOperation(char character)
        {
            return character == '*' || character == '+';
        }
    }
}
