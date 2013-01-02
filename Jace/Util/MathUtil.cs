using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Util
{
    public static class MathUtil
    {
        public static double Cot(double a)
        {
            return 1 / Math.Tan(a);
        }

        public static double Acot(double d)
        {
            return Math.Atan(1 / d);
        }

        public static double Csc(double a)
        {
            return 1 / Math.Sin(a);
        }

        public static double Sec(double d)
        {
            return 1 / Math.Cos(d);
        }
    }
}
