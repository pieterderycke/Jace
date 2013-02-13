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
            Type funcType = function.GetType();

            if (!funcType.FullName.StartsWith("System.Func"))
                throw new ArgumentException("Only System.Func delegates are permitted.", "function");

#if NETFX_CORE
            foreach (Type genericArgument in funcType.GenericTypeArguments)
#else
            foreach (Type genericArgument in funcType.GetGenericArguments())
#endif
                if (genericArgument != typeof(double))
                    throw new ArgumentException("Only doubles are supported as function arguments", "function");

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

            if (functions.ContainsKey(functionName) && functions[functionName].NumberOfParameters != numberOfParameters)
            {
                string message = string.Format("The number of parameters cannot be changed when overwriting a method.");
                throw new Exception(message);
            }

            FunctionInfo functionInfo = new FunctionInfo(functionName, numberOfParameters, isOverWritable, function);

            if (functions.ContainsKey(functionName))
                functions[functionName] = functionInfo;
            else
                functions.Add(functionName, functionInfo);
        }

        public bool IsFunctionName(string functionName)
        {
            return functions.ContainsKey(functionName);
        }
    }
}
