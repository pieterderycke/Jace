using System;
using System.Collections.Generic;
using System.Globalization;
using Jace.Tokenizer;

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
    public class TokenReaderTests
    {
#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader1()
        {
            TokenReader reader = new TokenReader();
            List<Token> tokens = reader.Read("42+31");
#if !NETCORE
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual(42, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(2, tokens[0].Length);

            Assert.AreEqual('+', tokens[1].Value);
            Assert.AreEqual(2, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual(31, tokens[2].Value);
            Assert.AreEqual(3, tokens[2].StartPosition);
            Assert.AreEqual(2, tokens[2].Length);
#else
            Assert.Equal(3, tokens.Count);

            Assert.Equal(42, tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(2, tokens[0].Length);

            Assert.Equal('+', tokens[1].Value);
            Assert.Equal(2, tokens[1].StartPosition);
            Assert.Equal(1, tokens[1].Length);

            Assert.Equal(31, tokens[2].Value);
            Assert.Equal(3, tokens[2].StartPosition);
            Assert.Equal(2, tokens[2].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader2()
        {
            TokenReader reader = new TokenReader();
            List<Token> tokens = reader.Read("(42+31)");
#if !NETCORE
            Assert.AreEqual(5, tokens.Count);

            Assert.AreEqual('(', tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual(42, tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(2, tokens[1].Length);
            
            Assert.AreEqual('+', tokens[2].Value);
            Assert.AreEqual(3, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);
            
            Assert.AreEqual(31, tokens[3].Value);
            Assert.AreEqual(4, tokens[3].StartPosition);
            Assert.AreEqual(2, tokens[3].Length);
            
            Assert.AreEqual(')', tokens[4].Value);
            Assert.AreEqual(6, tokens[4].StartPosition);
            Assert.AreEqual(1, tokens[4].Length);
#else
            Assert.Equal(5, tokens.Count);

            Assert.Equal('(', tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(1, tokens[0].Length);

            Assert.Equal(42, tokens[1].Value);
            Assert.Equal(1, tokens[1].StartPosition);
            Assert.Equal(2, tokens[1].Length);

            Assert.Equal('+', tokens[2].Value);
            Assert.Equal(3, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);

            Assert.Equal(31, tokens[3].Value);
            Assert.Equal(4, tokens[3].StartPosition);
            Assert.Equal(2, tokens[3].Length);

            Assert.Equal(')', tokens[4].Value);
            Assert.Equal(6, tokens[4].StartPosition);
            Assert.Equal(1, tokens[4].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader3()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("(42+31.0");
#if !NETCORE
            Assert.AreEqual(4, tokens.Count);

            Assert.AreEqual('(', tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual(42, tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(2, tokens[1].Length);

            Assert.AreEqual('+', tokens[2].Value);
            Assert.AreEqual(3, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);

            Assert.AreEqual(31.0, tokens[3].Value);
            Assert.AreEqual(4, tokens[3].StartPosition);
            Assert.AreEqual(4, tokens[3].Length);
#else
            Assert.Equal(4, tokens.Count);

            Assert.Equal('(', tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(1, tokens[0].Length);

            Assert.Equal(42, tokens[1].Value);
            Assert.Equal(1, tokens[1].StartPosition);
            Assert.Equal(2, tokens[1].Length);

            Assert.Equal('+', tokens[2].Value);
            Assert.Equal(3, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);

            Assert.Equal(31.0, tokens[3].Value);
            Assert.Equal(4, tokens[3].StartPosition);
            Assert.Equal(4, tokens[3].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader4()
        {
            TokenReader reader = new TokenReader();
            List<Token> tokens = reader.Read("(42+ 8) *2");
#if !NETCORE
            Assert.AreEqual(7, tokens.Count);
            
            Assert.AreEqual('(', tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual(42, tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(2, tokens[1].Length);

            Assert.AreEqual('+', tokens[2].Value);
            Assert.AreEqual(3, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);

            Assert.AreEqual(8, tokens[3].Value);
            Assert.AreEqual(5, tokens[3].StartPosition);
            Assert.AreEqual(1, tokens[3].Length);

            Assert.AreEqual(')', tokens[4].Value);
            Assert.AreEqual(6, tokens[4].StartPosition);
            Assert.AreEqual(1, tokens[4].Length);

            Assert.AreEqual('*', tokens[5].Value);
            Assert.AreEqual(8, tokens[5].StartPosition);
            Assert.AreEqual(1, tokens[5].Length);

            Assert.AreEqual(2, tokens[6].Value);
            Assert.AreEqual(9, tokens[6].StartPosition);
            Assert.AreEqual(1, tokens[6].Length);
#else
            Assert.Equal(7, tokens.Count);

            Assert.Equal('(', tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(1, tokens[0].Length);

            Assert.Equal(42, tokens[1].Value);
            Assert.Equal(1, tokens[1].StartPosition);
            Assert.Equal(2, tokens[1].Length);

            Assert.Equal('+', tokens[2].Value);
            Assert.Equal(3, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);

            Assert.Equal(8, tokens[3].Value);
            Assert.Equal(5, tokens[3].StartPosition);
            Assert.Equal(1, tokens[3].Length);

            Assert.Equal(')', tokens[4].Value);
            Assert.Equal(6, tokens[4].StartPosition);
            Assert.Equal(1, tokens[4].Length);

            Assert.Equal('*', tokens[5].Value);
            Assert.Equal(8, tokens[5].StartPosition);
            Assert.Equal(1, tokens[5].Length);

            Assert.Equal(2, tokens[6].Value);
            Assert.Equal(9, tokens[6].StartPosition);
            Assert.Equal(1, tokens[6].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader5()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("(42.87+31.0");
#if !NETCORE
            Assert.AreEqual(4, tokens.Count);

            Assert.AreEqual('(', tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);
            
            Assert.AreEqual(42.87, tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(5, tokens[1].Length);

            Assert.AreEqual('+', tokens[2].Value);
            Assert.AreEqual(6, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);

            Assert.AreEqual(31.0, tokens[3].Value);
            Assert.AreEqual(7, tokens[3].StartPosition);
            Assert.AreEqual(4, tokens[3].Length);
#else
            Assert.Equal(4, tokens.Count);

            Assert.Equal('(', tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(1, tokens[0].Length);

            Assert.Equal(42.87, tokens[1].Value);
            Assert.Equal(1, tokens[1].StartPosition);
            Assert.Equal(5, tokens[1].Length);

            Assert.Equal('+', tokens[2].Value);
            Assert.Equal(6, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);

            Assert.Equal(31.0, tokens[3].Value);
            Assert.Equal(7, tokens[3].StartPosition);
            Assert.Equal(4, tokens[3].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader6()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("(var+31.0");
#if !NETCORE
            Assert.AreEqual(4, tokens.Count);

            Assert.AreEqual('(', tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual("var", tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(3, tokens[1].Length);

            Assert.AreEqual('+', tokens[2].Value);
            Assert.AreEqual(4, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);

            Assert.AreEqual(31.0, tokens[3].Value);
            Assert.AreEqual(5, tokens[3].StartPosition);
            Assert.AreEqual(4, tokens[3].Length);
#else
            Assert.Equal(4, tokens.Count);

            Assert.Equal('(', tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(1, tokens[0].Length);

            Assert.Equal("var", tokens[1].Value);
            Assert.Equal(1, tokens[1].StartPosition);
            Assert.Equal(3, tokens[1].Length);

            Assert.Equal('+', tokens[2].Value);
            Assert.Equal(4, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);

            Assert.Equal(31.0, tokens[3].Value);
            Assert.Equal(5, tokens[3].StartPosition);
            Assert.Equal(4, tokens[3].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader7()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("varb");
#if !NETCORE
            Assert.AreEqual(1, tokens.Count);

            Assert.AreEqual("varb", tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(4, tokens[0].Length);
#else
            Assert.Equal(1, tokens.Count);

            Assert.Equal("varb", tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(4, tokens[0].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader8()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("varb(");
#if !NETCORE
            Assert.AreEqual(2, tokens.Count);
            
            Assert.AreEqual("varb", tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(4, tokens[0].Length);

            Assert.AreEqual('(', tokens[1].Value);
            Assert.AreEqual(4, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);
#else
            Assert.Equal(2, tokens.Count);

            Assert.Equal("varb", tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(4, tokens[0].Length);

            Assert.Equal('(', tokens[1].Value);
            Assert.Equal(4, tokens[1].StartPosition);
            Assert.Equal(1, tokens[1].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader9()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("+varb(");
#if !NETCORE
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual('+', tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual("varb", tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(4, tokens[1].Length);

            Assert.AreEqual('(', tokens[2].Value);
            Assert.AreEqual(5, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);
#else
            Assert.Equal(3, tokens.Count);

            Assert.Equal('+', tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(1, tokens[0].Length);

            Assert.Equal("varb", tokens[1].Value);
            Assert.Equal(1, tokens[1].StartPosition);
            Assert.Equal(4, tokens[1].Length);

            Assert.Equal('(', tokens[2].Value);
            Assert.Equal(5, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader10()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("var1+2");
#if !NETCORE
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual("var1", tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(4, tokens[0].Length);

            Assert.AreEqual('+', tokens[1].Value);
            Assert.AreEqual(4, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual(2, tokens[2].Value);
            Assert.AreEqual(5, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);
#else
            Assert.Equal(3, tokens.Count);

            Assert.Equal("var1", tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(4, tokens[0].Length);

            Assert.Equal('+', tokens[1].Value);
            Assert.Equal(4, tokens[1].StartPosition);
            Assert.Equal(1, tokens[1].Length);

            Assert.Equal(2, tokens[2].Value);
            Assert.Equal(5, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader11()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("5.1%2");
#if !NETCORE
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual(5.1, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(3, tokens[0].Length);

            Assert.AreEqual('%', tokens[1].Value);
            Assert.AreEqual(3, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual(2, tokens[2].Value);
            Assert.AreEqual(4, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);
#else
            Assert.Equal(3, tokens.Count);

            Assert.Equal(5.1, tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(3, tokens[0].Length);

            Assert.Equal('%', tokens[1].Value);
            Assert.Equal(3, tokens[1].StartPosition);
            Assert.Equal(1, tokens[1].Length);

            Assert.Equal(2, tokens[2].Value);
            Assert.Equal(4, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader12()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("-2.1");
#if !NETCORE
            Assert.AreEqual(1, tokens.Count);

            Assert.AreEqual(-2.1, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(4, tokens[0].Length);
#else
            Assert.Equal(1, tokens.Count);

            Assert.Equal(-2.1, tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(4, tokens[0].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader13()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("5-2");
#if !NETCORE
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual(5, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual('-', tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual(2, tokens[2].Value);
            Assert.AreEqual(2, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);
#else
            Assert.Equal(3, tokens.Count);

            Assert.Equal(5, tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(1, tokens[0].Length);

            Assert.Equal('-', tokens[1].Value);
            Assert.Equal(1, tokens[1].StartPosition);
            Assert.Equal(1, tokens[1].Length);

            Assert.Equal(2, tokens[2].Value);
            Assert.Equal(2, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader14()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("5*-2");
#if !NETCORE
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual(5, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual('*', tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual(-2, tokens[2].Value);
            Assert.AreEqual(2, tokens[2].StartPosition);
            Assert.AreEqual(2, tokens[2].Length);
#else
            Assert.Equal(3, tokens.Count);

            Assert.Equal(5, tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(1, tokens[0].Length);

            Assert.Equal('*', tokens[1].Value);
            Assert.Equal(1, tokens[1].StartPosition);
            Assert.Equal(1, tokens[1].Length);

            Assert.Equal(-2, tokens[2].Value);
            Assert.Equal(2, tokens[2].StartPosition);
            Assert.Equal(2, tokens[2].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader15()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("5*(-2)");
#if !NETCORE
                        Assert.AreEqual(5, tokens.Count);

            Assert.AreEqual(5, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual('*', tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual('(', tokens[2].Value);
            Assert.AreEqual(2, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);

            Assert.AreEqual(-2, tokens[3].Value);
            Assert.AreEqual(3, tokens[3].StartPosition);
            Assert.AreEqual(2, tokens[3].Length);

            Assert.AreEqual(')', tokens[4].Value);
            Assert.AreEqual(5, tokens[4].StartPosition);
            Assert.AreEqual(1, tokens[4].Length);
#else
            Assert.Equal(5, tokens.Count);

            Assert.Equal(5, tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(1, tokens[0].Length);

            Assert.Equal('*', tokens[1].Value);
            Assert.Equal(1, tokens[1].StartPosition);
            Assert.Equal(1, tokens[1].Length);

            Assert.Equal('(', tokens[2].Value);
            Assert.Equal(2, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);

            Assert.Equal(-2, tokens[3].Value);
            Assert.Equal(3, tokens[3].StartPosition);
            Assert.Equal(2, tokens[3].Length);

            Assert.Equal(')', tokens[4].Value);
            Assert.Equal(5, tokens[4].StartPosition);
            Assert.Equal(1, tokens[4].Length);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader16()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("5*-(2+43)");


#if !NETCORE
            Assert.AreEqual(8, tokens.Count);

            Assert.AreEqual(5, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual('*', tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual('_', tokens[2].Value);
            Assert.AreEqual(2, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);

            Assert.AreEqual('(', tokens[3].Value);
            Assert.AreEqual(3, tokens[3].StartPosition);
            Assert.AreEqual(1, tokens[3].Length);

            Assert.AreEqual(2, tokens[4].Value);
            Assert.AreEqual(4, tokens[4].StartPosition);
            Assert.AreEqual(1, tokens[4].Length);

            Assert.AreEqual('+', tokens[5].Value);
            Assert.AreEqual(5, tokens[5].StartPosition);
            Assert.AreEqual(1, tokens[5].Length);

            Assert.AreEqual(43, tokens[6].Value);
            Assert.AreEqual(6, tokens[6].StartPosition);
            Assert.AreEqual(2, tokens[6].Length);

            Assert.AreEqual(')', tokens[7].Value);
            Assert.AreEqual(8, tokens[7].StartPosition);
            Assert.AreEqual(1, tokens[7].Length);
#else
            Assert.Equal(8, tokens.Count);

            Assert.Equal(5, tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(1, tokens[0].Length);

            Assert.Equal('*', tokens[1].Value);
            Assert.Equal(1, tokens[1].StartPosition);
            Assert.Equal(1, tokens[1].Length);

            Assert.Equal('_', tokens[2].Value);
            Assert.Equal(2, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);

            Assert.Equal('(', tokens[3].Value);
            Assert.Equal(3, tokens[3].StartPosition);
            Assert.Equal(1, tokens[3].Length);

            Assert.Equal(2, tokens[4].Value);
            Assert.Equal(4, tokens[4].StartPosition);
            Assert.Equal(1, tokens[4].Length);

            Assert.Equal('+', tokens[5].Value);
            Assert.Equal(5, tokens[5].StartPosition);
            Assert.Equal(1, tokens[5].Length);

            Assert.Equal(43, tokens[6].Value);
            Assert.Equal(6, tokens[6].StartPosition);
            Assert.Equal(2, tokens[6].Length);

            Assert.Equal(')', tokens[7].Value);
            Assert.Equal(8, tokens[7].StartPosition);
            Assert.Equal(1, tokens[7].Length);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader17()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("logn(2,5)");

#if !NETCORE

            Assert.AreEqual(6, tokens.Count);

            Assert.AreEqual("logn", tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(4, tokens[0].Length);

            Assert.AreEqual('(', tokens[1].Value);
            Assert.AreEqual(4, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);
            Assert.AreEqual(TokenType.LeftBracket, tokens[1].TokenType);

            Assert.AreEqual(2, tokens[2].Value);
            Assert.AreEqual(5, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);

            Assert.AreEqual(',', tokens[3].Value);
            Assert.AreEqual(6, tokens[3].StartPosition);
            Assert.AreEqual(1, tokens[3].Length);
            Assert.AreEqual(TokenType.ArgumentSeparator, tokens[3].TokenType);

            Assert.AreEqual(5, tokens[4].Value);
            Assert.AreEqual(7, tokens[4].StartPosition);
            Assert.AreEqual(1, tokens[4].Length);

            Assert.AreEqual(')', tokens[5].Value);
            Assert.AreEqual(8, tokens[5].StartPosition);
            Assert.AreEqual(1, tokens[5].Length);
            Assert.AreEqual(TokenType.RightBracket, tokens[5].TokenType);
#else

            Assert.Equal(6, tokens.Count);

            Assert.Equal("logn", tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(4, tokens[0].Length);

            Assert.Equal('(', tokens[1].Value);
            Assert.Equal(4, tokens[1].StartPosition);
            Assert.Equal(1, tokens[1].Length);
            Assert.Equal(TokenType.LeftBracket, tokens[1].TokenType);

            Assert.Equal(2, tokens[2].Value);
            Assert.Equal(5, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);

            Assert.Equal(',', tokens[3].Value);
            Assert.Equal(6, tokens[3].StartPosition);
            Assert.Equal(1, tokens[3].Length);
            Assert.Equal(TokenType.ArgumentSeparator, tokens[3].TokenType);

            Assert.Equal(5, tokens[4].Value);
            Assert.Equal(7, tokens[4].StartPosition);
            Assert.Equal(1, tokens[4].Length);

            Assert.Equal(')', tokens[5].Value);
            Assert.Equal(8, tokens[5].StartPosition);
            Assert.Equal(1, tokens[5].Length);
            Assert.Equal(TokenType.RightBracket, tokens[5].TokenType);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader18()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("var_1+2");


#if !NETCORE
            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual("var_1", tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(5, tokens[0].Length);

            Assert.AreEqual('+', tokens[1].Value);
            Assert.AreEqual(5, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual(2, tokens[2].Value);
            Assert.AreEqual(6, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);
#else
            Assert.Equal(3, tokens.Count);

            Assert.Equal("var_1", tokens[0].Value);
            Assert.Equal(0, tokens[0].StartPosition);
            Assert.Equal(5, tokens[0].Length);

            Assert.Equal('+', tokens[1].Value);
            Assert.Equal(5, tokens[1].StartPosition);
            Assert.Equal(1, tokens[1].Length);

            Assert.Equal(2, tokens[2].Value);
            Assert.Equal(6, tokens[2].StartPosition);
            Assert.Equal(1, tokens[2].Length);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestTokenReader19()
        {
#if !NETCORE
            AssertExtensions.ThrowsException<ParseException>(() =>
            {
                TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
                List<Token> tokens = reader.Read("$1+$2+$3");
            });
#else
            Assert.Throws<ParseException>(() =>
            {
                TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
                List<Token> tokens = reader.Read("$1+$2+$3");
            });
#endif

        }
    }
}
