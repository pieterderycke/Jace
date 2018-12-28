using Jace.Execution;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Jace
{
    public class JaceOptions
    {
        public JaceOptions()
        {
            CultureInfo = CultureInfo.CurrentCulture;
            ExecutionMode = ExecutionMode.Compiled;
            CacheEnabled = true;
            OptimizerEnabled = true;
            AdjustVariableCase = true;
            DefaultFunctions = true;
            DefaultConstants = true;
        }

        public CultureInfo CultureInfo { get; set; }
        public ExecutionMode ExecutionMode { get; set; }
        public bool CacheEnabled { get; set; }
        public bool OptimizerEnabled { get; set; }
        public bool AdjustVariableCase { get; private set; }
        public bool DefaultFunctions { get; set; }
        public bool DefaultConstants { get; set; }
    }
}
