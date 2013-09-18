using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Execution
{
    public interface IConstantRegistry : IEnumerable<ConstantInfo>
    {
        ConstantInfo GetConstantInfo(string constantName);
        bool IsConstantName(string constantName);
        void RegisterConstant(string constantName, double value);
        void RegisterConstant(string constantName, double value, bool isOverWritable);
    }
}
