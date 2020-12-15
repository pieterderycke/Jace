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
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Jace.Tests
{
    [TestClass]
    public class TokenReaderTests
    {
        [TestMethod]
        public void TestTokenReader1()
        {
            TokenReader reader = new TokenReader();
            List<Token> tokens = reader.Read("42+31");

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
        }

        [TestMethod]
        public void TestTokenReader2()
        {
            TokenReader reader = new TokenReader();
            List<Token> tokens = reader.Read("(42+31)");

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
        }

        [TestMethod]
        public void TestTokenReader3()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("(42+31.0");

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
        }

        [TestMethod]
        public void TestTokenReader4()
        {
            TokenReader reader = new TokenReader();
            List<Token> tokens = reader.Read("(42+ 8) *2");

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
        }

        [TestMethod]
        public void TestTokenReader5()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("(42.87+31.0");

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
        }

        [TestMethod]
        public void TestTokenReader6()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("(var+31.0");

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
        }

        [TestMethod]
        public void TestTokenReader7()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("varb");

            Assert.AreEqual(1, tokens.Count);

            Assert.AreEqual("varb", tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(4, tokens[0].Length);
        }

        [TestMethod]
        public void TestTokenReader8()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("varb(");

            Assert.AreEqual(2, tokens.Count);
            
            Assert.AreEqual("varb", tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(4, tokens[0].Length);

            Assert.AreEqual('(', tokens[1].Value);
            Assert.AreEqual(4, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);
        }

        [TestMethod]
        public void TestTokenReader9()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("+varb(");

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
        }

        [TestMethod]
        public void TestTokenReader10()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("var1+2");

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
        }

        [TestMethod]
        public void TestTokenReader11()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("5.1%2");

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
        }

        [TestMethod]
        public void TestTokenReader12()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("-2.1");

            Assert.AreEqual(1, tokens.Count);

            Assert.AreEqual(-2.1, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(4, tokens[0].Length);
        }

        [TestMethod]
        public void TestTokenReader13()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("5-2");

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
        }

        [TestMethod]
        public void TestTokenReader14()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("5*-2");

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
        }

        [TestMethod]
        public void TestTokenReader15()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("5*(-2)");

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
        }

        [TestMethod]
        public void TestTokenReader16()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("5*-(2+43)");

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
        }

        [TestMethod]
        public void TestTokenReader17()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("logn(2,5)");

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
        }

        [TestMethod]
        public void TestTokenReader18()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("var_1+2");

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
        }

        [TestMethod]
        public void TestTokenReader19()
        {
            AssertExtensions.ThrowsException<ParseException>(() =>
            {
                TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
                List<Token> tokens = reader.Read("$1+$2+$3");
            });
        }

        [TestMethod]
        public void TestTokenReader20()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("2.11E-3");

            Assert.AreEqual(1, tokens.Count);
            
            Assert.AreEqual(2.11E-3, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(7, tokens[0].Length);
        }

        [TestMethod]
        public void TestTokenReader21()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("var_1+2.11E-3");

            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual("var_1", tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(5, tokens[0].Length);

            Assert.AreEqual('+', tokens[1].Value);
            Assert.AreEqual(5, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual(2.11E-3, tokens[2].Value);
            Assert.AreEqual(6, tokens[2].StartPosition);
            Assert.AreEqual(7, tokens[2].Length);
        }

        [TestMethod]
        public void TestTokenReader22()
        {
            AssertExtensions.ThrowsException<ParseException>(() =>
            {
                TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
                List<Token> tokens = reader.Read("2.11E-E3");
            });
        }

        [TestMethod]
        public void TestTokenReader23()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("2.11e3");

            Assert.AreEqual(1, tokens.Count);

            Assert.AreEqual(2.11E3, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(6, tokens[0].Length);
        }

        [TestMethod]
        public void TestTokenReader24()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("1 * e");

            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual(1, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual('*', tokens[1].Value);
            Assert.AreEqual(2, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual("e", tokens[2].Value);
            Assert.AreEqual(4, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);
        }

        [TestMethod]
        public void TestTokenReader25()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("e");

            Assert.AreEqual(1, tokens.Count);

            Assert.AreEqual("e", tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);
        }

        [TestMethod]
        public void TestTokenReader26()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("2.11e3+1.23E4");

            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual(2.11E3, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(6, tokens[0].Length);

            Assert.AreEqual('+', tokens[1].Value);

            Assert.AreEqual(1.23E4, tokens[2].Value);
            Assert.AreEqual(7, tokens[2].StartPosition);
            Assert.AreEqual(6, tokens[2].Length);
        }

        [TestMethod]
        public void TestTokenReader32()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("-e");

            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual('_', tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual("e", tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);
        }

        [TestMethod]
        public void TestTokenReader33()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("1-e");

            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual(1, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual('-', tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual("e", tokens[2].Value);
            Assert.AreEqual(2, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);
        }

        [TestMethod]
        public void TestTokenReader34()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<Token> tokens = reader.Read("1+e");

            Assert.AreEqual(3, tokens.Count);

            Assert.AreEqual(1, tokens[0].Value);
            Assert.AreEqual(0, tokens[0].StartPosition);
            Assert.AreEqual(1, tokens[0].Length);

            Assert.AreEqual('+', tokens[1].Value);
            Assert.AreEqual(1, tokens[1].StartPosition);
            Assert.AreEqual(1, tokens[1].Length);

            Assert.AreEqual("e", tokens[2].Value);
            Assert.AreEqual(2, tokens[2].StartPosition);
            Assert.AreEqual(1, tokens[2].Length);
        }

        [TestMethod]
        public void TestTokenReader35()
        {
            AssertExtensions.ThrowsException<ParseException>(() =>
            {
                TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
                List<Token> tokens = reader.Read("2.11E-e3");
            });
        }

        [TestMethod]
        public void TestTokenReader36()
        {
            AssertExtensions.ThrowsException<ParseException>(() =>
            {
                TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
                List<Token> tokens = reader.Read("2.11E-e");
            });
        }
    }
}
