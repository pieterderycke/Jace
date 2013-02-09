using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Jace.Execution
{
    public class FunctionRegistry : IFunctionRegistry
    {
        Dictionary<string, FunctionInfo> functions;

        public FunctionRegistry()
        {
            this.functions = new Dictionary<string, FunctionInfo>();
        }

        public FunctionInfo GetFunctionInfo(string functionName)
        {
            FunctionInfo functionInfo = null;
            return functions.TryGetValue(functionName, out functionInfo) ? functionInfo : null;
        }

        public void RegisterFunction(string functionName, Delegate function)
        {
            RegisterFunction(functionName, function, true);
        }

        public void RegisterFunction(string functionName, Delegate function, bool isOverWritable)
        {
            if (functions.ContainsKey(functionName) && !functions[functionName].IsOverWritable)
            {
                string message = string.Format("The function \"{0}\" cannot be overwriten.", functionName);
                throw new Exception(message);
            }

#if NETFX_CORE
            int numberOfParameters = function.GetMethodInfo().GetParameters().Length;
#else
            int numberOfParameters = function.Method.GetParameters().Length;
#endif

            FunctionInfo functionInfo = new FunctionInfo(functionName, numberOfParameters, isOverWritable, function);

            if (functions.ContainsKey(functionName))
                functions[functionName] = functionInfo;
            else
                functions.Add(functionName, functionInfo);
        }

        public void RegisterFunction(string functionName, int numberOfParameters)
        {
            functions.Add(functionName, new FunctionInfo(functionName, numberOfParameters, false, null));
        }

        public bool IsFunctionName(string functionName)
        {
            return functions.ContainsKey(functionName);
        }
    }
}
