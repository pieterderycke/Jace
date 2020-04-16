using System;
using System.Collections.Generic;
using System.Text;

namespace Jace
{
    public static class CalculationEngine
    {
        public static ICalculationEngine<T> Create<T>(JaceOptions options)
        {
            if (typeof(T) == typeof(double))
            {
                return (ICalculationEngine<T>)new DoubleCalculationEngine(options);
            }
            else
            {
                return (ICalculationEngine<T>)new DecimalCalculationEngine(options);
            }
        }
    }
}
