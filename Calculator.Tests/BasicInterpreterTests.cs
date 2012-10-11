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
            IExecutor interpreter = new Interpreter();
            double result = interpreter.Execute(new Substraction(
                DataType.Integer,
                new IntegerConstant(6),
                new IntegerConstant(9)));

            Assert.AreEqual(-3.0, result);
        }

        [TestMethod]
        public void TestBasicInterpreter1()
        {
            IExecutor interpreter = new Interpreter();
            // 6 + (2 * 4)
            double result = interpreter.Execute(
                new Addition(
                    DataType.Integer,
                    new IntegerConstant(6),
                    new Multiplication(DataType.Integer, new IntegerConstant(2), new IntegerConstant(4))));

            Assert.AreEqual(14.0, result);
        }

        [TestMethod]
        public void TestBasicInterpreterWithVariables()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("age", 4);

            IExecutor interpreter = new Interpreter();
            // var1 + 2 * (3 * age)
            double result = interpreter.Execute(
                new Addition(DataType.FloatingPoint,
                    new Variable("var1"),
                    new Multiplication(
                        DataType.FloatingPoint,
                        new IntegerConstant(2),
                        new Multiplication(
                            DataType.FloatingPoint, 
                            new IntegerConstant(3), 
                            new Variable("age")))), variables);

            Assert.AreEqual(26.0, result);
        }
    }
}
