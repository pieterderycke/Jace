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
            var engine = CalculationEngine.New<T>(new JaceOptions
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

        [TestMethod]
        public void TestNegativeConstant()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("-100");

            Assert.AreEqual(-100.0m, result);
        }

        [TestMethod]
        public void TestMultiplicationWithNegativeConstant()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("5*-100");

            Assert.AreEqual(-500.0m, result);
        }

        [TestMethod]
        public void TestUnaryMinus1Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("-(1+2+(3+4))");

            Assert.AreEqual(-10.0m, result);
        }

        [TestMethod]
        public void TestUnaryMinus1Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("-(1+2+(3+4))");

            Assert.AreEqual(-10.0m, result);
        }

        [TestMethod]
        public void TestUnaryMinus2Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("5+(-(1*2))");

            Assert.AreEqual(3.0m, result);
        }

        [TestMethod]
        public void TestUnaryMinus2Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("5+(-(1*2))");

            Assert.AreEqual(3.0m, result);
        }

        [TestMethod]
        public void TestUnaryMinus3Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("5*(-(1*2)*3)");

            Assert.AreEqual(-30.0m, result);
        }

        [TestMethod]
        public void TestUnaryMinus3Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("5*(-(1*2)*3)");

            Assert.AreEqual(-30.0m, result);
        }

        [TestMethod]
        public void TestUnaryMinus4Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("5* -(1*2)");

            Assert.AreEqual(-10.0m, result);
        }

        [TestMethod]
        public void TestUnaryMinus4Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("5* -(1*2)");

            Assert.AreEqual(-10.0m, result);
        }

        [TestMethod]
        public void TestUnaryMinus5Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("-(1*2)^3");

            Assert.AreEqual(-8.0m, result);
        }

        [TestMethod]
        public void TestUnaryMinus5Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("-(1*2)^3");

            Assert.AreEqual(-8.0m, result);
        }

        [TestMethod]
        public void TestBuild()
        {
            var engine = CreateEngineHelper<decimal>();
            Func<Dictionary<string, decimal>, decimal> function = engine.Build("var1+2*(3*age)");

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("age", 4);

            decimal result = function(variables);
            Assert.AreEqual(26.0m, result);
        }

        [TestMethod]
        public void TestFormulaBuilder()
        {
            var engine = CreateEngineHelper<decimal>();
            Func<int, decimal, decimal> function = (Func<int, decimal, decimal>)engine.Formula("var1+2*(3*age)")
                .Parameter("var1", DataType.Integer)
                .Parameter("age", DataType.FloatingPoint)
                .Result(DataType.FloatingPoint)
                .Build();

            decimal result = function(2, 4);
            Assert.AreEqual(26.0m, result);
        }

        [TestMethod]
        public void TestFormulaBuilderCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            Func<int, decimal, decimal> function = (Func<int, decimal, decimal>)engine.Formula("var1+2*(3*age)")
                .Parameter("var1", DataType.Integer)
                .Parameter("age", DataType.FloatingPoint)
                .Result(DataType.FloatingPoint)
                .Build();

            decimal result = function(2, 4);
            Assert.AreEqual(26.0m, result);
        }

        [TestMethod]
        public void TestFormulaBuilderConstantInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            engine.AddConstant("age", 18.0m);

            Func<int, decimal> function = (Func<int, decimal>)engine.Formula("age+var1")
                .Parameter("var1", DataType.Integer)
                .Result(DataType.FloatingPoint)
                .Build();

            decimal result = function(3);
            Assert.AreEqual(21.0m, result);
        }

        [TestMethod]
        public void TestFormulaBuilderConstantCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            engine.AddConstant("age", 18.0m);

            Func<int, decimal> function = (Func<int, decimal>)engine.Formula("age+var1")
                .Parameter("var1", DataType.Integer)
                .Result(DataType.FloatingPoint)
                .Build();

            decimal result = function(3);
            Assert.AreEqual(21.0m, result);
        }

        [TestMethod]
        public void TestFormulaBuilderInvalidParameterName()
        {
            AssertExtensions.ThrowsException<ArgumentException>(() =>
            {
                var engine = CreateEngineHelper<decimal>();
                Func<int, decimal, decimal> function = (Func<int, decimal, decimal>)engine.Formula("sin+2")
                    .Parameter("sin", DataType.Integer)
                    .Build();
            });
        }

        [TestMethod]
        public void TestFormulaBuilderDuplicateParameterName()
        {
            AssertExtensions.ThrowsException<ArgumentException>(() =>
            {
                var engine = CreateEngineHelper<decimal>();
                Func<int, decimal, decimal> function = (Func<int, decimal, decimal>)engine.Formula("var1+2")
                    .Parameter("var1", DataType.Integer)
                    .Parameter("var1", DataType.FloatingPoint)
                    .Build();
            });
        }

        [TestMethod]
        public void TestPiMultiplication()
        {
            var engine = CreateEngineHelper<decimal>();
            decimal result = engine.Calculate("2 * pI");

            Assert.AreEqual((2 * ((decimal)Math.PI)), result);
        }

        [TestMethod]
        public void TestReservedVariableName()
        {
            AssertExtensions.ThrowsException<ArgumentException>(() =>
            {
                Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
                variables.Add("pi", 2.0m);

                var engine = CreateEngineHelper<decimal>();
                decimal result = engine.Calculate("2 * pI", variables);
            });
        }

        [TestMethod]
        public void TestVariableNameCaseSensitivityCompiled()
        {
            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("blabla", 42.5m);

            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            decimal result = engine.Calculate("2 * BlAbLa", variables);

            Assert.AreEqual(85.0m, result);
        }

        [TestMethod]
        public void TestVariableNameCaseSensitivityInterpreted()
        {
            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("blabla", 42.5m);

            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            decimal result = engine.Calculate("2 * BlAbLa", variables);

            Assert.AreEqual(85.0m, result);
        }

        [TestMethod]
        public void TestVariableNameCaseSensitivityNoToLowerCompiled()
        {
            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("BlAbLa", 42.5m);

            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, false, false, false);
            decimal result = engine.Calculate("2 * BlAbLa", variables);

            Assert.AreEqual(85.0m, result);
        }

        [TestMethod]
        public void TestVariableNameCaseSensitivityNoToLowerInterpreted()
        {
            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("BlAbLa", 42.5m);

            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, false, false, false);
            decimal result = engine.Calculate("2 * BlAbLa", variables);

            Assert.AreEqual(85.0m, result);
        }

        [TestMethod]
        public void TestCustomFunctionInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture,
                ExecutionMode.Interpreted, false, false, false);
            engine.AddFunction("test", (a, b) => a + b);

            decimal result = engine.Calculate("test(2,3)");
            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestCustomFunctionCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture,
                ExecutionMode.Compiled, false, false, false);
            engine.AddFunction("test", (a, b) => a + b);

            decimal result = engine.Calculate("test(2,3)");
            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestComplicatedPrecedence1()
        {
            var engine = CreateEngineHelper<decimal>();

            decimal result = engine.Calculate("1+2-3*4/5+6-7*8/9+0");
            Assert.AreEqual(0.378m, Math.Round(result, 3));
        }

        [TestMethod]
        public void TestComplicatedPrecedence2()
        {
            var engine = CreateEngineHelper<decimal>();

            decimal result = engine.Calculate("1+2-3*4/sqrt(25)+6-7*8/9+0");
            Assert.AreEqual(0.378m, Math.Round(result, 3));
        }

        [TestMethod]
        public void TestExpressionArguments1()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture);

            decimal result = engine.Calculate("ifless(0.57, (3000-500)/(1500-500), 10, 20)");
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestExpressionArguments2()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture);

            decimal result = engine.Calculate("if(0.57 < (3000-500)/(1500-500), 10, 20)");
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestNestedFunctions()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture);

            decimal result = engine.Calculate("max(sin(67), cos(67))");
            Assert.AreEqual(-0.517769799789505m, Math.Round(result, 15));
        }

        [TestMethod]
        public void TestVariableCaseFuncInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            Func<Dictionary<string, decimal>, decimal> formula = engine.Build("var1+2/(3*otherVariablE)");

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("otherVariable", 4.2m);

            decimal result = formula(variables);
        }

        [TestMethod]
        public void TestVariableCaseFuncCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            Func<Dictionary<string, decimal>, decimal> formula = engine.Build("var1+2/(3*otherVariablE)");

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("otherVariable", 4.2m);

            decimal result = formula(variables);
        }

        [TestMethod]
        public void TestVariableCaseNonFunc()
        {
            var engine = CreateEngineHelper<decimal>();

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("otherVariable", 4.2m);

            decimal result = engine.Calculate("var1+2/(3*otherVariablE)", variables);
        }

        [TestMethod]
        public void TestLessThanInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 4.2m);

            decimal result = engine.Calculate("var1 < var2", variables);
            Assert.AreEqual(1.0m, result);
        }

        [TestMethod]
        public void TestLessThanCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 4.2m);

            decimal result = engine.Calculate("var1 < var2", variables);
            Assert.AreEqual(1.0m, result);
        }

        [TestMethod]
        public void TestLessOrEqualThan1Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            decimal result = engine.Calculate("var1 <= var2", variables);
            Assert.AreEqual(1.0m, result);
        }

        [TestMethod]
        public void TestLessOrEqualThan1Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            decimal result = engine.Calculate("var1 <= var2", variables);
            Assert.AreEqual(1.0m, result);
        }

        [TestMethod]
        public void TestLessOrEqualThan2Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            decimal result = engine.Calculate("var1 ≤ var2", variables);
            Assert.AreEqual(1.0m, result);
        }

        [TestMethod]
        public void TestLessOrEqualThan2Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            decimal result = engine.Calculate("var1 ≤ var2", variables);
            Assert.AreEqual(1.0m, result);
        }

        [TestMethod]
        public void TestGreaterThan1Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 3);

            decimal result = engine.Calculate("var1 > var2", variables);
            Assert.AreEqual(0.0m, result);
        }

        [TestMethod]
        public void TestGreaterThan1Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 3);

            decimal result = engine.Calculate("var1 > var2", variables);
            Assert.AreEqual(0.0m, result);
        }

        [TestMethod]
        public void TestGreaterOrEqualThan1Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            decimal result = engine.Calculate("var1 >= var2", variables);
            Assert.AreEqual(1.0m, result);
        }

        [TestMethod]
        public void TestGreaterOrEqualThan1Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            decimal result = engine.Calculate("var1 >= var2", variables);
            Assert.AreEqual(1.0m, result);
        }

        [TestMethod]
        public void TestNotEqual1Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            decimal result = engine.Calculate("var1 != 2", variables);
            Assert.AreEqual(0.0m, result);
        }

        [TestMethod]
        public void TestNotEqual2Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            decimal result = engine.Calculate("var1 ≠ 2", variables);
            Assert.AreEqual(0.0m, result);
        }

        [TestMethod]
        public void TestNotEqual2Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            decimal result = engine.Calculate("var1 ≠ 2", variables);
            Assert.AreEqual(0.0m, result);
        }

        [TestMethod]
        public void TestEqualInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            decimal result = engine.Calculate("var1 == 2", variables);
            Assert.AreEqual(1.0m, result);
        }

        [TestMethod]
        public void TestEqualCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            decimal result = engine.Calculate("var1 == 2", variables);
            Assert.AreEqual(1.0m, result);
        }

        public void TestVariableUnderscoreInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var_var_1", 1);
            variables.Add("var_var_2", 2);

            decimal result = engine.Calculate("var_var_1 + var_var_2", variables);
            Assert.AreEqual(3.0m, result);
        }

        [TestMethod]
        public void TestVariableUnderscoreCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
            variables.Add("var_var_1", 1);
            variables.Add("var_var_2", 2);

            decimal result = engine.Calculate("var_var_1 + var_var_2", variables);
            Assert.AreEqual(3.0m, result);
        }

        [TestMethod]
        public void TestCustomFunctionFunc11Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture,
                ExecutionMode.Interpreted, false, false, false);
            engine.AddFunction("test", (a, b, c, d, e, f, g, h, i, j, k) => a + b + c + d + e + f + g + h + i + j + k);
            decimal result = engine.Calculate("test(1,2,3,4,5,6,7,8,9,10,11)");
            decimal expected = (11m * (11m + 1m)) / 2.0m;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCustomFunctionFunc11Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture,
                ExecutionMode.Compiled, false, false, false);
            engine.AddFunction("test", (a, b, c, d, e, f, g, h, i, j, k) => a + b + c + d + e + f + g + h + i + j + k);
            decimal result = engine.Calculate("test(1,2,3,4,5,6,7,8,9,10,11)");
            decimal expected = (11m * (11m + 1m)) / 2.0m;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCustomFunctionDynamicFuncInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture,
                ExecutionMode.Interpreted, false, false, false);

            decimal DoSomething(params decimal[] a)
            {
                return a.Sum();
            }

            engine.AddFunction("test", DoSomething);
            decimal result = engine.Calculate("test(1,2,3,4,5,6,7,8,9,10,11)");
            decimal expected = (11m * (11m + 1m)) / 2.0m;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCustomFunctionDynamicFuncCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture,
                ExecutionMode.Compiled, false, false, false);

            decimal DoSomething(params decimal[] a)
            {
                return a.Sum();
            }

            engine.AddFunction("test", DoSomething);
            decimal result = engine.Calculate("test(1,2,3,4,5,6,7,8,9,10,11)");
            decimal expected = (11m * (11m + 1m)) / 2.0m;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCustomFunctionDynamicFuncNestedInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture,
                ExecutionMode.Interpreted, false, false, false);

            decimal DoSomething(params decimal[] a)
            {
                return a.Sum();
            }

            engine.AddFunction("test", DoSomething);
            decimal result = engine.Calculate("test(1,2,3,test(4,5,6)) + test(7,8,9,10,11)");
            decimal expected = (11m * (11m + 1m)) / 2.0m;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCustomFunctionDynamicFuncNestedDynamicCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture,
                ExecutionMode.Compiled, false, false, false);

            decimal DoSomething(params decimal[] a)
            {
                return a.Sum();
            }

            engine.AddFunction("test", DoSomething);
            decimal result = engine.Calculate("test(1,2,3,test(4,5,6)) + test(7,8,9,10,11)");
            decimal expected = (11m * (11m + 1m)) / 2.0m;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAndCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("(1 && 0)");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestAndInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("(1 && 0)");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestOr1Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("(1 || 0)");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestOr1Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("(1 || 0)");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestOr2Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("(0 || 0)");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestOr2Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("(0 || 0)");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestMedian1Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("median(3,1,5,4,2)");
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TestMedian1Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("median(3,1,5,4,2)");
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TestMedian2Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("median(3,1,5,4)");
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TestMedian2Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("median(3,1,5,4)");
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstants1Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            var fn = engine.Build("a+b+c", new Dictionary<string, decimal> { { "a", 1 } });
            decimal result = fn(new Dictionary<string, decimal> { { "b", 2 }, { "c", 2 } });
            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstants1Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            var fn = engine.Build("a+b+c", new Dictionary<string, decimal> { { "a", 1 } });
            decimal result = fn(new Dictionary<string, decimal> { { "b", 2 }, { "c", 2 } });
            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstants2Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Func<decimal, decimal, decimal> formula = (Func<decimal, decimal, decimal>)engine.Formula("a+b+c")
                .Parameter("b", DataType.FloatingPoint)
                .Parameter("c", DataType.FloatingPoint)
                .Constant("a", 1)
                .Result(DataType.FloatingPoint)
                .Build();

            decimal result = formula(2.0m, 2.0m);
            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstants2Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Func<decimal, decimal, decimal> formula = (Func<decimal, decimal, decimal>)engine.Formula("a+b+c")
                .Parameter("b", DataType.FloatingPoint)
                .Parameter("c", DataType.FloatingPoint)
                .Constant("a", 1)
                .Result(DataType.FloatingPoint)
                .Build();

            decimal result = formula(2.0m, 2.0m);
            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstants3Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, true, true);

            Func<decimal, decimal> formula = (Func<decimal, decimal>)engine.Formula("a+A")
                .Parameter("A", DataType.FloatingPoint)
                .Constant("a", 1)
                .Result(DataType.FloatingPoint)
                .Build();

            decimal result = formula(2.0m);
            Assert.AreEqual(3.0m, result);
        }

        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstants3Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, true, true);

            Func<decimal, decimal> formula = (Func<decimal, decimal>)engine.Formula("a+A")
                .Parameter("A", DataType.FloatingPoint)
                .Constant("a", 1)
                .Result(DataType.FloatingPoint)
                .Build();

            decimal result = formula(2.0m);
            Assert.AreEqual(3.0m, result);
        }

        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstantsCache1()
        {
            var engine = CalculationEngine.New<decimal>(new JaceOptions { CacheEnabled = true });

            var fn = engine.Build("a+b+c", new Dictionary<string, decimal> { { "a", 1 } });
            decimal result = fn(new Dictionary<string, decimal> { { "b", 2 }, { "c", 2 } });
            Assert.AreEqual(5.0m, result);

            AssertExtensions.ThrowsException<VariableNotDefinedException>(() =>
            {
                var fn1 = engine.Build("a+b+c");
                decimal result1 = fn1(new Dictionary<string, decimal> { { "b", 3 }, { "c", 3 } });
            });
        }

        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstantsCache2()
        {
            var engine = CalculationEngine.New<decimal>(new JaceOptions { CacheEnabled = true });
            var fn = engine.Build("a+b+c");
            decimal result = fn(new Dictionary<string, decimal> { { "a", 1 }, { "b", 2 }, { "c", 2 } });
            Assert.AreEqual(5.0m, result);


            var fn1 = engine.Build("a+b+c", new Dictionary<string, decimal> { { "a", 2 } });
            decimal result1 = fn1(new Dictionary<string, decimal> { { "b", 2 }, { "c", 2 } });
            Assert.AreEqual(6.0m, result1);
        }


        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstantsCache3()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, true, true);

            Func<decimal, decimal> formula = (Func<decimal, decimal>)engine.Formula("a+A")
                .Parameter("A", DataType.FloatingPoint)
                .Constant("a", 1)
                .Result(DataType.FloatingPoint)
                .Build();

            decimal result = formula(2.0m);
            Assert.AreEqual(3.0m, result);

            Func<decimal, decimal, decimal> formula1 = (Func<decimal, decimal, decimal>)engine.Formula("a+A")
            .Parameter("A", DataType.FloatingPoint)
            .Parameter("a", DataType.FloatingPoint)
            .Result(DataType.FloatingPoint)
            .Build();

            decimal result1 = formula1(2.0m, 2.0m);
            Assert.AreEqual(4.0m, result1);
        }


        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstantsCache4()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, true, true);

            Func<decimal, decimal> formula = (Func<decimal, decimal>)engine.Formula("a+A")
                .Parameter("A", DataType.FloatingPoint)
                .Constant("a", 1)
                .Result(DataType.FloatingPoint)
                .Build();

            decimal result = formula(2.0m);
            Assert.AreEqual(3.0m, result);

            Func<decimal, decimal, decimal> formula1 = (Func<decimal, decimal, decimal>)engine.Formula("a+A")
                .Parameter("A", DataType.FloatingPoint)
                .Parameter("a", DataType.FloatingPoint)
                .Result(DataType.FloatingPoint)
                .Build();

            decimal result1 = formula1(2.0m, 2.0m);
            Assert.AreEqual(4.0m, result1);
        }

        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstantsCache5()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            var fn = engine.Build("a+b+c", new Dictionary<string, decimal> { { "a", 1 } });
            decimal result = fn(new Dictionary<string, decimal> { { "b", 2 }, { "c", 2 } });
            Assert.AreEqual(5.0m, result);

            var fn1 = engine.Build("a+b+c", new Dictionary<string, decimal> { { "a", 2 } });
            decimal result1 = fn1(new Dictionary<string, decimal> { { "b", 2 }, { "c", 2 } });
            Assert.AreEqual(6.0m, result1);
        }

        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstantsCache6()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            var fn = engine.Build("a+b+c", new Dictionary<string, decimal> { { "a", 1 } });
            decimal result = fn(new Dictionary<string, decimal> { { "b", 2 }, { "c", 2 } });
            Assert.AreEqual(5.0m, result);

            var fn1 = engine.Build("a+b+c", new Dictionary<string, decimal> { { "a", 2 } });
            decimal result1 = fn1(new Dictionary<string, decimal> { { "b", 2 }, { "c", 2 } });
            Assert.AreEqual(6.0m, result1);
        }

        [TestMethod]
        public void TestCalculationFormulaBuildingWithConstantsCache7()
        {
            var engine = CalculationEngine.New<decimal>(new JaceOptions { CacheEnabled = true });

            var fn = engine.Build("a+b+c");
            decimal result = fn(new Dictionary<string, decimal> { { "a", 1 }, { "b", 2 }, { "c", 2 } });
            Assert.AreEqual(5.0m, result);


            var fn1 = engine.Build("a+b+c", new Dictionary<string, decimal> { { "a", 2 } });
            decimal result1 = fn1(new Dictionary<string, decimal> { { "b", 3 }, { "c", 3 } });
            Assert.AreEqual(8.0m, result1);
        }

        [TestMethod]
        public void TestCalculationCompiledExpression1Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, true, false);

            Expression<Func<decimal, decimal, decimal>> expression = (a, b) => a + b;
            expression.Compile();

            engine.AddFunction("test", expression.Compile());

            decimal result = engine.Calculate("test(2, 3)");
            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestCalculationCompiledExpression1Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, true, false);

            Expression<Func<decimal, decimal, decimal>> expression = (a, b) => a + b;
            expression.Compile();

            engine.AddFunction("test", expression.Compile());

            decimal result = engine.Calculate("test(2, 3)");
            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestCalculationCompiledExpression2Compiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, true, false);

            Expression<Func<decimal>> expression = () => 5;
            expression.Compile();

            engine.AddFunction("test", expression.Compile());

            decimal result = engine.Calculate("test()");
            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestCalculationCompiledExpression2Interpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, true, false);

            Expression<Func<decimal>> expression = () => 5;
            expression.Compile();

            engine.AddFunction("test", expression.Compile());

            decimal result = engine.Calculate("test()");
            Assert.AreEqual(5.0m, result);
        }

        [TestMethod]
        public void TestPowInterpreted()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
            decimal result = engine.Calculate("2^2");

            Assert.AreEqual(4m, result);
        }


        [TestMethod]
        public void TestPowCompiled()
        {
            var engine = CreateEngineHelper<decimal>(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
            decimal result = engine.Calculate("2^2");

            Assert.AreEqual(4m, result);
        }
    }
}