using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Jace.Util
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Get constructor for a given type matching with the parameter types provided.
        /// </summary>
        /// <param name="type">The type for witch a matching constructor must be found.</param>
        /// <param name="parameters">The types of the parameters of the constructor.</param>
        /// <returns>The matching constructor.</returns>
        public static ConstructorInfo GetConstructor(this Type type, Type[] parameters)
        {
            IEnumerable<ConstructorInfo> constructors =
                type.GetTypeInfo().DeclaredConstructors.Where(c => c.GetParameters().Length == parameters.Length);

            foreach (ConstructorInfo constructor in constructors)
            {
                bool parametersMatch = true;

                ParameterInfo[] constructorParameters = constructor.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i] != constructorParameters[i].ParameterType)
                    {
                        parametersMatch = false;
                        break;
                    }
                }

                if (parametersMatch)
                    return constructor;
            }

            throw new Exception("No constructor was found matching with the provided parameters.");
        }
    }
}