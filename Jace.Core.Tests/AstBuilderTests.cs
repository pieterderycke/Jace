using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;
using Jace.Tokenizer;
using Jace.Execution;
using Jace.Tests.Mocks;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __ANDROID__
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif NETCORE
using Xunit;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Jace.Tests
{
#if !NETCORE
    [TestClass]
#endif
    public class AstBuilderTests
    {
#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestBuildFormula1()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
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

#if !NETCORE
            Assert.AreEqual(42, ((Constant<int>)addition.Argument1).Value);
            Assert.AreEqual(8, ((Constant<int>)addition.Argument2).Value);
            Assert.AreEqual(2, ((Constant<int>)multiplication.Argument2).Value);
#else
            Assert.Equal(42, ((Constant<int>)addition.Argument1).Value);
            Assert.Equal(8, ((Constant<int>)addition.Argument2).Value);
            Assert.Equal(2, ((Constant<int>)multiplication.Argument2).Value);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestBuildFormula2()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
            Operation operation = builder.Build(new List<Token>() {
                new Token() { Value = 2, TokenType = TokenType.Integer }, 
                new Token() { Value = '+', TokenType = TokenType.Operation }, 
                new Token() { Value = 8, TokenType = TokenType.Integer }, 
                new Token() { Value = '*', TokenType = TokenType.Operation }, 
                new Token() { Value = 3, TokenType = TokenType.Integer } 
            });

            Addition addition = (Addition)operation;
            Multiplication multiplication = (Multiplication)addition.Argument2;
#if !NETCORE
            Assert.AreEqual(2, ((Constant<int>)addition.Argument1).Value);
            Assert.AreEqual(8, ((Constant<int>)multiplication.Argument1).Value);
            Assert.AreEqual(3, ((Constant<int>)multiplication.Argument2).Value);
#else
            Assert.Equal(2, ((Constant<int>)addition.Argument1).Value);
            Assert.Equal(8, ((Constant<int>)multiplication.Argument1).Value);
            Assert.Equal(3, ((Constant<int>)multiplication.Argument2).Value);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestBuildFormula3()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
            Operation operation = builder.Build(new List<Token>() {
                new Token() { Value = 2, TokenType = TokenType.Integer }, 
                new Token() { Value = '*', TokenType = TokenType.Operation }, 
                new Token() { Value = 8, TokenType = TokenType.Integer }, 
                new Token() { Value = '-', TokenType = TokenType.Operation }, 
                new Token() { Value = 3, TokenType = TokenType.Integer }
            });

            Subtraction substraction = (Subtraction)operation;
            Multiplication multiplication = (Multiplication)substraction.Argument1;
#if !NETCORE
            Assert.AreEqual(3, ((Constant<int>)substraction.Argument2).Value);
            Assert.AreEqual(2, ((Constant<int>)multiplication.Argument1).Value);
            Assert.AreEqual(8, ((Constant<int>)multiplication.Argument2).Value);
#else
            Assert.Equal(3, ((Constant<int>)substraction.Argument2).Value);
            Assert.Equal(2, ((Constant<int>)multiplication.Argument1).Value);
            Assert.Equal(8, ((Constant<int>)multiplication.Argument2).Value);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestDivision()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 10, TokenType = TokenType.Integer }, 
                new Token() { Value = '/', TokenType = TokenType.Operation }, 
                new Token() { Value = 2, TokenType = TokenType.Integer }
            });
#if !NETCORE
            Assert.AreEqual(typeof(Division), operation.GetType());
#else
            Assert.Equal(typeof(Division), operation.GetType());
#endif
            Division division = (Division)operation;
#if !NETCORE
            Assert.AreEqual(new IntegerConstant(10), division.Dividend);
            Assert.AreEqual(new IntegerConstant(2), division.Divisor);
#else
            Assert.Equal(new IntegerConstant(10), division.Dividend);
            Assert.Equal(new IntegerConstant(2), division.Divisor);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestMultiplication()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 10, TokenType = TokenType.Integer }, 
                new Token() { Value = '*', TokenType = TokenType.Operation }, 
                new Token() { Value = 2.0, TokenType = TokenType.FloatingPoint }
            });

            Multiplication multiplication = (Multiplication)operation;
#if !NETCORE
            Assert.AreEqual(new IntegerConstant(10), multiplication.Argument1);
            Assert.AreEqual(new FloatingPointConstant(2.0), multiplication.Argument2);
#else
            Assert.Equal(new IntegerConstant(10), multiplication.Argument1);
            Assert.Equal(new FloatingPointConstant(2.0), multiplication.Argument2);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestExponentiation()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 2, TokenType = TokenType.Integer }, 
                new Token() { Value = '^', TokenType = TokenType.Operation }, 
                new Token() { Value = 3, TokenType = TokenType.Integer }
            });

            Exponentiation exponentiation = (Exponentiation)operation;
