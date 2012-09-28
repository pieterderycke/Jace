using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator
{
    public class Constant<T>
    {
        private T value;

        public Constant(T value)
        {
            this.value = value;
        }
    }
}
