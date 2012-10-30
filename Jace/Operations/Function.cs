using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Operations
{
    public class Function : Operation
    {
        public Function(DataType dataType, FunctionType functionType, IList<Operation> arguments)
            : base(dataType, arguments.FirstOrDefault(o => o.DependsOnVariables) != null)
        {
            this.FunctionType = functionType;
            this.Arguments = arguments;
        }

        public FunctionType FunctionType { get; private set; }

        public IList<Operation> Arguments { get; private set; }
    }
}
