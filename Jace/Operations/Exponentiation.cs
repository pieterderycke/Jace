using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Operations
{
    public class Exponentiation : Operation
    {
        public Exponentiation(DataType dataType, Operation @base, Operation exponent)
            : base(dataType)
        {
            Base = @base;
            Exponent = exponent;
        }

        public Operation Base { get; private set; }
        public Operation Exponent { get; private set; }
    }
}
