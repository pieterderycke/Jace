using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Tokenizer
{
    public class TokenizerException : Exception
    {
        public TokenizerException(string message)
            : base(message)
        {
        }
    }
}
