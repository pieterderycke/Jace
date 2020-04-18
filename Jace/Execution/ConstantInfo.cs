using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Execution
{
    public class ConstantInfo<T>
    {
        public ConstantInfo(string constantName, T value, bool isOverWritable)
        {
            this.ConstantName = constantName;
            this.Value = value;
            this.IsOverWritable = isOverWritable;
        }

        public string ConstantName { get; private set; }

        public T Value { get; private set; }

        public bool IsOverWritable { get; set; }
    }
}