#if !NETCORE
            Assert.AreEqual(new IntegerConstant(2), exponentiation.Base);
            Assert.AreEqual(new IntegerConstant(3), exponentiation.Exponent);
#else
            Assert.Equal(new IntegerConstant(2), exponentiation.Base);
            Assert.Equal(new IntegerConstant(3), exponentiation.Exponent);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestModulo()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 2.7, TokenType = TokenType.FloatingPoint }, 
                new Token() { Value = '%', TokenType = TokenType.Operation }, 
                new Token() { Value = 3, TokenType = TokenType.Integer }
            });

            Modulo modulo = (Modulo)operation;
#if !NETCORE
            Assert.AreEqual(new FloatingPointConstant(2.7), modulo.Dividend);
            Assert.AreEqual(new IntegerConstant(3), modulo.Divisor);
#else
            Assert.Equal(new FloatingPointConstant(2.7), modulo.Dividend);
            Assert.Equal(new IntegerConstant(3), modulo.Divisor);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestVariable()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 10, TokenType = TokenType.Integer }, 
                new Token() { Value = '*', TokenType = TokenType.Operation }, 
                new Token() { Value = "var1", TokenType = TokenType.Text }
            });

            Multiplication multiplication = (Multiplication)operation;
#if !NETCORE
            Assert.AreEqual(new IntegerConstant(10), multiplication.Argument1);
            Assert.AreEqual(new Variable("var1"), multiplication.Argument2);
#else
            Assert.Equal(new IntegerConstant(10), multiplication.Argument1);
            Assert.Equal(new Variable("var1"), multiplication.Argument2);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestMultipleVariable()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
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

#if !NETCORE
            Assert.AreEqual(new Variable("var1"), addition.Argument1);
            Assert.AreEqual(new IntegerConstant(2), multiplication1.Argument1);
            Assert.AreEqual(new IntegerConstant(3), multiplication2.Argument1);
            Assert.AreEqual(new Variable("age"), multiplication2.Argument2);
#else
            Assert.Equal(new Variable("var1"), addition.Argument1);
            Assert.Equal(new IntegerConstant(2), multiplication1.Argument1);
            Assert.Equal(new IntegerConstant(3), multiplication2.Argument1);
            Assert.Equal(new Variable("age"), multiplication2.Argument2);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestSinFunction1()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = "sin", TokenType = TokenType.Text }, 
                new Token() { Value = '(', TokenType = TokenType.LeftBracket }, 
                new Token() { Value = 2, TokenType = TokenType.Integer }, 
                new Token() { Value = ')', TokenType = TokenType.RightBracket }
            });

            Function sineFunction = (Function)operation;
#if !NETCORE
            Assert.AreEqual(new IntegerConstant(2), sineFunction.Arguments.Single());
#else
            Assert.Equal(new IntegerConstant(2), sineFunction.Arguments.Single());
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestSinFunction2()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
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

#if !NETCORE
            Assert.AreEqual(new IntegerConstant(2), addition.Argument1);
            Assert.AreEqual(new IntegerConstant(3), addition.Argument2);
#else
            Assert.Equal(new IntegerConstant(2), addition.Argument1);
            Assert.Equal(new IntegerConstant(3), addition.Argument2);
#endif


        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestSinFunction3()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
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

#if !NETCORE
            Assert.AreEqual(new IntegerConstant(2), addition.Argument1);
            Assert.AreEqual(new IntegerConstant(3), addition.Argument2);
            Assert.AreEqual(new FloatingPointConstant(4.9), multiplication.Argument2);
#else
            Assert.Equal(new IntegerConstant(2), addition.Argument1);
            Assert.Equal(new IntegerConstant(3), addition.Argument2);
            Assert.Equal(new FloatingPointConstant(4.9), multiplication.Argument2);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestUnaryMinus()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
            Operation operation = builder.Build(new List<Token>() { 
                new Token() { Value = 5.3, TokenType = TokenType.FloatingPoint }, 
                new Token() { Value = '*', TokenType = TokenType.Operation}, 
                new Token() { Value = '_', TokenType = TokenType.Operation }, 
                new Token() { Value = '(', TokenType = TokenType.LeftBracket }, 
                new Token() { Value = 5, TokenType = TokenType.Integer }, 
                new Token() { Value = '+', TokenType = TokenType.Operation }, 
                new Token() { Value = 42, TokenType = TokenType.Integer }, 
                new Token() { Value = ')', TokenType = TokenType.RightBracket }, 
            });

            Multiplication multiplication = (Multiplication)operation;

#if !NETCORE
            Assert.AreEqual(new FloatingPointConstant(5.3), multiplication.Argument1);
#else
            Assert.Equal(new FloatingPointConstant(5.3), multiplication.Argument1);
#endif
            UnaryMinus unaryMinus = (UnaryMinus)multiplication.Argument2;
            Addition addition = (Addition)unaryMinus.Argument;
