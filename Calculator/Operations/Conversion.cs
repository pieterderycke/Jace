using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator.Operations
{
    public class Conversion : Operation
    {
        public Conversion(DataType targetDataType, double value)
        {
            this.IntegerValue = (int)value;
            this.FloatingPointValue = value;
            this.DataType = targetDataType;
        }

        public Conversion(DataType targetDataType, int value)
        {
            this.IntegerValue = value;
            this.FloatingPointValue = value;
            this.DataType = targetDataType;
        }

        public int IntegerValue { get; private set; }
        public double FloatingPointValue { get; private set; }
    }
}
