using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Util;

namespace Jace.Execution
{
    public class ConstantRegistry : IConstantRegistry
    {
        private readonly bool caseSensitive;
        private readonly Dictionary<string, ConstantInfo> constants;

        public ConstantRegistry(bool caseSensitive)
        {
            this.caseSensitive = caseSensitive;
            this.constants = new Dictionary<string, ConstantInfo>();
        }

        public IEnumerator<ConstantInfo> GetEnumerator()
        {
            return constants.Values.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public ConstantInfo GetConstantInfo(string constantName)
        {
            if (string.IsNullOrEmpty(constantName))
                throw new ArgumentNullException("constantName");

            ConstantInfo constantInfo = null;
            return constants.TryGetValue(ConvertConstantName(constantName), out constantInfo) ? constantInfo : null;
        }

        public bool IsConstantName(string constantName)
        {
            if (string.IsNullOrEmpty(constantName))
                throw new ArgumentNullException("constantName");

            return constants.ContainsKey(ConvertConstantName(constantName));
        }

        public void RegisterConstant(string constantName, double value)
        {
            RegisterConstant(constantName, value, true);
        }

        public void RegisterConstant(string constantName, double value, bool isOverWritable)
        {
            if(string.IsNullOrEmpty(constantName))
                throw new ArgumentNullException("constantName");

            constantName = ConvertConstantName(constantName);

            if (constants.ContainsKey(constantName) && !constants[constantName].IsOverWritable)
            {
                string message = string.Format("The constant \"{0}\" cannot be overwriten.", constantName);
                throw new Exception(message);
            }

            ConstantInfo constantInfo = new ConstantInfo(constantName, value, isOverWritable);

            if (constants.ContainsKey(constantName))
                constants[constantName] = constantInfo;
            else
                constants.Add(constantName, constantInfo);
        }

        private string ConvertConstantName(string constantName)
        {
            return caseSensitive ? constantName : constantName.ToLowerFast();
        }
    }
}
