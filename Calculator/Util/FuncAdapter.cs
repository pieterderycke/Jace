using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Calculator.Operations;

namespace Calculator.Util
{
    /// <summary>
    /// An adapter for creating a func wrapper around a func accepting a dictionary. The wrapper
    /// can create a func that has an argument for every expected key in the dictionary.
    /// </summary>
    public class FuncAdapter
    {
        /// <summary>
        /// Wrap the parsed the function into a delegate of the specified type. The delegate must accept 
        /// the parameters defined in the parameters collection. The order of parameters is respected as defined
        /// in parameters collection.
        /// <br/>
        /// The function must accept a dictionary of strings and doubles as input. The values passed to the 
        /// wrapping function will be passed to the function using the dictionary. The keys in the dictionary
        /// are the names of the parameters of the wrapping function.
        /// </summary>
        /// <param name="parameters">The required parameters of the wrapping function delegate.</param>
        /// <param name="function">The function that must be wrapped.</param>
        /// <returns>A delegate instance of the required type.</returns>
        public Delegate Wrap(IEnumerable<Calculator.Execution.ParameterInfo> parameters, 
            Func<Dictionary<string, double>, double> function)
        {
            Calculator.Execution.ParameterInfo[] parameterArray = parameters.ToArray();

            Type[] parameterTypes = GetParameterTypes(parameterArray);

            Type delegateType = GetDelegateType(parameterArray);

            DynamicMethod method = new DynamicMethod("FuncWrapperMethod", typeof(double), 
                parameterTypes, typeof(FuncAdapterArguments));

            ILGenerator generator = method.GetILGenerator();

            GenerateMethodBody(generator, parameterArray, function);

            for (int i = 0; i < parameterArray.Length; i++)
            {
                Calculator.Execution.ParameterInfo parameter = parameterArray[i];
                method.DefineParameter((i + 1), ParameterAttributes.In, parameter.Name);
            }

            return method.CreateDelegate(delegateType, new FuncAdapterArguments(function));
        }

        // Uncomment for debugging purposes
        //public void CreateDynamicModuleBuilder()
        //{
        //    AssemblyName assemblyName = new AssemblyName("JaceDynamicAssembly");
        //    AppDomain domain = AppDomain.CurrentDomain;
        //    AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(assemblyName,
        //        AssemblyBuilderAccess.RunAndSave);
        //    ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, "test.dll");

        //    TypeBuilder typeBuilder = moduleBuilder.DefineType("MyTestClass");

        //    MethodBuilder method = typeBuilder.DefineMethod("MyTestMethod", MethodAttributes.Static, typeof(double),
        //       new Type[] { typeof(FuncAdapterArguments), typeof(int), typeof(double) });

        //    ILGenerator generator = method.GetILGenerator();
        //    GenerateMethodBody(generator, new List<Calculator.Execution.ParameterInfo>() { 
        //        new Calculator.Execution.ParameterInfo() { Name = "test1", DataType = DataType.Integer },
        //        new Calculator.Execution.ParameterInfo() { Name = "test2", DataType = DataType.FloatingPoint }},
        //        (a) => 0.0);

        //    typeBuilder.CreateType();

        //    assemblyBuilder.Save(@"test.dll");
        //}

        private Type GetDelegateType(Calculator.Execution.ParameterInfo[] parameters)
        {
            string funcTypeName = string.Format("System.Func`{0}", parameters.Length + 1);
            Type funcType = Type.GetType(funcTypeName);

            Type[] typeArguments = new Type[parameters.Length + 1];
            for (int i = 0; i < parameters.Length; i++)
                typeArguments[i] = (parameters[i].DataType == DataType.FloatingPoint) ? typeof(double) : typeof(int);
            typeArguments[typeArguments.Length - 1] = typeof(double);

            return funcType.MakeGenericType(typeArguments);
        }

        private Type[] GetParameterTypes(Calculator.Execution.ParameterInfo[] parameters)
        {
            Type[] parameterTypes = new Type[parameters.Length + 1];

            parameterTypes[0] = typeof(FuncAdapterArguments);

            for (int i = 0; i < parameters.Length; i++)
                parameterTypes[i + 1] = (parameters[i].DataType == DataType.FloatingPoint) ? typeof(double) : typeof(int);

            return parameterTypes;
        }

        private void GenerateMethodBody(ILGenerator generator, Calculator.Execution.ParameterInfo[] parameters,
            Func<Dictionary<string, double>, double> function)
        {
            Type dictionaryType = typeof(Dictionary<string, double>);
            ConstructorInfo dictionaryConstructorInfo = dictionaryType.GetConstructor(Type.EmptyTypes);

            FieldInfo functionField = typeof(FuncAdapterArguments).GetField("function", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            generator.DeclareLocal(dictionaryType);
            generator.DeclareLocal(typeof(double));

            generator.Emit(OpCodes.Newobj, dictionaryConstructorInfo);
            generator.Emit(OpCodes.Stloc_0);

            for (int i = 0; i < parameters.Length; i++)
            {
                Calculator.Execution.ParameterInfo parameter = parameters[i];

                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldstr, parameter.Name);

                switch (i)
                {
                    case 0:
                        generator.Emit(OpCodes.Ldarg_1);
                        break;
                    case 1:
                        generator.Emit(OpCodes.Ldarg_2);
                        break;
                    case 2:
                        generator.Emit(OpCodes.Ldarg_3);
                        break;
                }

                if (parameter.DataType != DataType.FloatingPoint)
                    generator.Emit(OpCodes.Conv_R8);

                generator.Emit(OpCodes.Callvirt, dictionaryType.GetMethod("Add", new Type[] { typeof(string), typeof(double) }));
            }

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, functionField);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Callvirt, function.GetType().GetMethod("Invoke"));

            generator.Emit(OpCodes.Ret);
        }

        private class FuncAdapterArguments
        {
            private readonly Func<Dictionary<string, double>, double> function;

            public FuncAdapterArguments(Func<Dictionary<string, double>, double> function)
            {
                this.function = function;
            }
        }
    }
}
