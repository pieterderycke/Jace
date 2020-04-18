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
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Jace.Tests
{
    [TestClass]
    public class FactoryTests
    {
        [TestMethod]
        public void TestDouble()
        {
            var engine = CalculationEngine.New<double>();
            Assert.IsInstanceOfType(engine, typeof(ICalculationEngine<double>));              
        }

        [TestMethod]
        public void TestDecimal()
        {
            var engine = CalculationEngine.New<decimal>();
            Assert.IsInstanceOfType(engine, typeof(ICalculationEngine<decimal>));
        }

        [TestMethod]
        public void TestNotDefined()
        {         
            AssertExtensions.ThrowsException<ArgumentException>(() =>
                {
                    var engine = CalculationEngine.New<int>();
                });
        }
    }
}
