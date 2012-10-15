using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Jace.Tokenizer
{
    /// <summary>
    /// A token reader that converts the input string in a list of tokens.
    /// </summary>
    public class TokenReader
    {
        private readonly CultureInfo cultureInfo;
        private readonly char decimalSeparator; 

        public TokenReader() 
            : this(CultureInfo.CurrentCulture)
        {
        }

        public TokenReader(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
            this.decimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator[0];
        }

        /// <summary>
        /// Read in the provided formula and convert it into a list of takens that can be processed by the
        /// Abstract Syntax Tree Builder.
        /// </summary>
        /// <param name="formula">The formula that must be converted into a list of tokens.</param>
        /// <returns>The list of tokens for the provided formula.</returns>
        public List<object> Read(string formula)
        {
            if (string.IsNullOrEmpty(formula))
                throw new ArgumentNullException("formula");

            List<object> tokens = new List<object>();

            char[] characters = formula.ToCharArray();

            for(int i = 0; i < characters.Length; i++)
            {
                if (IsPartOfNumeric(characters[i]))
                {
                    string buffer = "" + characters[i];

                    while (++i < characters.Length && IsPartOfNumeric(characters[i]))
                    {
                        buffer += characters[i];
                    }

                    // Verify if we do not have an int
                    int intValue;
                    if (int.TryParse(buffer, out intValue))
                    {
                        tokens.Add(intValue);
                    }
                    else
                    {
                        double doubleValue;
                        if (double.TryParse(buffer, NumberStyles.Float | NumberStyles.AllowThousands,
                            cultureInfo, out doubleValue))
                        {
                            tokens.Add(doubleValue);
                        }
                        // Else we skip
                    }

                    if (i == characters.Length)
                    {
                        // Last character read
                        continue;
                    }
                }

                if (IsPartOfVariable(characters[i], true))
                {
                    string buffer = "" + characters[i];

                    while (++i < characters.Length && IsPartOfVariable(characters[i], false))
                    {
                        buffer += characters[i];
                    }

                    tokens.Add(buffer);

                    if (i == characters.Length)
                    {
                        // Last character read
                        continue;
                    }
                }

                switch (characters[i])
                { 
                    case ' ':
                        continue;
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '(':
                    case ')':
                        tokens.Add(characters[i]);
                        break;
                    default:
                        break;
                }
            }

            return tokens;
        }

        private bool IsPartOfNumeric(char character)
        {
            return character == decimalSeparator || (character >= '0' && character <= '9');
        }

        private bool IsPartOfVariable(char character, bool isFirstCharacter)
        {
            return (character >= 'a' && character <= 'z') || (character >= 'A' && character <= 'Z') || (!isFirstCharacter && character >= '0' && character <= '9');
        }
    }
}
