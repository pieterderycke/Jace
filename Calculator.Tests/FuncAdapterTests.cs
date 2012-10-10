using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.Execution;
using Calculator.Operations;
using Calculator.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
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

            Func<int, double, double> wrappedFunction = (Func<int, double, double>)adapter.Wrap(typeof(Func<int, double, double>), 
                parameters, function);

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

            Func<int, double, double> wrappedFunction = (Func<int, double, double>)adapter.Wrap(typeof(Func<int, double, double>),
                parameters, function);

            adapter = null;
            GC.Collect();

            Assert.AreEqual(3.0, wrappedFunction(1, 2.0));
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
