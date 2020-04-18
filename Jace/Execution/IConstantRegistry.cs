using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Execution
{
    public interface IConstantRegistry<T> : IEnumerable<ConstantInfo<T>>
    {
        ConstantInfo<T> GetConstantInfo(string constantName);
        bool IsConstantName(string constantName);
        void RegisterConstant(string constantName, T value);
        void RegisterConstant(string constantName, T value, bool isOverWritable);
    }
}
