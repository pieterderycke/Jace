using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Jace.Benchmark
{
    public class FunctionGenerator
    {
        private const int NumberOfVariables = 3;

        private readonly CultureInfo cultureInfo;
        private readonly Random random;

        public FunctionGenerator()
            : this(CultureInfo.CurrentCulture)
        {
        }

        public FunctionGenerator(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
            this.random = new Random();
        }

        public string Next()
        {
            Queue<string> variables = new Queue<string>();
            for (int i = 0; i < NumberOfVariables; i++)
                variables.Enqueue("var" + (i + 1));

            StringBuilder sb = new StringBuilder();
            Generate(sb, variables);

            return sb.ToString();
        }

        private void Generate(StringBuilder result, Queue<string> variables)
        {
            double value = random.NextDouble();

            if (value < 0.35)
            {
                result.Append(variables.Dequeue());
                result.Append(GetRandomOperator());

                if(variables.Count > 0)
                    Generate(result, variables);
                else
                    result.Append(GetRandomValue());
            }
            else if (value < 0.8)
            {
                result.Append(GetRandomValue());
                result.Append(GetRandomOperator());

                if (variables.Count > 0)
                    Generate(result, variables);
                else
                    result.Append(GetRandomValue());
            }
            else
            {
                if (variables.Count > 0)
                {
                    result.Append('(');
                    Generate(result, variables);
                    result.Append(')');
                    result.Append(GetRandomOperator());

                    if (variables.Count > 0)
                        Generate(result, variables);
                    else
                        result.Append(GetRandomValue());
                }
                else
                    result.Append(GetRandomValue());
            }
        }

        private char GetRandomOperator()
        {
            double value = random.NextDouble();

            if (value < 0.2)
                return '+';
            else if (value < 0.4)
                return '*';
            else if (value < 0.6)
                return '/';
            else //if (value < 0.8) TODO add support for '^'
                return '-';
        }

        private string GetRandomValue()
        {
            double value = random.NextDouble();

            if (value < 0.6)
                return random.Next().ToString(cultureInfo);
            else
                return (random.Next() * random.NextDouble()).ToString(cultureInfo);
        }
    }
}
