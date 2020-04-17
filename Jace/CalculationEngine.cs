using System;
using System.Collections.Generic;
using System.Text;

namespace Jace
{
    public static class CalculationEngine
    {
        public static ICalculationEngine<T> New<T>(JaceOptions options)
        {
            return GenericCalculationEngine<T>.New(options);
        }
    }
}
