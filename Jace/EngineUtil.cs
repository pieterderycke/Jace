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
