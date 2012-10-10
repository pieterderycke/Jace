using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Calculator.Operations;

namespace Calculator
{
    public class ReflectionEmitInterpreter : IInterpreter
    {
        public double Execute(Operation operation)
        {
            return Execute(operation, new Dictionary<string, double>());
        }

        public double Execute(Operation operation, Dictionary<string, int> variables)
        {
            Dictionary<string, double> doubleVariables = new Dictionary<string, double>();
            foreach (string key in variables.Keys)
                doubleVariables.Add(key, variables[key]);

            return Execute(operation, doubleVariables);
        }

        public double Execute(Operation operation, Dictionary<string, double> variables)
        {
            if (operation == null)
                throw new ArgumentException("operation");

            if (operation.GetType() == typeof(IntegerConstant))
            {
                IntegerConstant constant = (IntegerConstant)operation;
                return constant.Value;
            }
            else if (operation.GetType() == typeof(FloatingPointConstant))
            {
                FloatingPointConstant constant = (FloatingPointConstant)operation;
                return constant.Value;
            }
            else if (operation.GetType() == typeof(Variable))
            {
                Variable variable = (Variable)operation;
                if (variables.ContainsKey(variable.Name))
                    return variables[variable.Name];
                else
                    throw new VariableNotDefinedException(string.Format("The variable \"{0}\" used is not defined.", variable.Name));
            }
            else if (operation.GetType() == typeof(Multiplication))
            {
                Multiplication multiplication = (Multiplication)operation;
                return Execute(multiplication.Argument1, variables) * Execute(multiplication.Argument2, variables);
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition)operation;
                return Execute(addition.Argument1, variables) + Execute(addition.Argument2, variables);
            }
            else if (operation.GetType() == typeof(Substraction))
            {
                Substraction addition = (Substraction)operation;
                return Execute(addition.Argument1, variables) - Execute(addition.Argument2, variables);
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division)operation;
                return Execute(division.Dividend, variables) / Execute(division.Divisor, variables);
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }

        private void Test()
        {
            ModuleBuilder moduleBuilder = CreateDynamicModuleBuilder();

            Type[] parameterTypes = new Type[] { typeof(int), typeof(double)};
            Type returnType = typeof(double);

            ConstructorInfo dictionaryConstructorInfo = typeof(Dictionary<string, double>).GetConstructor(Type.EmptyTypes);

            MethodBuilder methodBuilder = moduleBuilder.DefineGlobalMethod("test", MethodAttributes.Static, returnType, parameterTypes);

            ILGenerator generator = methodBuilder.GetILGenerator();

            //generator.Emit(OpCodes.Newobj, new ConstructorInfo(dictionaryConstructorInfo));
            generator.Emit(OpCodes.Stloc_0);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ldstr, "parameter1");
        }

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
