using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;
using Jace.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jace.Tests
{
    [TestClass]
    public class AstBuilderTests
    {
        [TestMethod]
        public void TestBuildFormula1()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = '(', TokenType = TokenType.LeftBracket }, 
                new Token() { Value = 42, TokenType = TokenType.Integer }, 
                new Token() { Value = '+', TokenType = TokenType.Operation }, 
                new Token() { Value = 8, TokenType = TokenType.Integer }, 
                new Token() { Value = ')', TokenType = TokenType.RightBracket }, 
                new Token() { Value = '*', TokenType = TokenType.Operation }, 
                new Token() { Value = 2, TokenType = TokenType.Integer } 
            });

            Multiplication multiplication = (Multiplication)operation;
            Addition addition = (Addition)multiplication.Argument1;

            Assert.AreEqual(42, ((Constant<int>)addition.Argument1).Value);
            Assert.AreEqual(8, ((Constant<int>)addition.Argument2).Value);
            Assert.AreEqual(2, ((Constant<int>)multiplication.Argument2).Value);
        }

        [TestMethod]
        public void TestBuildFormula2()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() {
                new Token() { Value = 2, TokenType = TokenType.Integer }, 
                new Token() { Value = '+', TokenType = TokenType.Operation }, 
                new Token() { Value = 8, TokenType = TokenType.Integer }, 
                new Token() { Value = '*', TokenType = TokenType.Operation }, 
                new Token() { Value = 3, TokenType = TokenType.Integer } 
            });

            Addition addition = (Addition)operation;
            Multiplication multiplication = (Multiplication)addition.Argument2;

            Assert.AreEqual(2, ((Constant<int>)addition.Argument1).Value);
            Assert.AreEqual(8, ((Constant<int>)multiplication.Argument1).Value);
            Assert.AreEqual(3, ((Constant<int>)multiplication.Argument2).Value);
        }

        [TestMethod]
        public void TestBuildFormula3()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() {
                new Token() { Value = 2, TokenType = TokenType.Integer }, 
                new Token() { Value = '*', TokenType = TokenType.Operation }, 
                new Token() { Value = 8, TokenType = TokenType.Integer }, 
                new Token() { Value = '-', TokenType = TokenType.Operation }, 
                new Token() { Value = 3, TokenType = TokenType.Integer }
            });

            Substraction substraction = (Substraction)operation;
            Multiplication multiplication = (Multiplication)substraction.Argument1;

            Assert.AreEqual(3, ((Constant<int>)substraction.Argument2).Value);
            Assert.AreEqual(2, ((Constant<int>)multiplication.Argument1).Value);
            Assert.AreEqual(8, ((Constant<int>)multiplication.Argument2).Value);
        }

        [TestMethod]
        public void TestDivision()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 10, TokenType = TokenType.Integer }, 
                new Token() { Value = '/', TokenType = TokenType.Operation }, 
                new Token() { Value = 2, TokenType = TokenType.Integer }
            });

            Assert.AreEqual(typeof(Division), operation.GetType());

            Division division = (Division)operation;

            Assert.AreEqual(new IntegerConstant(10), division.Dividend);
            Assert.AreEqual(new IntegerConstant(2), division.Divisor);
        }

        [TestMethod]
        public void TestMultiplication()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 10, TokenType = TokenType.Integer }, 
                new Token() { Value = '*', TokenType = TokenType.Operation }, 
                new Token() { Value = 2.0, TokenType = TokenType.FloatingPoint }
            });

            Multiplication multiplication = (Multiplication)operation;

            Assert.AreEqual(new IntegerConstant(10), multiplication.Argument1);
            Assert.AreEqual(new FloatingPointConstant(2.0), multiplication.Argument2);
        }

        [TestMethod]
        public void TestExponentiation()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 2, TokenType = TokenType.Integer }, 
                new Token() { Value = '^', TokenType = TokenType.Operation }, 
                new Token() { Value = 3, TokenType = TokenType.Integer }
            });

            Exponentiation exponentiation = (Exponentiation)operation;

            Assert.AreEqual(new IntegerConstant(2), exponentiation.Base);
            Assert.AreEqual(new IntegerConstant(3), exponentiation.Exponent);
        }

        [TestMethod]
        public void TestVariable()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 10, TokenType = TokenType.Integer }, 
                new Token() { Value = '*', TokenType = TokenType.Operation }, 
                new Token() { Value = "var1", TokenType = TokenType.Text }
            });

            Multiplication multiplication = (Multiplication)operation;

            Assert.AreEqual(new IntegerConstant(10), multiplication.Argument1);
            Assert.AreEqual(new Variable("var1"), multiplication.Argument2);
        }

        [TestMethod]
        public void TestMultipleVariable()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = "var1", TokenType = TokenType.Text }, 
                new Token() { Value = '+', TokenType = TokenType.Operation }, 
                new Token() { Value = 2, TokenType = TokenType.Integer }, 
                new Token() { Value = '*', TokenType = TokenType.Operation }, 
                new Token() { Value = '(', TokenType = TokenType.LeftBracket }, 
                new Token() { Value = 3, TokenType = TokenType.Integer }, 
                new Token() { Value = '*', TokenType = TokenType.Operation }, 
                new Token() { Value = "age", TokenType = TokenType.Text }, 
                new Token() { Value = ')', TokenType = TokenType.RightBracket }
            });

            Addition addition = (Addition)operation;
            Multiplication multiplication1 = (Multiplication)addition.Argument2;
            Multiplication multiplication2 = (Multiplication)multiplication1.Argument2;

            Assert.AreEqual(new Variable("var1"), addition.Argument1);
            Assert.AreEqual(new IntegerConstant(2), multiplication1.Argument1);
            Assert.AreEqual(new IntegerConstant(3), multiplication2.Argument1);
            Assert.AreEqual(new Variable("age"), multiplication2.Argument2);
        }

        [TestMethod]
        public void TestSinFunction1()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = "sin", TokenType = TokenType.Text }, 
                new Token() { Value = '(', TokenType = TokenType.LeftBracket }, 
                new Token() { Value = 2, TokenType = TokenType.Integer }, 
                new Token() { Value = ')', TokenType = TokenType.RightBracket }
            });

            Function sineFunction = (Function)operation;
            Assert.AreEqual(new IntegerConstant(2), sineFunction.Arguments.Single());
        }

        [TestMethod]
        public void TestSinFunction2()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = "sin", TokenType = TokenType.Text }, 
                new Token() { Value = '(', TokenType = TokenType.LeftBracket }, 
                new Token() { Value = 2, TokenType = TokenType.Integer }, 
                new Token() { Value = '+', TokenType = TokenType.Operation }, 
                new Token() { Value = 3, TokenType = TokenType.Integer }, 
                new Token() { Value = ')', TokenType = TokenType.RightBracket }
            });

            Function sineFunction = (Function)operation;

            Addition addition = (Addition)sineFunction.Arguments.Single();
            Assert.AreEqual(new IntegerConstant(2), addition.Argument1);
            Assert.AreEqual(new IntegerConstant(3), addition.Argument2);
        }

        [TestMethod]
        public void TestSinFunction3()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = "sin", TokenType = TokenType.Text }, 
                new Token() { Value = '(', TokenType = TokenType.LeftBracket }, 
                new Token() { Value = 2, TokenType = TokenType.Integer }, 
                new Token() { Value = '+', TokenType = TokenType.Operation }, 
                new Token() { Value = 3, TokenType = TokenType.Integer }, 
                new Token() { Value = ')', TokenType = TokenType.RightBracket },
                new Token() { Value = '*', TokenType = TokenType.Operation },
                new Token() { Value = 4.9, TokenType = TokenType.FloatingPoint }
            });

            Multiplication multiplication = (Multiplication)operation;

            Function sineFunction = (Function)multiplication.Argument1;

            Addition addition = (Addition)sineFunction.Arguments.Single();
            Assert.AreEqual(new IntegerConstant(2), addition.Argument1);
            Assert.AreEqual(new IntegerConstant(3), addition.Argument2);

            Assert.AreEqual(new FloatingPointConstant(4.9), multiplication.Argument2);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void TestBuildInvalidFormula1()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = '(', TokenType = TokenType.LeftBracket, StartPosition = 0 }, 
                new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 1 }, 
                new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 3 }, 
                new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 4 }, 
                new Token() { Value = ')', TokenType = TokenType.RightBracket, StartPosition = 5 }, 
                new Token() { Value = '*', TokenType = TokenType.Operation, StartPosition = 6 }, 
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void TestBuildInvalidFormula2()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 0 }, 
                new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 2 }, 
                new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 3 }, 
                new Token() { Value = ')', TokenType = TokenType.RightBracket, StartPosition = 4 }, 
                new Token() { Value = '*', TokenType = TokenType.Operation, StartPosition = 5 }, 
                new Token() { Value = 2, TokenType = TokenType.Integer, StartPosition = 6 }, 
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void TestBuildInvalidFormula3()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = '(', TokenType = TokenType.LeftBracket, StartPosition = 0 }, 
                new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 1 }, 
                new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 3 }, 
                new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 4 }
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void TestBuildInvalidFormula4()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 5, TokenType = TokenType.Integer, StartPosition = 0 }, 
                new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 1 }, 
                new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 3 }, 
                new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 4 }
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void TestBuildInvalidFormula5()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 0 }, 
                new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 2 }, 
                new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 3 },
                new Token() { Value = 5, TokenType = TokenType.Integer, StartPosition = 4 } 
            });
        }
    }
}
