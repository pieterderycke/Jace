using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Execution;
using Jace.Operations;
using Jace.Util;

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
    public class FuncAdapterTests
    {
        [TestMethod]
        public void TestFuncAdapterWrap()
        {
            FuncAdapter adapter = new FuncAdapter();

            List<ParameterInfo> parameters = new List<ParameterInfo>() { 
                new ParameterInfo() { Name = "test1", DataType = DataType.Integer },
                new ParameterInfo() { Name = "test2", DataType = DataType.FloatingPoint }
            };

            Func<Dictionary<string, double>, double> function = (dictionary) => dictionary["test1"] + dictionary["test2"]; 

            Func<int, double, double> wrappedFunction = (Func<int, double, double>)adapter.Wrap(parameters, function);

            Assert.AreEqual(3.0, wrappedFunction(1, 2.0));
        }

        [TestMethod]
        public void TestFuncAdapterWrapAndGC()
        {
            FuncAdapter adapter = new FuncAdapter();

            List<ParameterInfo> parameters = new List<ParameterInfo>() { 
                new ParameterInfo() { Name = "test1", DataType = DataType.Integer },
                new ParameterInfo() { Name = "test2", DataType = DataType.FloatingPoint }
            };

            Func<Dictionary<string, double>, double> function = (dictionary) => dictionary["test1"] + dictionary["test2"];

            Func<int, double, double> wrappedFunction = (Func<int, double, double>)adapter.Wrap(parameters, function);

            adapter = null;
            GC.Collect();

            Assert.AreEqual(3.0, wrappedFunction(1, 2.0));
        }

        [TestMethod]
        public void TestFourArguments()
        {
            FuncAdapter adapater = new FuncAdapter();

            List<ParameterInfo> parameters = new List<ParameterInfo>() { 
                new ParameterInfo() { Name = "test1", DataType = DataType.Integer },
                new ParameterInfo() { Name = "test2", DataType = DataType.Integer },
                new ParameterInfo() { Name = "test3", DataType = DataType.Integer },
                new ParameterInfo() { Name = "test4", DataType = DataType.Integer }
            };

            Func<int, int, int, int, double> wrappedFunction = (Func<int, int, int, int, double>)adapater.Wrap(parameters, dictionary => dictionary["test4"]);

            Assert.AreEqual(8.0, wrappedFunction(2, 4, 6, 8));
        }

        // Uncomment for debugging purposes
        //[TestMethod]
        //public void SaveToDisk()
        //{ 
        //    FuncAdapter adapter = new FuncAdapter();
        //    adapter.CreateDynamicModuleBuilder();
        //}
    }
}
