﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Execution;

namespace Jace.Tests.Mocks
{
    public class MockFunctionRegistry<T> : IFunctionRegistry<T>
    {
        private HashSet<string> functionNames;

        public MockFunctionRegistry()
            : this(new string[] { "sin", "cos", "csc", "sec", "asin", "acos", "tan", "cot", "atan", "acot", "loge", "log10", "logn", "sqrt", "abs" })
        {
        }

        public MockFunctionRegistry(IEnumerable<string> functionNames)
        {
            this.functionNames = new HashSet<string>(functionNames);
        }

        public IEnumerator<FunctionInfo> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public FunctionInfo GetFunctionInfo(string functionName)
        {
            return new FunctionInfo(functionName, 1, true, false, false, null);
        }

        public bool IsFunctionName(string functionName)
        {
            return functionNames.Contains(functionName);
        }

        public void RegisterFunction(string functionName, Delegate function)
        {
            throw new NotImplementedException();
        }

        public void RegisterFunction(string functionName, Delegate function, bool isIdempotent, bool isOverWritable)
        {
            throw new NotImplementedException();
        }

        public void RegisterFunction(string functionName, int numberOfParameters)
        {
            throw new NotImplementedException();
        }

        public void RegisterFunction(string functionName, Delegate function, int numberOfParameters)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
