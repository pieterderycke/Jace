﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Jace.Execution
{
    public interface IFunctionRegistry : IEnumerable<FunctionInfo>
    {
        FunctionInfo GetFunctionInfo(string functionName);
        bool IsFunctionName(string functionName);
        void RegisterFunction(string functionName, Delegate function);
        void RegisterFunction(string functionName, Delegate function, bool isOverWritable);
    }
}
