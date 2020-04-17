using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Jace.Operations;
using Jace.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace Jace.Tests
{
    [TestClass]
    public class DecimalCalculationEngineTests
    {

        ICalculationEngine<T> CreateEngineHelper<T>(CultureInfo culture = null, ExecutionMode execMode = ExecutionMode.Compiled, bool cacheEnabled = true, bool optmizerEnabled = true, bool caseSensitive = false)
        {
            var engine = CalculationEngine.Create<T>(new JaceOptions
            {
                CultureInfo = culture ?? CultureInfo.InvariantCulture,
                ExecutionMode = execMode,
                CacheEnabled = cacheEnabled,
                OptimizerEnabled = optmizerEnabled,
                CaseSensitive = caseSensitive
            });

            return engine;
        }


        [TestMethod]
        public void TestCalculationFormula1FloatingPointCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            decimal result = engine.Calculate("2.0+3.0");

            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestCalculationFormula1IntegersCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            decimal result = engine.Calculate("2+3");

            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestCalculateFormula1()
        {
            var engine = CreateEngineHelper<decimal>();
            decimal result = engine.Calculate("2+3");

            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestCalculateModuloCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, false, false, true);
            decimal result = engine.Calculate("5 % 3.0");

            Assert.AreEqual(2.0m, result);
        }

        [TestMethod]
        public void TestCalculateModuloInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, false, false, true);
            decimal result = engine.Calculate("5 % 3.0");

            Assert.AreEqual(2.0m, result);
        }

        [TestMethod]
        public void TestCalculatePowCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            decimal result = engine.Calculate("2^3.0");

            Assert.AreEqual(8.0m, result);
        }

        [TestMethod]
        public void TestCalculatePowInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            decimal result = engine.Calculate("2^3.0");

            Assert.AreEqual(8.0m, result);
        }

        [TestMethod]
        public void TestCalculateFormulaWithVariables()
        {
            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2.5m);
            variables.Add("var2", 3.4m);

            var engine = CreateEngineHelper<decimal>();
            decimal result = engine.Calculate("var1*var2", variables);

            Assert.AreEqual(8.5m, result);
        }

        [TestMethod]
        public void TestCalculateFormulaWithCaseSensitiveVariables1Compiled()
        {
            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("VaR1", 2.5m);
            variables.Add("vAr2", 3.4m);
            variables.Add("var1", 1);
            variables.Add("var2", 1);

            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, false, false, true);
            decimal result = engine.Calculate("VaR1*vAr2", variables);

            Assert.AreEqual(8.5m, result);
        }

        [TestMethod]
        public void TestCalculateFormulaWithCaseSensitiveVariables1Interpreted()
        {
            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("VaR1", 2.5m);
            variables.Add("vAr2", 3.4m);
            variables.Add("var1", 1);
            variables.Add("var2", 1);

            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, false, false, true);
            decimal result = engine.Calculate("VaR1*vAr2", variables);

            Assert.AreEqual(8.5m, result);
        }

        [TestMethod]
        public void TestCalculateFormulaVariableNotDefinedInterpreted()
        {
            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2.5m);

            AssertExtensions.ThrowsException<VariableNotDefinedException>(() =>
            {
                var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
                decimal result = engine.Calculate("var1*var2", variables);
            });
        }

        [TestMethod]
        public void TestCalculateFormulaVariableNotDefinedCompiled()
        {
            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2.5m);

            AssertExtensions.ThrowsException<VariableNotDefinedException>(() =>
            {
                var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
                decimal result = engine.Calculate("var1*var2", variables);
            });
        }

        [TestMethod]
        public void TestCalculateSineFunctionInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            decimal result = engine.Calculate("sin(14)");

            Assert.AreEqual((decimal)Math.Sin(14.0), result);
        }

        [TestMethod]
        public void TestCalculateSineFunctionCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("sin(14)");

            Assert.AreEqual((decimal)Math.Sin(14.0), result);
        }

        [TestMethod]
        public void TestCalculateCosineFunctionInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            decimal result = engine.Calculate("cos(41)");

            Assert.AreEqual((decimal)Math.Cos(41.0), result);
        }

        [TestMethod]
        public void TestCalculateCosineFunctionCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("cos(41)");

            Assert.AreEqual((decimal)Math.Cos(41.0), result);
        }

        [TestMethod]
        public void TestCalculateLognFunctionInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("logn(14, 3)");

            Assert.AreEqual((decimal)Math.Log(14.0, 3.0), result);
        }

        [TestMethod]
        public void TestCalculateLognFunctionCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("logn(14, 3)");

            Assert.AreEqual((decimal)Math.Log(14.0, 3.0), result);
        }
    }
}