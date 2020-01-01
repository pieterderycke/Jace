using System;
using System.Collections.Generic;
using System.Text;

namespace Jace.Operations
{
    public class And : Operation
    {
        public And(DataType dataType, Operation argument1, Operation argument2)
            : base(dataType, argument1.DependsOnVariables || argument2.DependsOnVariables, argument1.IsIdempotent && argument2.IsIdempotent)
        {
            this.Argument1 = argument1;
            this.Argument2 = argument2;
        }

        public Operation Argument1 { get; internal set; }
        public Operation Argument2 { get; internal set; }
    }
}
