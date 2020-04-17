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
        public void TestOptimizerIdempotentFunction()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance));

            TokenReader<double> tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);            
            IList<Token> tokens = tokenReader.Read("test(var1, (2+3) * 500)");

            var functionRegistry = new FunctionRegistry<double>(true);
            functionRegistry.RegisterFunction("test", (Func<double, double, double>)((a, b) =>  a + b));

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Function optimizedFuction = (Function)optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedFuction.Arguments[1].GetType());
        }

        [TestMethod]
        public void TestOptimizerNonIdempotentFunction()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance));

            TokenReader<double> tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("test(500)");

            var functionRegistry = new FunctionRegistry<double>(true);
            functionRegistry.RegisterFunction("test", (Func<double, double>)(a => a), false, true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedFuction = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(Function), optimizedFuction.GetType());
            Assert.AreEqual(typeof(IntegerConstant), ((Function)optimizedFuction).Arguments[0].GetType());
        }

        [TestMethod]
        public void TestOptimizerMultiplicationByZero()
        {
            var optimizer = new Optimizer<double>(new Interpreter<double>(DoubleNumericalOperations.Instance));

            TokenReader<double> tokenReader = new TokenReader<double>(CultureInfo.InvariantCulture, DoubleNumericalOperations.Instance);
            IList<Token> tokens = tokenReader.Read("var1 * 0.0");

            var functionRegistry = new FunctionRegistry<double>(true);

            var astBuilder = new AstBuilder<double>(functionRegistry, true);
            Operation operation = astBuilder.Build(tokens);

            Operation optimizedOperation = optimizer.Optimize(operation, functionRegistry, null);

            Assert.AreEqual(typeof(FloatingPointConstant<double>), optimizedOperation.GetType());
            Assert.AreEqual(0.0, ((FloatingPointConstant<double>)optimizedOperation).Value);
        }
    }
}
