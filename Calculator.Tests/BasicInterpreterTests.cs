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
        public void TestBasicInterpreterSubstraction()
        {
            IInterpreter interpreter = new BasicInterpreter();
            double result = interpreter.Execute(new Substraction(
                DataType.Integer,
                new Constant<int>(6),
                new Constant<int>(9)));

            Assert.AreEqual(-3.0, result);
        }

        [TestMethod]
        public void TestBasicInterpreter1()
        {
            IInterpreter interpreter = new BasicInterpreter();
            // 6 + (2 * 4)
            double result = interpreter.Execute(
                new Addition(
                    DataType.Integer,
                    new Constant<int>(6),
                    new Multiplication(DataType.Integer, new Constant<int>(2), new Constant<int>(4))));

            Assert.AreEqual(14.0, result);
        }
    }
}
