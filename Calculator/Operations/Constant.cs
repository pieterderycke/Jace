using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator.Operations
{
    public class Constant<T> : Operation
    {
        public Constant(T value)
            : base(DataType.Integer)
        {
            this.Value = value;
        }

        public T Value { get; private set; }

        public override bool Equals(object obj)
        {
            Constant<T> other = obj as Constant<T>;
            if (other != null)
                return this.Value.Equals(other.Value);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }
}
