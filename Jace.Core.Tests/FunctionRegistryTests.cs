using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class FunctionRegistryTests
    {
#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestAddFunc2()
        {
            FunctionRegistry registry = new FunctionRegistry(false);
            
            Func<double, double, double> testFunction = (a, b) => a * b;
            registry.RegisterFunction("test", testFunction);

            FunctionInfo functionInfo = registry.GetFunctionInfo("test");
#if !NETCORE
            Assert.IsNotNull(functionInfo);
            Assert.AreEqual("test", functionInfo.FunctionName);
            Assert.AreEqual(2, functionInfo.NumberOfParameters);
            Assert.AreEqual(testFunction, functionInfo.Function);
#else
            Assert.NotNull(functionInfo);
            Assert.Equal("test", functionInfo.FunctionName);
            Assert.Equal(2, functionInfo.NumberOfParameters);
            Assert.Equal(testFunction, functionInfo.Function);
#endif

        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestOverwritable()
        {
            FunctionRegistry registry = new FunctionRegistry(false);

            Func<double, double, double> testFunction1 = (a, b) => a * b;
            Func<double, double, double> testFunction2 = (a, b) => a * b;
            registry.RegisterFunction("test", testFunction1);
            registry.RegisterFunction("test", testFunction2);
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestNotOverwritable()
        {
            FunctionRegistry registry = new FunctionRegistry(false);

            Func<double, double, double> testFunction1 = (a, b) => a * b;
            Func<double, double, double> testFunction2 = (a, b) => a * b;

            registry.RegisterFunction("test", testFunction1, false);
#if !NETCORE
            AssertExtensions.ThrowsException<Exception>(() =>
                {
                    registry.RegisterFunction("test", testFunction2, false);
                });
#else
            Assert.Throws<Exception>(() =>
            {
                registry.RegisterFunction("test", testFunction2, false);
            });
#endif
        }
    }
}
