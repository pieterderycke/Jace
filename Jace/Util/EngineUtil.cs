using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Util
{
    /// <summary>
    /// Utility methods of Jace.NET that can be used throughout the engine.
    /// </summary>
    internal  static class EngineUtil
    {
        static internal IDictionary<string, double> ConvertVariableNamesToLowerCase(IDictionary<string, double> variables)
        {
            Dictionary<string, double> temp = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> keyValuePair in variables)
            {
                temp.Add(keyValuePair.Key.ToLowerInvariant(), keyValuePair.Value);
            }

            return temp;
        }
    }
}
