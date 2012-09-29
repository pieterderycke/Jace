using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator.Operations
{
    public class Substraction<T> : Operation<T>
    {
        public Substraction(Operation<T> argument1, Operation<T> argument2)
        {
            this.Argument1 = argument1;
            this.Argument2 = argument2;
        }

        public Operation<T> Argument1 { get; private set; }
        public Operation<T> Argument2 { get; private set; }
    }
}
