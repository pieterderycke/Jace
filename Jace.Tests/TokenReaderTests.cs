using System;
using System.Collections.Generic;
using System.Globalization;
using Jace.Tokenizer;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
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
    }
}
