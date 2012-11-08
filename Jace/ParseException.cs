using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace
{
    /// <summary>
    /// The exception that is thrown when there is a syntax error in the formula provided 
    /// to the calculation engine.
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException(string message)
            : base(message)
        {
        }
    }
}
