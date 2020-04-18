using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;
using Jace.Execution;
using Jace.Tests.Mocks;

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
    public class BasicInterpreterTests
    {
        [TestMethod]
        public void TestBasicInterpreterSubstraction()
        {
            var functionRegistry = new MockFunctionRegistry<double>();
            var constantRegistry = new MockConstantRegistry<double>();

            var executor = new Interpreter<double>(DoubleNumericalOperations.Instance);
            double result = executor.Execute(new Subtraction(
                DataType.Integer,
                new IntegerConstant(6),
                new IntegerConstant(9)), functionRegistry, constantRegistry);

            Assert.AreEqual(-3.0, result);
        }

        [TestMethod]
        public void TestBasicInterpreter1()
        {
            var functionRegistry = new MockFunctionRegistry<double>();
            var constantRegistry = new MockConstantRegistry<double>();

            var executor = new Interpreter<double>(DoubleNumericalOperations.Instance);
            // 6 + (2 * 4)
            double result = executor.Execute(
                new Addition(
                    DataType.Integer,
                    new IntegerConstant(6),
                    new Multiplication(
                        DataType.Integer, 
                        new IntegerConstant(2), 
                        new IntegerConstant(4))), functionRegistry, constantRegistry);

            Assert.AreEqual(14.0, result);
        }

        [TestMethod]
        public void TestBasicInterpreterWithVariables()
        {
            var functionRegistry = new MockFunctionRegistry<double>();
            var constantRegistry = new MockConstantRegistry<double>();

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("age", 4);

            var interpreter = new Interpreter<double>(DoubleNumericalOperations.Instance);

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
                            new Variable("age")))), functionRegistry, constantRegistry, variables);

            Assert.AreEqual(26.0, result);
        }
    }
}
