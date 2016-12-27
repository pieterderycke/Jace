using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Operations
{
    public class Exponentiation : Operation
    {
        public Exponentiation(DataType dataType, Operation @base, Operation exponent)
            : base(dataType, @base.DependsOnVariables || exponent.DependsOnVariables)
        {
            Base = @base;
            Exponent = exponent;
        }

        public Operation Base { get; internal set; }
        public Operation Exponent { get; internal set; }
    }
}
