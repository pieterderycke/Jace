using System;
using System.Collections.Generic;
using System.Globalization;
using Calculator.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass]
    public class TokenReaderTests
    {
        [TestMethod]
        public void TestTokenReader1()
        {
            TokenReader reader = new TokenReader();
            List<object> tokens = reader.Read("42+31");

            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual(42, tokens[0]);
            Assert.AreEqual('+', tokens[1]);
            Assert.AreEqual(31, tokens[2]);
        }

        [TestMethod]
        public void TestTokenReader2()
        {
            TokenReader reader = new TokenReader();
            List<object> tokens = reader.Read("(42+31)");

            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual('(', tokens[0]);
            Assert.AreEqual(42, tokens[1]);
            Assert.AreEqual('+', tokens[2]);
            Assert.AreEqual(31, tokens[3]);
            Assert.AreEqual(')', tokens[4]);
        }

        [TestMethod]
        public void TestTokenReader3()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<object> tokens = reader.Read("(42+31.0");

            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual('(', tokens[0]);
            Assert.AreEqual(42, tokens[1]);
            Assert.AreEqual('+', tokens[2]);
            Assert.AreEqual(31.0, tokens[3]);
        }

        [TestMethod]
        public void TestTokenReader4()
        {
            TokenReader reader = new TokenReader();
            List<object> tokens = reader.Read("(42+ 8) *2");

            Assert.AreEqual(7, tokens.Count);
            Assert.AreEqual('(', tokens[0]);
            Assert.AreEqual(42, tokens[1]);
            Assert.AreEqual('+', tokens[2]);
            Assert.AreEqual(8, tokens[3]);
            Assert.AreEqual(')', tokens[4]);
            Assert.AreEqual('*', tokens[5]);
            Assert.AreEqual(2, tokens[6]);
        }

        [TestMethod]
        public void TestTokenReader5()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<object> tokens = reader.Read("(42.87+31.0");

            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual('(', tokens[0]);
            Assert.AreEqual(42.87, tokens[1]);
            Assert.AreEqual('+', tokens[2]);
            Assert.AreEqual(31.0, tokens[3]);
        }

        [TestMethod]
        public void TestTokenReader6()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<object> tokens = reader.Read("(var+31.0");

            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual('(', tokens[0]);
            Assert.AreEqual("var", tokens[1]);
            Assert.AreEqual('+', tokens[2]);
            Assert.AreEqual(31.0, tokens[3]);
        }

        [TestMethod]
        public void TestTokenReader7()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<object> tokens = reader.Read("varb");

            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual("varb", tokens[0]);
        }

        [TestMethod]
        public void TestTokenReader8()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<object> tokens = reader.Read("varb(");

            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual("varb", tokens[0]);
            Assert.AreEqual('(', tokens[1]);
        }

        [TestMethod]
        public void TestTokenReader9()
        {
            TokenReader reader = new TokenReader(CultureInfo.InvariantCulture);
            List<object> tokens = reader.Read("+varb(");

            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual('+', tokens[0]);
            Assert.AreEqual("varb", tokens[1]);
            Assert.AreEqual('(', tokens[2]);
        }
    }
}
