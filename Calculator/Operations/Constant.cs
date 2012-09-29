using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator.Operations
{
    public class Constant<T> : Operation<T>
    {
        public Constant(T value)
        {
            this.Value = value;
        }

        public T Value { get; private set; }
    }
}
