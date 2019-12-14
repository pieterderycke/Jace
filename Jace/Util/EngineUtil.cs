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
                temp.Add(keyValuePair.Key.ToLowerFast(), keyValuePair.Value);
            }

            return temp;
        }

        // This is a fast ToLower for strings that are in ASCII
        static internal string ToLowerFast(this string text)
        {
            StringBuilder buffer = new StringBuilder(text.Length);

            for(int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c >= 'A' && c <= 'Z')
                {
                    buffer.Append((char)(c + 32));
                }
                else 
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }
    }
}
