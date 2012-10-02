using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator.Operations
{
    public abstract class Operation
    {
        public Operation(DataType dataType)
        {
            this.DataType = dataType;
        }

        public DataType DataType { get; set; }
    }
}