#if !NETCORE
          
            Assert.AreEqual(new IntegerConstant(5), addition.Argument1);
            Assert.AreEqual(new IntegerConstant(42), addition.Argument2);
#else
            Assert.Equal(new IntegerConstant(5), addition.Argument1);
            Assert.Equal(new IntegerConstant(42), addition.Argument2);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestBuildInvalidFormula1()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);

#if !NETCORE
            AssertExtensions.ThrowsException<ParseException>(() =>
                {
                    Operation operation = builder.Build(new List<Token>() { 
                        new Token() { Value = '(', TokenType = TokenType.LeftBracket, StartPosition = 0 }, 
                        new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 1 }, 
                        new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 3 }, 
                        new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 4 }, 
                        new Token() { Value = ')', TokenType = TokenType.RightBracket, StartPosition = 5 }, 
                        new Token() { Value = '*', TokenType = TokenType.Operation, StartPosition = 6 }, 
                    });
                });
#else
            Assert.Throws<ParseException>(() =>
            {
                Operation operation = builder.Build(new List<Token>() {
                        new Token() { Value = '(', TokenType = TokenType.LeftBracket, StartPosition = 0 },
                        new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 1 },
                        new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 3 },
                        new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 4 },
                        new Token() { Value = ')', TokenType = TokenType.RightBracket, StartPosition = 5 },
                        new Token() { Value = '*', TokenType = TokenType.Operation, StartPosition = 6 },
                    });
            });
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestBuildInvalidFormula2()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
#if !NETCORE
            AssertExtensions.ThrowsException<ParseException>(() =>
                {
                    Operation operation = builder.Build(new List<Token>() { 
                        new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 0 }, 
                        new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 2 }, 
                        new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 3 }, 
                        new Token() { Value = ')', TokenType = TokenType.RightBracket, StartPosition = 4 }, 
                        new Token() { Value = '*', TokenType = TokenType.Operation, StartPosition = 5 }, 
                        new Token() { Value = 2, TokenType = TokenType.Integer, StartPosition = 6 }, 
                    });
                });
#else
            Assert.Throws<ParseException>(() =>
            {
                Operation operation = builder.Build(new List<Token>() {
                        new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 0 },
                        new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 2 },
                        new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 3 },
                        new Token() { Value = ')', TokenType = TokenType.RightBracket, StartPosition = 4 },
                        new Token() { Value = '*', TokenType = TokenType.Operation, StartPosition = 5 },
                        new Token() { Value = 2, TokenType = TokenType.Integer, StartPosition = 6 },
                    });
            });
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestBuildInvalidFormula3()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
#if !NETCORE
            AssertExtensions.ThrowsException<ParseException>(() =>
                {
                    Operation operation = builder.Build(new List<Token>() { 
                        new Token() { Value = '(', TokenType = TokenType.LeftBracket, StartPosition = 0 }, 
                        new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 1 }, 
                        new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 3 }, 
                        new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 4 }
                    });
                });
#else
            Assert.Throws<ParseException>(() =>
            {
                Operation operation = builder.Build(new List<Token>() {
                        new Token() { Value = '(', TokenType = TokenType.LeftBracket, StartPosition = 0 },
                        new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 1 },
                        new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 3 },
                        new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 4 }
                    });
            });
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestBuildInvalidFormula4()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
#if !NETCORE
            AssertExtensions.ThrowsException<ParseException>(() =>
                {
                    Operation operation = builder.Build(new List<Token>() { 
                        new Token() { Value = 5, TokenType = TokenType.Integer, StartPosition = 0 }, 
                        new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 1 }, 
                        new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 3 }, 
                        new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 4 }
                    });
                });
#else
            Assert.Throws<ParseException>(() =>
            {
                Operation operation = builder.Build(new List<Token>() {
                        new Token() { Value = 5, TokenType = TokenType.Integer, StartPosition = 0 },
                        new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 1 },
                        new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 3 },
                        new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 4 }
                    });
            });
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestBuildInvalidFormula5()
        {
            IFunctionRegistry registry = new MockFunctionRegistry();

            AstBuilder builder = new AstBuilder(registry);
#if !NETCORE
            AssertExtensions.ThrowsException<ParseException>(() =>
                {
                    Operation operation = builder.Build(new List<Token>() { 
                        new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 0 }, 
                        new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 2 }, 
                        new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 3 },
                        new Token() { Value = 5, TokenType = TokenType.Integer, StartPosition = 4 } 
                    });
                });
#else
            Assert.Throws<ParseException>(() =>
            {
                Operation operation = builder.Build(new List<Token>() {
                        new Token() { Value = 42, TokenType = TokenType.Integer, StartPosition = 0 },
                        new Token() { Value = '+', TokenType = TokenType.Operation, StartPosition = 2 },
                        new Token() { Value = 8, TokenType = TokenType.Integer, StartPosition = 3 },
                        new Token() { Value = 5, TokenType = TokenType.Integer, StartPosition = 4 }
                    });
            });
#endif
        }
    }
}
