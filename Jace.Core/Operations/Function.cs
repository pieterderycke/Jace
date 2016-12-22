using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Operations
{
    public class Function : Operation
    {
        public Function(DataType dataType, string functionName, IList<Operation> arguments)
            : base(dataType, arguments.FirstOrDefault(o => o.DependsOnVariables) != null)
        {
            this.FunctionName = functionName;
            this.Arguments = arguments;
        }

        public string FunctionName { get; private set; }

        public IList<Operation> Arguments { get; private set; }
    }
}
