using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Execution
{
    public class FunctionInfo
    {
        public FunctionInfo(string functionName, int numberOfParameters, bool isOverWritable, bool isDynamicFunc, Delegate function)
        {
            this.FunctionName = functionName;
            this.NumberOfParameters = numberOfParameters;
            this.IsOverWritable = isOverWritable;
            this.IsDynamicFunc = isDynamicFunc;
            this.Function = function;
        }

        public string FunctionName { get; private set; }
        
        public int NumberOfParameters { get; private set; }

        public bool IsOverWritable { get; set; }

        public bool IsDynamicFunc { get; private set; }

        public Delegate Function { get; private set; }
    }
}
