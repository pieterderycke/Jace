using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace
{
    /// <summary>
    /// Utility methods of Jace.NET that can be used throughout the engine.
    /// </summary>
    internal static class EngineUtil
    {
        /// <summary>
        /// Verify if the provided text matches with a function name.
        /// Variables are not allowed to match with a function name.
        /// </summary>
        /// <param name="text">The text to verify.</param>
        /// <returns>Returns true if the name matches with a function name.</returns>
        internal static bool IsFunctionName(string text)
        {
            switch (text.ToLowerInvariant())
            {
                case "sin":
                case "cos":
                case "loge":
                case "log10":
                case "logn":
                    return true;
                default:
                    return false;
            }
        }
    }
}
