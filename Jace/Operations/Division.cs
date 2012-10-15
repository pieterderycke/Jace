using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Operations
{
    public class Division : Operation
    {
        public Division(DataType dataType, Operation dividend, Operation divisor)
            : base(dataType)
        {
            this.Dividend = dividend;
            this.Divisor = divisor;
        }

        public Operation Dividend { get; private set; }
        public Operation Divisor { get; private set; }
    }
}
