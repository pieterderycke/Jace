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
                case "csc":
                case "sec":
                case "asin":
                case "acos":
                case "tan":
                case "cot":
                case "atan":
                case "acot":
                case "loge":
                case "log10":
                case "logn":
                case "sqrt":
                case "abs":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Verify if the provided variable name is not a reserved one.
        /// Reserved variables name are not allowed to be (re)defined by users.
        /// </summary>
        /// <param name="variableName">The variable name to verify.</param>
        /// <returns>Returns true if the name matches with a reserved variable name.</returns>
        internal static bool IsReservedVariable(string variableName)
        {
            switch (variableName.ToLowerInvariant())
            {
                case "e":
                case "pi":
                    return true;
                default:
                    return false;
            }
        }
    }
}
