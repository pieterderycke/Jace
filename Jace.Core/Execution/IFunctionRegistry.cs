using System;

namespace Jace.Execution
{
    public interface IFunctionRegistry
    {
        FunctionInfo GetFunctionInfo(string functionName);
        bool IsFunctionName(string functionName);
        void RegisterFunction(string functionName, Delegate function);
        void RegisterFunction(string functionName, Delegate function, bool isOverWritable);
    }
}
