using System;
using System.Collections.Generic;
using System.Text;

namespace Jace.Operations
{
    public class Negation : Operation
    {
        public Negation(DataType dataType, Operation argument)
            : base(dataType, argument.DependsOnVariables)
        {
            this.Argument = argument;
        }

        public Operation Argument { get; internal set; }
    }
}
