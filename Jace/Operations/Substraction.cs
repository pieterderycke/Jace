using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Operations
{
    public class Substraction : Operation
    {
        public Substraction(DataType dataType, Operation argument1, Operation argument2)
            : base(dataType)
        {
            this.Argument1 = argument1;
            this.Argument2 = argument2;
        }

        public Operation Argument1 { get; private set; }
        public Operation Argument2 { get; private set; }
    }
}
