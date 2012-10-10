using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Calculator.Operations;

namespace Calculator.Execution
{
    public class FunctionBuilder
    {
        private readonly CalculationEngine engine;

        private string functionText;
        private DataType? resultDataType;
        private List<ParameterInfo> parameters;

        /// <summary>
        /// Creates a new instance of the FunctionBuilder class.
        /// </summary>
        /// <param name="functionText">
        /// A calculation engine instance that can be used for interpreting and executing 
        /// the function.
        /// </param>
        internal FunctionBuilder(string functionText, CalculationEngine engine)
        {
            this.parameters = new List<ParameterInfo>();
            this.functionText = functionText;
            this.engine = engine;
        }

        public FunctionBuilder Parameter(string name, DataType dataType)
        {
            parameters.Add(new ParameterInfo() {Name = name, DataType = dataType});
            return this;
        }

        public FunctionBuilder Result(DataType dataType)
        {
            resultDataType = dataType;
            return this;
        }

        public Delegate Build()
        {
            if (!resultDataType.HasValue)
                throw new Exception("Please define a result data type for the function.");

            Operation operation = engine.BuildAbstractSyntaxTree(functionText);

            return null;
        }

        //private void Test()
        //{
        //    ModuleBuilder moduleBuilder = CreateDynamicModuleBuilder();

        //    Type[] parameterTypes = new Type[] { typeof(int), typeof(double) };
        //    Type returnType = typeof(double);

        //    Type dictionaryType = typeof(Dictionary<string, double>);
        //    ConstructorInfo dictionaryConstructorInfo = dictionaryType.GetConstructor(Type.EmptyTypes);

        //    MethodBuilder methodBuilder = moduleBuilder.DefineGlobalMethod("test", MethodAttributes.Static, returnType, parameterTypes);

        //    ILGenerator generator = methodBuilder.GetILGenerator();

        //    generator.Emit(OpCodes.Newobj, new ConstructorInfo(dictionaryConstructorInfo));
        //    generator.Emit(OpCodes.Stloc_0);

        //    foreach (ParameterInfo parameter in parameters)
        //    {
        //        generator.Emit(OpCodes.Ldloc_0);
        //        generator.Emit(OpCodes.Ldstr, parameter.Name);
        //        generator.Emit(OpCodes.Ldarg_1);

        //        if(parameter.DataType != DataType.FloatingPoint)
        //            generator.Emit(OpCodes.Conv_R8);

        //        generator.Emit(OpCodes.Callvirt, dictionaryType.GetMethod("Add", new Type[] { typeof(string), typeof(double) }));
        //    }
        //}

        private ModuleBuilder CreateDynamicModuleBuilder()
        {
            AssemblyName assemblyName = new AssemblyName("JaceDynamicAssembly");
            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(assemblyName,
                AssemblyBuilderAccess.Run);
            return assemblyBuilder.DefineDynamicModule(assemblyName.Name);
        }
    }
}
