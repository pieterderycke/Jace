using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Execution;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
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
            ConstantRegistry registry = new ConstantRegistry(false);
            
            registry.RegisterConstant("test", 42.0);

            ConstantInfo functionInfo = registry.GetConstantInfo("test");
            
            Assert.IsNotNull(functionInfo);
            Assert.AreEqual("test", functionInfo.ConstantName);
            Assert.AreEqual(42.0, functionInfo.Value);
        }

        [TestMethod]
        public void TestOverwritable()
        {
            ConstantRegistry registry = new ConstantRegistry(false);

            registry.RegisterConstant("test", 42.0);
            registry.RegisterConstant("test", 26.3);
        }

        [TestMethod]
        public void TestNotOverwritable()
        {
            ConstantRegistry registry = new ConstantRegistry(false);

            registry.RegisterConstant("test", 42.0, false);

            AssertExtensions.ThrowsException<Exception>(() =>
                {
                    registry.RegisterConstant("test", 26.3, false);
                });
        }
    }
}
