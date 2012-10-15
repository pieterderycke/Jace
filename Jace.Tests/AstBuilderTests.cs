using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jace.Tests
{
    [TestClass]
    public class AstBuilderTests
    {
        [TestMethod]
        public void TestBuildFormula1()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<object>() { '(', 42, '+', 8, ')', '*', 2 });

            Multiplication multiplication = (Multiplication)operation;
            Addition addition = (Addition)multiplication.Argument1;

            Assert.AreEqual(42, ((Constant<int>)addition.Argument1).Value);
            Assert.AreEqual(8, ((Constant<int>)addition.Argument2).Value);
            Assert.AreEqual(2, ((Constant<int>)multiplication.Argument2).Value);
        }

        [TestMethod]
        public void TestBuildFormula2()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<object>() { 2, '+', 8, '*', 3 });

            Addition addition = (Addition)operation;
            Multiplication multiplication = (Multiplication)addition.Argument2;

            Assert.AreEqual(2, ((Constant<int>)addition.Argument1).Value);
            Assert.AreEqual(8, ((Constant<int>)multiplication.Argument1).Value);
            Assert.AreEqual(3, ((Constant<int>)multiplication.Argument2).Value);
        }

        [TestMethod]
        public void TestBuildFormula3()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<object>() { 2, '*', 8, '-', 3 });

            Substraction substraction = (Substraction)operation;
            Multiplication multiplication = (Multiplication)substraction.Argument1;

            Assert.AreEqual(3, ((Constant<int>)substraction.Argument2).Value);
            Assert.AreEqual(2, ((Constant<int>)multiplication.Argument1).Value);
            Assert.AreEqual(8, ((Constant<int>)multiplication.Argument2).Value);
        }

        [TestMethod]
        public void TestDivision()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<object>() { 10, '/', 2 });

            Assert.AreEqual(typeof(Division), operation.GetType());

            Division division = (Division)operation;

            Assert.AreEqual(new IntegerConstant(10), division.Dividend);
            Assert.AreEqual(new IntegerConstant(2), division.Divisor);
        }

        [TestMethod]
        public void TestMultiplication()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<object>() { 10, '*', 2.0 });

            Multiplication multiplication = (Multiplication)operation;

            Assert.AreEqual(new IntegerConstant(10), multiplication.Argument1);
            Assert.AreEqual(new FloatingPointConstant(2.0), multiplication.Argument2);
        }

        [TestMethod]
        public void TestVariable()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<object>() { 10, '*', "var1" });

            Multiplication multiplication = (Multiplication)operation;

            Assert.AreEqual(new IntegerConstant(10), multiplication.Argument1);
            Assert.AreEqual(new Variable("var1"), multiplication.Argument2);
        }

        [TestMethod]
        public void TestMultipleVariable()
        {
            AstBuilder builder = new AstBuilder();
            Operation operation = builder.Build(new List<object>() { "var1", '+', 2, '*', '(', 3, '*', "age", ')' });

            Addition addition = (Addition)operation;
            Multiplication multiplication1 = (Multiplication)addition.Argument2;
            Multiplication multiplication2 = (Multiplication)multiplication1.Argument2;

            Assert.AreEqual(new Variable("var1"), addition.Argument1);
            Assert.AreEqual(new IntegerConstant(2), multiplication1.Argument1);
            Assert.AreEqual(new IntegerConstant(3), multiplication2.Argument1);
            Assert.AreEqual(new Variable("age"), multiplication2.Argument2);
        }
    }
}
