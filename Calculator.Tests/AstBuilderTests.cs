using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass]
    public class AstBuilderTests
    {
        [TestMethod]
        public void TestBuildAst1()
        {
            AstBuilder builder = new AstBuilder();
            Operation<int> operation = builder.Build(new List<object>() { '(', 42, '+', 8, ')', '*', 2 });

            Assert.AreEqual(typeof(Multiplication<int>), operation.GetType());
            Assert.AreEqual(typeof(Constant<int>), ((Multiplication<int>)operation).Argument1.GetType());
            Assert.AreEqual(typeof(Addition<int>), ((Multiplication<int>)operation).Argument2.GetType());
        }
    }
}
