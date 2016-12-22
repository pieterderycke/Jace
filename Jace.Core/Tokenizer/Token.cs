using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Tokenizer
{
    /// <summary>
    /// Represents an input token
    /// </summary>
    public struct Token
    {
        /// <summary>
        /// The start position of the token in the input function text.
        /// </summary>
        public int StartPosition;
        
        /// <summary>
        /// The length of token in the input function text.
        /// </summary>
        public int Length;

        /// <summary>
        /// The type of the token.
        /// </summary>
        public TokenType TokenType;

        /// <summary>
        /// The value of the token.
        /// </summary>
        public object Value;
    }
}
