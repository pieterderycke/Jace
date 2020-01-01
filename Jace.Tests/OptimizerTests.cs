using Jace.Execution;
using Jace.Operations;
using Jace.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jace.Tests
{
    [TestClass]
    public class OptimizerTests
    {
        [TestMethod]
        public void TestIdempotentFunctionOptimization()
        {
            Optimizer optimizer = new Optimizer(new Interpreter());

            TokenReader tokenReader = new TokenReader(CultureInfo.InvariantCulture);
            IList<Token> tokens = tokenReader.Read("test(var1, (2+3) * 500)");

            IFunctionRegistry functionRegistry = new FunctionRegistry(true);
            functionRegistry.RegisterFunction("test", (Func<double, double, double>)((a, b) =>  a + b));

            AstBuilder astBuilder = new AstBuilder(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Function optimizedFuction = (Function)optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant), optimizedFuction.Arguments[1].GetType());
        }

        [TestMethod]
        public void TestNonIdempotentFunctionOptimization()
        {
            Optimizer optimizer = new Optimizer(new Interpreter());

            TokenReader tokenReader = new TokenReader(CultureInfo.InvariantCulture);
            IList<Token> tokens = tokenReader.Read("test(500)");

            IFunctionRegistry functionRegistry = new FunctionRegistry(true);
            functionRegistry.RegisterFunction("test", (Func<double, double>)(a => a), false, true);

            AstBuilder astBuilder = new AstBuilder(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedFuction = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(Function), optimizedFuction.GetType());
            Assert.AreEqual(typeof(IntegerConstant), ((Function)optimizedFuction).Arguments[0].GetType());
        }
    }
}
