using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Operations
{
    public class ConversionToFloatingPoint : Operation
    {
        public ConversionToFloatingPoint(Operation operation)
            : base(DataType.FloatingPoint)
        {
            this.Operation = operation;
        }

        public Operation Operation { get; private set; }
    }
}
