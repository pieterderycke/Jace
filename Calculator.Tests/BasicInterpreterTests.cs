using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass]
    public class BasicInterpreterTests
    {
        [TestMethod]
        public void TestBasicInterpreter1()
        {
            IInterpreter interpreter = new BasicInterpreter();
            // 6 + (2 * 4)
            int result = interpreter.Execute(
                new Addition<int>(
                    new Constant<int>(6),
                    new Multiplication<int>(new Constant<int>(2), new Constant<int>(4))), null);

            Assert.AreEqual(14, result);
        }
    }
}
