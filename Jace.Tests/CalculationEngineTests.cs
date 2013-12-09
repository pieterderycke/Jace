using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Jace.Operations;
using Jace.Execution;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

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
        public void TestCalculateModuloCompiled()
        {
            CalculationEngine engine = 
                new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, false, false);
            double result = engine.Calculate("5 % 3.0");

            Assert.AreEqual(2.0, result);
        }

        [TestMethod]
        public void TestCalculateModuloInterpreted()
        {
            CalculationEngine engine = 
                new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, false, false);
            double result = engine.Calculate("5 % 3.0");

            Assert.AreEqual(2.0, result);
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
        public void TestCalculateFormulaVariableNotDefinedInterpreted()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2.5);

            AssertExtensions.ThrowsException<VariableNotDefinedException>(() =>
                {
                    CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
                    double result = engine.Calculate("var1*var2", variables);
                });
        }

        [TestMethod]
        public void TestCalculateFormulaVariableNotDefinedCompiled()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2.5);

            AssertExtensions.ThrowsException<VariableNotDefinedException>(() =>
                {
                    CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
                    double result = engine.Calculate("var1*var2", variables);
                });
        }

        [TestMethod]
        public void TestCalculateSineFunctionInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            double result = engine.Calculate("sin(14)");

            Assert.AreEqual(Math.Sin(14.0), result);
        }

        [TestMethod]
        public void TestCalculateSineFunctionCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("sin(14)");

            Assert.AreEqual(Math.Sin(14.0), result);
        }

        [TestMethod]
        public void TestCalculateCosineFunctionInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            double result = engine.Calculate("cos(41)");

            Assert.AreEqual(Math.Cos(41.0), result);
        }

        [TestMethod]
        public void TestCalculateCosineFunctionCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("cos(41)");

            Assert.AreEqual(Math.Cos(41.0), result);
        }

        [TestMethod]
        public void TestCalculateLognFunctionInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("logn(14, 3)");

            Assert.AreEqual(Math.Log(14.0, 3.0), result);
        }

        [TestMethod]
        public void TestCalculateLognFunctionCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("logn(14, 3)");

            Assert.AreEqual(Math.Log(14.0, 3.0), result);
        }

        [TestMethod]
        public void TestNegativeConstant()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("-100");

            Assert.AreEqual(-100.0, result);
        }

        [TestMethod]
        public void TestMultiplicationWithNegativeConstant()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("5*-100");

            Assert.AreEqual(-500.0, result);
        }

        [TestMethod]
        public void TestUnaryMinus1Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("-(1+2+(3+4))");

            Assert.AreEqual(-10.0, result);
        }

        [TestMethod]
        public void TestUnaryMinus1Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("-(1+2+(3+4))");

            Assert.AreEqual(-10.0, result);
        }

        [TestMethod]
        public void TestUnaryMinus2Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("5+(-(1*2))");

            Assert.AreEqual(3.0, result);
        }

        [TestMethod]
        public void TestUnaryMinus2Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("5+(-(1*2))");

            Assert.AreEqual(3.0, result);
        }

        [TestMethod]
        public void TestUnaryMinus3Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("5*(-(1*2)*3)");

            Assert.AreEqual(-30.0, result);
        }

        [TestMethod]
        public void TestUnaryMinus3Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("5*(-(1*2)*3)");

            Assert.AreEqual(-30.0, result);
        }

        [TestMethod]
        public void TestUnaryMinus4Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("5* -(1*2)");

            Assert.AreEqual(-10.0, result);
        }

        [TestMethod]
        public void TestUnaryMinus4Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("5* -(1*2)");

            Assert.AreEqual(-10.0, result);
        }

        [TestMethod]
        public void TestUnaryMinus5Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("-(1*2)^3");

            Assert.AreEqual(-8.0, result);
        }

        [TestMethod]
        public void TestUnaryMinus5Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("-(1*2)^3");

            Assert.AreEqual(-8.0, result);
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
        public void TestFormulaBuilder()
        {
            CalculationEngine engine = new CalculationEngine();
            Func<int, double, double> function = (Func<int, double, double>)engine.Formula("var1+2*(3*age)")
                .Parameter("var1", DataType.Integer)
                .Parameter("age", DataType.FloatingPoint)
                .Result(DataType.FloatingPoint)
                .Build();

            double result = function(2, 4);
            Assert.AreEqual(26.0, result);
        }

        [TestMethod]
        public void TestFormulaBuilderCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            Func<int, double, double> function = (Func<int, double, double>)engine.Formula("var1+2*(3*age)")
                .Parameter("var1", DataType.Integer)
                .Parameter("age", DataType.FloatingPoint)
                .Result(DataType.FloatingPoint)
                .Build();

            double result = function(2, 4);
            Assert.AreEqual(26.0, result);
        }

        [TestMethod]
        public void TestFormulaBuilderInvalidParameterName()
        {
            AssertExtensions.ThrowsException<ArgumentException>(() =>
                {
                    CalculationEngine engine = new CalculationEngine();
                    Func<int, double, double> function = (Func<int, double, double>)engine.Formula("sin+2")
                        .Parameter("sin", DataType.Integer)
                        .Build();
                });
        }

        [TestMethod]
        public void TestFormulaBuilderDuplicateParameterName()
        {
            AssertExtensions.ThrowsException<ArgumentException>(() =>
                {
                    CalculationEngine engine = new CalculationEngine();
                    Func<int, double, double> function = (Func<int, double, double>)engine.Formula("var1+2")
                        .Parameter("var1", DataType.Integer)
                        .Parameter("var1", DataType.FloatingPoint)
                        .Build();
                });
        }

        [TestMethod]
        public void TestPiMultiplication()
        {
            CalculationEngine engine = new CalculationEngine();
            double result = engine.Calculate("2 * pI");

            Assert.AreEqual(2 * Math.PI, result);
        }

        [TestMethod]        
        public void TestReservedVariableName()
        {
            AssertExtensions.ThrowsException<ArgumentException>(() =>
            {
                Dictionary<string, double> variables = new Dictionary<string, double>();
                variables.Add("pi", 2.0);

                CalculationEngine engine = new CalculationEngine();
                double result = engine.Calculate("2 * pI", variables);
            });
        }

        [TestMethod]
        public void TestVariableNameCaseSensitivity()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("blabla", 42.5);

            CalculationEngine engine = new CalculationEngine();
            double result = engine.Calculate("2 * BlAbLa", variables);

            Assert.AreEqual(85.0, result);
        }

        [TestMethod]
        public void TestCustomFunctionInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture,
                ExecutionMode.Interpreted, false, false);
            engine.AddFunction("test", (a, b) => a + b);

            double result = engine.Calculate("test(2,3)");
            Assert.AreEqual(5.0, result);
        }

        [TestMethod]
        public void TestCustomFunctionCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture,
                ExecutionMode.Compiled, false, false);
            engine.AddFunction("test", (a, b) => a + b);

            double result = engine.Calculate("test(2,3)");
            Assert.AreEqual(5.0, result);
        }

        [TestMethod]
        public void TestComplicatedPrecedence1()
        {
            CalculationEngine engine = new CalculationEngine();

            double result = engine.Calculate("1+2-3*4/5+6-7*8/9+0");
            Assert.AreEqual(0.378, Math.Round(result, 3));
        }

        [TestMethod]
        public void TestComplicatedPrecedence2()
        {
            CalculationEngine engine = new CalculationEngine();

            double result = engine.Calculate("1+2-3*4/sqrt(25)+6-7*8/9+0");
            Assert.AreEqual(0.378, Math.Round(result, 3));
        }

        [TestMethod]
        public void TestExpressionArguments()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture);

            double result = engine.Calculate("ifless(0.57, (3000-500)/(1500-500), 10, 20)");
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestNestedFunctions()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture);

            double result = engine.Calculate("max(sin(67), cos(67))");
            Assert.AreEqual(-0.517769799789505, Math.Round(result, 15));
        }

        [TestMethod]
        public void TestVariableCaseFuncInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            Func<Dictionary<string, double>, double> formula = engine.Build("var1+2/(3*otherVariablE)");

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("otherVariable", 4.2);

            double result = formula(variables);
        }

        [TestMethod]
        public void TestVariableCaseFuncCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            Func<Dictionary<string, double>, double> formula = engine.Build("var1+2/(3*otherVariablE)");

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("otherVariable", 4.2);

            double result = formula(variables);
        }

        [TestMethod]
        public void TestVariableCaseNonFunc()
        {
            CalculationEngine engine = new CalculationEngine();

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("otherVariable", 4.2);

            double result = engine.Calculate("var1+2/(3*otherVariablE)", variables);
        }
    }
}
