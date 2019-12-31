using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using Jace.Util;

namespace Jace.Execution
{
    public class FunctionRegistry : IFunctionRegistry
    {
        private const string DynamicFuncName = "Jace.DynamicFunc";

        private readonly bool caseSensitive;
        private readonly Dictionary<string, FunctionInfo> functions;

        public FunctionRegistry(bool caseSensitive)
        {
            this.caseSensitive = caseSensitive;
            this.functions = new Dictionary<string, FunctionInfo>();
        }

        public IEnumerator<FunctionInfo> GetEnumerator()
        {
            return functions.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public FunctionInfo GetFunctionInfo(string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException("functionName");

            FunctionInfo functionInfo = null;
            return functions.TryGetValue(ConvertFunctionName(functionName), out functionInfo) ? functionInfo : null;
        }

        public void RegisterFunction(string functionName, Delegate function)
        {
            RegisterFunction(functionName, function, true, true);
        }
        
        public void RegisterFunction(string functionName, Delegate function, bool isIdempotent, bool isOverWritable)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException("functionName");

            if (function == null)
                throw new ArgumentNullException("function");

            Type funcType = function.GetType();
            bool isDynamicFunc = false;
            int numberOfParameters = -1;
            
            if (funcType.FullName.StartsWith("System.Func"))
            {
                foreach (Type genericArgument in funcType.GenericTypeArguments)
                    if (genericArgument != typeof(double))
                        throw new ArgumentException("Only doubles are supported as function arguments.", "function");

                numberOfParameters = function
                    .GetMethodInfo()
                    .GetParameters()
                    .Count(p => p.ParameterType == typeof(double));
            }
            else if (funcType.FullName.StartsWith(DynamicFuncName))
            {
                isDynamicFunc = true;
            }
            else
                throw new ArgumentException("Only System.Func and " + DynamicFuncName + " delegates are permitted.", "function");

            functionName = ConvertFunctionName(functionName);

            if (functions.ContainsKey(functionName) && !functions[functionName].IsOverWritable)
            {
                string message = string.Format("The function \"{0}\" cannot be overwriten.", functionName);
                throw new Exception(message);
            }

            if (functions.ContainsKey(functionName) && functions[functionName].NumberOfParameters != numberOfParameters)
            {
                string message = string.Format("The number of parameters cannot be changed when overwriting a method.");
                throw new Exception(message);
            }

            if (functions.ContainsKey(functionName) && functions[functionName].IsDynamicFunc != isDynamicFunc)
            {
                string message = string.Format("A Func can only be overwritten by another Func and a DynamicFunc can only be overwritten by another DynamicFunc.");
                throw new Exception(message);
            }

            FunctionInfo functionInfo = new FunctionInfo(functionName, numberOfParameters, isIdempotent, isOverWritable, isDynamicFunc, function);

            if (functions.ContainsKey(functionName))
                functions[functionName] = functionInfo;
            else
                functions.Add(functionName, functionInfo);
        }

            public bool IsFunctionName(string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException("functionName");

            return functions.ContainsKey(ConvertFunctionName(functionName));
        }

        private string ConvertFunctionName(string functionName)
        {
            return caseSensitive ? functionName : functionName.ToLowerFast();
        }
    }
}
