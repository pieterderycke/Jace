using System;
using System.Collections.Generic;
using System.Text;

namespace Jace.Execution
{
    public interface INumericConstants<T>
    {
        T Zero { get; }
        T One { get; }
    }
}
