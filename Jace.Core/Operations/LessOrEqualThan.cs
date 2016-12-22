﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Operations
{
    public class LessOrEqualThan : Operation
    {
        public LessOrEqualThan(DataType dataType, Operation argument1, Operation argument2)
            : base(dataType, argument1.DependsOnVariables || argument2.DependsOnVariables)
        {
            this.Argument1 = argument1;
            this.Argument2 = argument2;
        }

        public Operation Argument1 { get; internal set; }
        public Operation Argument2 { get; internal set; }
    }
}
