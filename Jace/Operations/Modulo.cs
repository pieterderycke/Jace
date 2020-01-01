using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Operations
{
    public class Modulo : Operation
    {
        public Modulo(DataType dataType, Operation dividend, Operation divisor)
            : base(dataType, dividend.DependsOnVariables || divisor.DependsOnVariables, dividend.IsIdempotent && divisor.IsIdempotent)
        {
            this.Dividend = dividend;
            this.Divisor = divisor;
        }

        public Operation Dividend { get; internal set; }
        public Operation Divisor { get; internal set; }
    }
}
