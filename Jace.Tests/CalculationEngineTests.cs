using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Jace.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jace.Tests
{
    [TestClass]
    public class CalculationEngineTests
    {
        [TestMethod]
        public void TestCalculationFormula1FloatingPointCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            double result = engine.Calculate("2.0+3.0");

            Assert.AreEqual(5.0, result);
        }

        [TestMethod]
        public void TestCalculationFormula1IntegersCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            double result = engine.Calculate("2+3");

            Assert.AreEqual(5.0, result);
        }

        [TestMethod]
        public void TestCalculateFormula1()
        {
            CalculationEngine engine = new CalculationEngine();
            double result = engine.Calculate("2+3");

            Assert.AreEqual(5.0, result);
        }

        [TestMethod]
        public void TestCalculatePowCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            double result = engine.Calculate("2^3.0");

            Assert.AreEqual(8.0, result);
        }

        [TestMethod]
        public void TestCalculatePowInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            double result = engine.Calculate("2^3.0");

            Assert.AreEqual(8.0, result);
        }

        [TestMethod]
        public void TestCalculateFormulaWithVariables()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2.5);
            variables.Add("var2", 3.4);

            CalculationEngine engine = new CalculationEngine();
            double result = engine.Calculate("var1*var2", variables);

            Assert.AreEqual(8.5, result);
        }

        [TestMethod]
        [ExpectedException(typeof(VariableNotDefinedException))]
        public void TestCalculateFormulaVariableNotDefinedInterpreted()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2.5);

            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            double result = engine.Calculate("var1*var2", variables);
        }

        [TestMethod]
        [ExpectedException(typeof(VariableNotDefinedException))]
        public void TestCalculateFormulaVariableNotDefinedCompiled()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2.5);

            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            double result = engine.Calculate("var1*var2", variables);
        }

        [TestMethod]
        public void TestBuild()
        { 
            CalculationEngine engine = new CalculationEngine();
            Func<Dictionary<string, double>, double> function = engine.Build("var1+2*(3*age)");

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("age", 4);

            double result = function(variables);
            Assert.AreEqual(26.0, result);
        }

        [TestMethod]
        public void TestFunctionBuilder()
        {
            CalculationEngine engine = new CalculationEngine();
            Func<int, double, double> function = (Func<int, double, double>)engine.Function("var1+2*(3*age)")
                .Parameter("var1", DataType.Integer)
                .Parameter("age", DataType.FloatingPoint)
                .Result(DataType.FloatingPoint)
                .Build();

            double result = function(2, 4);
            Assert.AreEqual(26.0, result);
        }

        [TestMethod]
        public void TestFunctionBuilderCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            Func<int, double, double> function = (Func<int, double, double>)engine.Function("var1+2*(3*age)")
                .Parameter("var1", DataType.Integer)
                .Parameter("age", DataType.FloatingPoint)
                .Result(DataType.FloatingPoint)
                .Build();

            double result = function(2, 4);
            Assert.AreEqual(26.0, result);
        }
    }
}
