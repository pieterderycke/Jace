using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jace.Benchmark
{
    public class BenchMarkOperation
    {
        public string Formula { get; set; }
        public Func<CalculationEngine, string, TimeSpan>  BenchMarkDelegate { get; set; }
    }
}
