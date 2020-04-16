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
    public class ConstantRegistryTests
    {
        [TestMethod]
        public void TestAddConstant()
        {
            var registry = new ConstantRegistry<double>(false);
            
            registry.RegisterConstant("test", 42.0);

            var functionInfo = registry.GetConstantInfo("test");
            
            Assert.IsNotNull(functionInfo);
            Assert.AreEqual("test", functionInfo.ConstantName);
            Assert.AreEqual(42.0, functionInfo.Value);
        }

        [TestMethod]
        public void TestOverwritable()
        {
            var registry = new ConstantRegistry<double>(false);

            registry.RegisterConstant("test", 42.0);
            registry.RegisterConstant("test", 26.3);
        }

        [TestMethod]
        public void TestNotOverwritable()
        {
            var registry = new ConstantRegistry<double>(false);

            registry.RegisterConstant("test", 42.0, false);

            AssertExtensions.ThrowsException<Exception>(() =>
                {
                    registry.RegisterConstant("test", 26.3, false);
                });
        }
    }
}
