using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Jace.Operations;
using Jace.Execution;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __ANDROID__
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif NETCORE
using Xunit;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Jace.Tests
{
#if !NETCORE
    [TestClass]
#endif
    public class CalculationEngineTests
    {
#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculationFormula1FloatingPointCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            double result = engine.Calculate("2.0+3.0");
#if !NETCORE
            Assert.AreEqual(5.0, result);
#else
            Assert.Equal(5.0, result);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculationFormula1IntegersCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            double result = engine.Calculate("2+3");
#if !NETCORE
           Assert.AreEqual(5.0, result);
#else
            Assert.Equal(5.0, result);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateFormula1()
        {
            CalculationEngine engine = new CalculationEngine();
            double result = engine.Calculate("2+3");
#if !NETCORE
            Assert.AreEqual(5.0, result);
#else
            Assert.Equal(5.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateModuloCompiled()
        {
            CalculationEngine engine = 
                new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, false, false);
            double result = engine.Calculate("5 % 3.0");
#if !NETCORE
            Assert.AreEqual(2.0, result);
#else
            Assert.Equal(2.0, result);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateModuloInterpreted()
        {
            CalculationEngine engine = 
                new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, false, false);
            double result = engine.Calculate("5 % 3.0");
#if !NETCORE
            Assert.AreEqual(2.0, result);
#else
            Assert.Equal(2.0, result);
#endif
           
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculatePowCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            double result = engine.Calculate("2^3.0");
#if !NETCORE
            Assert.AreEqual(8.0, result);
#else
            Assert.Equal(8.0, result);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculatePowInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            double result = engine.Calculate("2^3.0");
#if !NETCORE
             Assert.AreEqual(8.0, result);
#else
            Assert.Equal(8.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateFormulaWithVariables()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2.5);
            variables.Add("var2", 3.4);

            CalculationEngine engine = new CalculationEngine();
            double result = engine.Calculate("var1*var2", variables);
#if !NETCORE
             Assert.AreEqual(8.5, result);
#else
            Assert.Equal(8.5, result);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateFormulaVariableNotDefinedInterpreted()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2.5);
#if !NETCORE
            AssertExtensions.ThrowsException<VariableNotDefinedException>(() =>
                {
                    CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
                    double result = engine.Calculate("var1*var2", variables);
                });
#else
            Assert.Throws<VariableNotDefinedException>(() =>
            {
                CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
                double result = engine.Calculate("var1*var2", variables);
            });
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateFormulaVariableNotDefinedCompiled()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2.5);
#if !NETCORE
            AssertExtensions.ThrowsException<VariableNotDefinedException>(() =>
                {
                    CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
                    double result = engine.Calculate("var1*var2", variables);
                });
#else
            Assert.Throws<VariableNotDefinedException>(() =>
            {
                CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
                double result = engine.Calculate("var1*var2", variables);
            });
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateSineFunctionInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            double result = engine.Calculate("sin(14)");
#if !NETCORE
            Assert.AreEqual(Math.Sin(14.0), result);
#else
            Assert.Equal(Math.Sin(14.0), result);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateSineFunctionCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("sin(14)");

#if !NETCORE
            Assert.AreEqual(Math.Sin(14.0), result);
#else
            Assert.Equal(Math.Sin(14.0), result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateCosineFunctionInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            double result = engine.Calculate("cos(41)");

#if !NETCORE
            Assert.AreEqual(Math.Cos(41.0), result);
#else
            Assert.Equal(Math.Cos(41.0), result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateCosineFunctionCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("cos(41)");

#if !NETCORE
            Assert.AreEqual(Math.Cos(41.0), result);
#else
            Assert.Equal(Math.Cos(41.0), result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateLognFunctionInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("logn(14, 3)");

#if !NETCORE
            Assert.AreEqual(Math.Log(14.0, 3.0), result);
#else
            Assert.Equal(Math.Log(14.0, 3.0), result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCalculateLognFunctionCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("logn(14, 3)");

#if !NETCORE
            Assert.AreEqual(Math.Log(14.0, 3.0), result);
#else
            Assert.Equal(Math.Log(14.0, 3.0), result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestNegativeConstant()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("-100");

#if !NETCORE
             Assert.AreEqual(-100.0, result);
#else
            Assert.Equal(-100.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestMultiplicationWithNegativeConstant()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("5*-100");


#if !NETCORE
            Assert.AreEqual(-500.0, result);
#else
            Assert.Equal(-500.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestUnaryMinus1Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("-(1+2+(3+4))");

#if !NETCORE
            Assert.AreEqual(-10.0, result);
#else
            Assert.Equal(-10.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestUnaryMinus1Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("-(1+2+(3+4))");

#if !NETCORE
            Assert.AreEqual(-10.0, result);
#else
            Assert.Equal(-10.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestUnaryMinus2Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("5+(-(1*2))");

#if !NETCORE
             Assert.AreEqual(3.0, result);
#else
            Assert.Equal(3.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestUnaryMinus2Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("5+(-(1*2))");


#if !NETCORE
             Assert.AreEqual(3.0, result);
#else
            Assert.Equal(3.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestUnaryMinus3Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("5*(-(1*2)*3)");


#if !NETCORE
             Assert.AreEqual(-30.0, result);
#else
            Assert.Equal(-30.0, result);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestUnaryMinus3Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("5*(-(1*2)*3)");

#if !NETCORE
             Assert.AreEqual(-30.0, result);
#else
            Assert.Equal(-30.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestUnaryMinus4Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("5* -(1*2)");

#if !NETCORE
            Assert.AreEqual(-10.0, result);
#else
            Assert.Equal(-10.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestUnaryMinus4Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("5* -(1*2)");

#if !NETCORE
            Assert.AreEqual(-10.0, result);
#else
            Assert.Equal(-10.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestUnaryMinus5Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false);
            double result = engine.Calculate("-(1*2)^3");

#if !NETCORE
            Assert.AreEqual(-8.0, result);
#else
            Assert.Equal(-8.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestUnaryMinus5Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false);
            double result = engine.Calculate("-(1*2)^3");


#if !NETCORE
             Assert.AreEqual(-8.0, result);
#else
            Assert.Equal(-8.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestBuild()
        { 
            CalculationEngine engine = new CalculationEngine();
            Func<Dictionary<string, double>, double> function = engine.Build("var1+2*(3*age)");

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("age", 4);

            double result = function(variables);

#if !NETCORE
             Assert.AreEqual(26.0, result);
#else
            Assert.Equal(26.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestFormulaBuilder()
        {
            CalculationEngine engine = new CalculationEngine();
            Func<int, double, double> function = (Func<int, double, double>)engine.Formula("var1+2*(3*age)")
                .Parameter("var1", DataType.Integer)
                .Parameter("age", DataType.FloatingPoint)
                .Result(DataType.FloatingPoint)
                .Build();

            double result = function(2, 4);
#if !NETCORE
            Assert.AreEqual(26.0, result);
#else
            Assert.Equal(26.0, result);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestFormulaBuilderCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            Func<int, double, double> function = (Func<int, double, double>)engine.Formula("var1+2*(3*age)")
                .Parameter("var1", DataType.Integer)
                .Parameter("age", DataType.FloatingPoint)
                .Result(DataType.FloatingPoint)
                .Build();

            double result = function(2, 4);
#if !NETCORE
             Assert.AreEqual(26.0, result);
#else
            Assert.Equal(26.0, result);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestFormulaBuilderInvalidParameterName()
        {
#if !NETCORE
            AssertExtensions.ThrowsException<ArgumentException>(() =>
                {
                    CalculationEngine engine = new CalculationEngine();
                    Func<int, double, double> function = (Func<int, double, double>)engine.Formula("sin+2")
                        .Parameter("sin", DataType.Integer)
                        .Build();
                });
#else
            Assert.Throws<ArgumentException>(() =>
            {
                CalculationEngine engine = new CalculationEngine();
                Func<int, double, double> function = (Func<int, double, double>)engine.Formula("sin+2")
                    .Parameter("sin", DataType.Integer)
                    .Build();
            });
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestFormulaBuilderDuplicateParameterName()
        {
#if !NETCORE
            AssertExtensions.ThrowsException<ArgumentException>(() =>
                {
                    CalculationEngine engine = new CalculationEngine();
                    Func<int, double, double> function = (Func<int, double, double>)engine.Formula("var1+2")
                        .Parameter("var1", DataType.Integer)
                        .Parameter("var1", DataType.FloatingPoint)
                        .Build();
                });
#else
            Assert.Throws<ArgumentException>(() =>
            {
                CalculationEngine engine = new CalculationEngine();
                Func<int, double, double> function = (Func<int, double, double>)engine.Formula("var1+2")
                    .Parameter("var1", DataType.Integer)
                    .Parameter("var1", DataType.FloatingPoint)
                    .Build();
            });
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestPiMultiplication()
        {
            CalculationEngine engine = new CalculationEngine();
            double result = engine.Calculate("2 * pI");


#if !NETCORE
             Assert.AreEqual(2 * Math.PI, result);
#else
            Assert.Equal(2 * Math.PI, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestReservedVariableName()
        {
#if !NETCORE
            AssertExtensions.ThrowsException<ArgumentException>(() =>
            {
                Dictionary<string, double> variables = new Dictionary<string, double>();
                variables.Add("pi", 2.0);

                CalculationEngine engine = new CalculationEngine();
                double result = engine.Calculate("2 * pI", variables);
            });
#else
            Assert.Throws<ArgumentException>(() =>
            {
                Dictionary<string, double> variables = new Dictionary<string, double>();
                variables.Add("pi", 2.0);

                CalculationEngine engine = new CalculationEngine();
                double result = engine.Calculate("2 * pI", variables);
            });
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestVariableNameCaseSensitivity()
        {
            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("blabla", 42.5);

            CalculationEngine engine = new CalculationEngine();
            double result = engine.Calculate("2 * BlAbLa", variables);


#if !NETCORE
            Assert.AreEqual(85.0, result);
#else
            Assert.Equal(85.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCustomFunctionInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture,
                ExecutionMode.Interpreted, false, false);
            engine.AddFunction("test", (a, b) => a + b);

            double result = engine.Calculate("test(2,3)");

#if !NETCORE
            Assert.AreEqual(5.0, result);
#else
            Assert.Equal(5.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestCustomFunctionCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture,
                ExecutionMode.Compiled, false, false);
            engine.AddFunction("test", (a, b) => a + b);

            double result = engine.Calculate("test(2,3)");

#if !NETCORE
            Assert.AreEqual(5.0, result);
#else
            Assert.Equal(5.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestComplicatedPrecedence1()
        {
            CalculationEngine engine = new CalculationEngine();

            double result = engine.Calculate("1+2-3*4/5+6-7*8/9+0");

#if !NETCORE
             Assert.AreEqual(0.378, Math.Round(result, 3));
#else
            Assert.Equal(0.378, Math.Round(result, 3));
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestComplicatedPrecedence2()
        {
            CalculationEngine engine = new CalculationEngine();

            double result = engine.Calculate("1+2-3*4/sqrt(25)+6-7*8/9+0");

#if !NETCORE
            Assert.AreEqual(0.378, Math.Round(result, 3));
#else
            Assert.Equal(0.378, Math.Round(result, 3));
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestExpressionArguments1()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture);

            double result = engine.Calculate("ifless(0.57, (3000-500)/(1500-500), 10, 20)");

#if !NETCORE
             Assert.AreEqual(10, result);
#else
            Assert.Equal(10, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestExpressionArguments2()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture);

            double result = engine.Calculate("if(0.57 < (3000-500)/(1500-500), 10, 20)");

#if !NETCORE
            Assert.AreEqual(10, result);
#else
            Assert.Equal(10, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestNestedFunctions()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture);

            double result = engine.Calculate("max(sin(67), cos(67))");
#if !NETCORE
            Assert.AreEqual(-0.517769799789505, Math.Round(result, 15));
#else
            Assert.Equal(-0.517769799789505, Math.Round(result, 15));
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestVariableCaseFuncInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
            Func<Dictionary<string, double>, double> formula = engine.Build("var1+2/(3*otherVariablE)");

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("otherVariable", 4.2);

            double result = formula(variables);
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestVariableCaseFuncCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
            Func<Dictionary<string, double>, double> formula = engine.Build("var1+2/(3*otherVariablE)");

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("otherVariable", 4.2);

            double result = formula(variables);
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestVariableCaseNonFunc()
        {
            CalculationEngine engine = new CalculationEngine();

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("otherVariable", 4.2);

            double result = engine.Calculate("var1+2/(3*otherVariablE)", variables);
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestLessThanInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 4.2);

            double result = engine.Calculate("var1 < var2", variables);

#if !NETCORE
            Assert.AreEqual(1.0, result);
#else
            Assert.Equal(1.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestLessThanCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 4.2);

            double result = engine.Calculate("var1 < var2", variables);

#if !NETCORE
             Assert.AreEqual(1.0, result);
#else
            Assert.Equal(1.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestLessOrEqualThan1Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            double result = engine.Calculate("var1 <= var2", variables);

#if !NETCORE
            Assert.AreEqual(1.0, result);
#else
            Assert.Equal(1.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestLessOrEqualThan1Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            double result = engine.Calculate("var1 <= var2", variables);

#if !NETCORE
             Assert.AreEqual(1.0, result);
#else
            Assert.Equal(1.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestLessOrEqualThan2Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            double result = engine.Calculate("var1 ≤ var2", variables);

#if !NETCORE
            Assert.AreEqual(1.0, result);
#else
            Assert.Equal(1.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestLessOrEqualThan2Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            double result = engine.Calculate("var1 ≤ var2", variables);

#if !NETCORE
             Assert.AreEqual(1.0, result);
#else
            Assert.Equal(1.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestGreaterThan1Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 3);

            double result = engine.Calculate("var1 > var2", variables);

#if !NETCORE
            Assert.AreEqual(0.0, result);
#else
            Assert.Equal(0.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestGreaterThan1Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 3);

            double result = engine.Calculate("var1 > var2", variables);

#if !NETCORE
            Assert.AreEqual(0.0, result);
#else
            Assert.Equal(0.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestGreaterOrEqualThan1Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            double result = engine.Calculate("var1 >= var2", variables);
#if !NETCORE
            Assert.AreEqual(1.0, result);
#else
            Assert.Equal(1.0, result);
#endif


        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestGreaterOrEqualThan1Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            double result = engine.Calculate("var1 >= var2", variables);
#if !NETCORE
            Assert.AreEqual(1.0, result);
#else
            Assert.Equal(1.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestNotEqual1Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            double result = engine.Calculate("var1 != 2", variables);
#if !NETCORE
            Assert.AreEqual(0.0, result);
#else
            Assert.Equal(0.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestNotEqual2Interpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            double result = engine.Calculate("var1 ≠ 2", variables);
#if !NETCORE
            Assert.AreEqual(0.0, result);
#else
            Assert.Equal(0.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestNotEqual2Compiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            double result = engine.Calculate("var1 ≠ 2", variables);
#if !NETCORE
            Assert.AreEqual(0.0, result);
#else
            Assert.Equal(0.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestEqualInterpreted()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            double result = engine.Calculate("var1 == 2", variables);
#if !NETCORE
            Assert.AreEqual(1.0, result);
#else
            Assert.Equal(1.0, result);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestEqualCompiled()
        {
            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

            Dictionary<string, double> variables = new Dictionary<string, double>();
            variables.Add("var1", 2);
            variables.Add("var2", 2);

            double result = engine.Calculate("var1 == 2", variables);
#if !NETCORE
            Assert.AreEqual(1.0, result);
#else
            Assert.Equal(1.0, result);
#endif
        }
    }
}
