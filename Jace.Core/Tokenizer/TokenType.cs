using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Tokenizer
{
    public enum TokenType
    {
        Integer,
        FloatingPoint,
        Text,
        Operation,
        LeftBracket,
        RightBracket,
        ArgumentSeparator
    }
}
