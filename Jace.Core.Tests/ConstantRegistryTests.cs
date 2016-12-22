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
    public class ConstantRegistryTests
    {
#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestAddConstant()
        {
            ConstantRegistry registry = new ConstantRegistry(false);
            
            registry.RegisterConstant("test", 42.0);

            ConstantInfo functionInfo = registry.GetConstantInfo("test");
#if !NETCORE
            Assert.IsNotNull(functionInfo);
            Assert.AreEqual("test", functionInfo.ConstantName);
            Assert.AreEqual(42.0, functionInfo.Value);
#else
            Assert.NotNull(functionInfo);
            Assert.Equal("test", functionInfo.ConstantName);
            Assert.Equal(42.0, functionInfo.Value);
#endif
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestOverwritable()
        {
            ConstantRegistry registry = new ConstantRegistry(false);

            registry.RegisterConstant("test", 42.0);
            registry.RegisterConstant("test", 26.3);
        }

#if !NETCORE
        [TestMethod]
#else
        [Fact]
#endif
        public void TestNotOverwritable()
        {
            ConstantRegistry registry = new ConstantRegistry(false);

            registry.RegisterConstant("test", 42.0, false);
#if !NETCORE
            AssertExtensions.ThrowsException<Exception>(() =>
                {
                    registry.RegisterConstant("test", 26.3, false);
                });
#else
            Assert.Throws<Exception>(() =>
            {
                registry.RegisterConstant("test", 26.3, false);
            });
#endif
        }
    }
}
