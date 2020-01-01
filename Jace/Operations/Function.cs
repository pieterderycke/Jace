using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Operations
{
    public class Function : Operation
    {
        private IList<Operation> arguments;

        public Function(DataType dataType, string functionName, IList<Operation> arguments, bool isIdempotent)
            : base(dataType, arguments.FirstOrDefault(o => o.DependsOnVariables) != null, isIdempotent && arguments.All(o => o.IsIdempotent))
        {
            this.FunctionName = functionName;
            this.arguments = arguments;
        }

        public string FunctionName { get; private set; }

        public IList<Operation> Arguments {
            get
            {
                return arguments;
            }
            internal set
            {
                this.arguments = value;
                this.DependsOnVariables = arguments.FirstOrDefault(o => o.DependsOnVariables) != null;
            }
        }
    }
}
