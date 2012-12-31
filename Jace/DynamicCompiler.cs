using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Jace.Operations;
using Jace.Util;

namespace Jace
{
#if !NETFX_CORE
    public class DynamicCompiler : IExecutor
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
            return BuildFunction(operation)(variables);
        }

        public Func<Dictionary<string, double>, double> BuildFunction(Operation operation)
        {
            DynamicMethod method = new DynamicMethod("MyCalcMethod", typeof(double),
                new Type[] { typeof(Dictionary<string, double>) });
            GenerateMethodBody(method, operation);

            Func<Dictionary<string, double>, double> function =
                (Func<Dictionary<string, double>, double>)method.CreateDelegate(typeof(Func<Dictionary<string, double>, double>));

            return function;
        }

        private void GenerateMethodBody(DynamicMethod method, Operation operation)
        {
            ILGenerator generator = method.GetILGenerator();
            generator.DeclareLocal(typeof(double));
            GenerateMethodBody(generator, operation);
            generator.Emit(OpCodes.Ret);
        }

        private void GenerateMethodBody(ILGenerator generator, Operation operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            if (operation.GetType() == typeof(IntegerConstant))
            {
                IntegerConstant constant = (IntegerConstant)operation;
                
                generator.Emit(OpCodes.Ldc_I4, constant.Value);
                generator.Emit(OpCodes.Conv_R8);
            }
            else if (operation.GetType() == typeof(FloatingPointConstant))
            {
                FloatingPointConstant constant = (FloatingPointConstant)operation;

                generator.Emit(OpCodes.Ldc_R8, constant.Value);
            }
            else if (operation.GetType() == typeof(Variable))
            {
                Type dictionaryType = typeof(Dictionary<string, double>);

                Variable variable = (Variable)operation;

                Label throwExceptionLabel = generator.DefineLabel();
                Label returnLabel = generator.DefineLabel();

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, variable.Name);
                generator.Emit(OpCodes.Callvirt, dictionaryType.GetMethod("ContainsKey", new Type[] { typeof(string) }));
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ceq);
                generator.Emit(OpCodes.Brtrue_S, throwExceptionLabel);

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, variable.Name);
                generator.Emit(OpCodes.Callvirt, dictionaryType.GetMethod("get_Item", new Type[] { typeof(string) }));
                generator.Emit(OpCodes.Br_S, returnLabel);

                generator.MarkLabel(throwExceptionLabel);
                generator.Emit(OpCodes.Ldstr, string.Format("The variable \"{0}\" used is not defined.", variable.Name));
                generator.Emit(OpCodes.Newobj, typeof(VariableNotDefinedException).GetConstructor(new Type[] { typeof(string) }));
                generator.Emit(OpCodes.Throw);

                generator.MarkLabel(returnLabel);
            }
            else if (operation.GetType() == typeof(Multiplication))
            {
                Multiplication multiplication = (Multiplication)operation;
                GenerateMethodBody(generator, multiplication.Argument1);
                GenerateMethodBody(generator, multiplication.Argument2);

                generator.Emit(OpCodes.Mul);
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition)operation;
                GenerateMethodBody(generator, addition.Argument1);
                GenerateMethodBody(generator, addition.Argument2);

                generator.Emit(OpCodes.Add);
            }
            else if (operation.GetType() == typeof(Subtraction))
            {
                Subtraction addition = (Subtraction)operation;
                GenerateMethodBody(generator, addition.Argument1);
                GenerateMethodBody(generator, addition.Argument2);

                generator.Emit(OpCodes.Sub);
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division)operation;
                GenerateMethodBody(generator, division.Dividend);
                GenerateMethodBody(generator, division.Divisor);

                generator.Emit(OpCodes.Div);
            }
            else if (operation.GetType() == typeof(Modulo))
            {
                Modulo modulo = (Modulo)operation;
                GenerateMethodBody(generator, modulo.Dividend);
                GenerateMethodBody(generator, modulo.Divisor);

                generator.Emit(OpCodes.Rem);
            }
            else if (operation.GetType() == typeof(Exponentiation))
            {
                Exponentiation exponentation = (Exponentiation)operation;
                GenerateMethodBody(generator, exponentation.Base);
                GenerateMethodBody(generator, exponentation.Exponent);

                generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Pow"));
            }
            else if (operation.GetType() == typeof(Function))
            {
                Function function = (Function)operation;

                switch (function.FunctionType)
                {
                    case FunctionType.Sine:
                        GenerateMethodBody(generator, function.Arguments[0]);
                        
                        generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Sin"));
                        break;
                    case FunctionType.Cosine:
                        GenerateMethodBody(generator, function.Arguments[0]);
                        
                        generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Cos"));
                        break;
                    case FunctionType.Arcsine:
                        GenerateMethodBody(generator, function.Arguments[0]);

                        generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Asin", new Type[] { typeof(double) }));
                        break;
                    case FunctionType.Arccosine:
                        GenerateMethodBody(generator, function.Arguments[0]);

                        generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Acos", new Type[] { typeof(double) }));
                        break;
                    case FunctionType.Tangent:
                        GenerateMethodBody(generator, function.Arguments[0]);

                        generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Tan", new Type[] { typeof(double) }));
                        break;
                    case FunctionType.Cotangent:
                        GenerateMethodBody(generator, function.Arguments[0]);

                        generator.Emit(OpCodes.Call, typeof(MathUtil).GetMethod("Cot", new Type[] { typeof(double) }));
                        break;
                    case FunctionType.Arctangent:
                        GenerateMethodBody(generator, function.Arguments[0]);

                        generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Atan", new Type[] { typeof(double) }));
                        break;
                    case FunctionType.Arccotangent:
                        GenerateMethodBody(generator, function.Arguments[0]);

                        generator.Emit(OpCodes.Call, typeof(MathUtil).GetMethod("Acot", new Type[] { typeof(double) }));
                        break;
                    case FunctionType.Loge:
                        GenerateMethodBody(generator, function.Arguments[0]);

                        generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Log", new Type[] { typeof(double) }));
                        break;
                    case FunctionType.Log10:
                        GenerateMethodBody(generator, function.Arguments[0]);
                        
                        generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Log10"));
                        break;
                    case FunctionType.Logn:
                        GenerateMethodBody(generator, function.Arguments[0]);
                        GenerateMethodBody(generator, function.Arguments[1]);

                        generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Log", new Type[] { typeof(double), typeof(double) }));
                        break;
                    case FunctionType.SquareRoot:
                        GenerateMethodBody(generator, function.Arguments[0]);

                        generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Sqrt", new Type[] { typeof(double) }));
                        break;
                    default:
                        throw new ArgumentException(string.Format("Unsupported function \"{0}\".", function.FunctionType), "operation");
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }
    }
#else
    public class DynamicCompiler : IExecutor
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
            return BuildFunction(operation)(variables);
        }

        public Func<Dictionary<string, double>, double> BuildFunction(Operation operation)
        {
            ParameterExpression dictionaryParameter = 
                Expression.Parameter(typeof(Dictionary<string, double>), "dictionary");

            LabelTarget returnLabel = Expression.Label(typeof(double));

            return Expression.Lambda<Func<Dictionary<string, double>, double>>(
                Expression.Block(
                    Expression.Return(returnLabel, GenerateMethodBody(operation, dictionaryParameter)),
                    Expression.Label(returnLabel, Expression.Constant(0.0))
                ),
                dictionaryParameter
            ).Compile();
        }

        private Expression GenerateMethodBody(Operation operation, ParameterExpression dictionaryParameter)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            if (operation.GetType() == typeof(IntegerConstant))
            {
                IntegerConstant constant = (IntegerConstant)operation;

                return Expression.Convert(Expression.Constant(constant.Value, typeof(int)), typeof(double));
            }
            else if (operation.GetType() == typeof(FloatingPointConstant))
            {
                FloatingPointConstant constant = (FloatingPointConstant)operation;

                return Expression.Constant(constant.Value, typeof(double));
            }
            else if (operation.GetType() == typeof(Variable))
            {
                Type dictionaryType = typeof(Dictionary<string, double>);

                Variable variable = (Variable)operation;

                Expression isInDictionaryExpression = Expression.Call(dictionaryParameter, 
                    dictionaryType.GetRuntimeMethod("ContainsKey", new Type[] { typeof(string) }),
                    Expression.Constant(variable.Name));

                Expression throwException = Expression.Throw(
                    Expression.New(typeof(VariableNotDefinedException).GetConstructor(new Type[] { typeof(string) }),
                        Expression.Constant(string.Format("The variable \"{0}\" used is not defined.", variable.Name))));

                LabelTarget returnLabel = Expression.Label(typeof(double));

                return Expression.Block(
                    Expression.IfThenElse(
                        isInDictionaryExpression,
                        Expression.Return(returnLabel, Expression.Call(dictionaryParameter, 
                            dictionaryType.GetRuntimeMethod("get_Item", new Type[] { typeof(string) }), 
                            Expression.Constant(variable.Name))),
                        throwException
                    ),
                    Expression.Label(returnLabel, Expression.Constant(0.0))
                );
            }
            else if (operation.GetType() == typeof(Multiplication))
            {
                Multiplication multiplication = (Multiplication)operation;
                Expression argument1 = GenerateMethodBody(multiplication.Argument1, dictionaryParameter);
                Expression argument2 = GenerateMethodBody(multiplication.Argument2, dictionaryParameter);

                return Expression.Multiply(argument1, argument2);
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition)operation;
                Expression argument1 = GenerateMethodBody(addition.Argument1, dictionaryParameter);
                Expression argument2 = GenerateMethodBody(addition.Argument2, dictionaryParameter);

                return Expression.Add(argument1, argument2);
            }
            else if (operation.GetType() == typeof(Subtraction))
            {
                Subtraction addition = (Subtraction)operation;
                Expression argument1 = GenerateMethodBody(addition.Argument1, dictionaryParameter);
                Expression argument2 = GenerateMethodBody(addition.Argument2, dictionaryParameter);

                return Expression.Subtract(argument1, argument2);
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division)operation;
                Expression dividend = GenerateMethodBody(division.Dividend, dictionaryParameter);
                Expression divisor = GenerateMethodBody(division.Divisor, dictionaryParameter);

                return Expression.Divide(dividend, divisor);
            }
            else if (operation.GetType() == typeof(Modulo))
            {
                Modulo modulo = (Modulo)operation;
                Expression dividend = GenerateMethodBody(modulo.Dividend, dictionaryParameter);
                Expression divisor = GenerateMethodBody(modulo.Divisor, dictionaryParameter);

                return Expression.Modulo(dividend, divisor);
            }
            else if (operation.GetType() == typeof(Exponentiation))
            {
                Exponentiation exponentation = (Exponentiation)operation;
                Expression @base = GenerateMethodBody(exponentation.Base, dictionaryParameter);
                Expression exponent = GenerateMethodBody(exponentation.Exponent, dictionaryParameter);

                return Expression.Call(null, typeof(Math).GetRuntimeMethod("Pow", new Type[0]), @base, exponent);
            }
            else if (operation.GetType() == typeof(Function))
            {
                Function function = (Function)operation;

                Expression argument1;
                Expression argument2;
                switch (function.FunctionType)
                {
                    case FunctionType.Sine:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);

                        return Expression.Call(null, typeof(Math).GetRuntimeMethod("Sin", new Type[] { typeof(double) }), argument1);
                    case FunctionType.Cosine:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);

                        return Expression.Call(null, typeof(Math).GetRuntimeMethod("Cos", new Type[] { typeof(double) }), argument1);
                    case FunctionType.Arcsine:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);

                        return Expression.Call(null, typeof(Math).GetRuntimeMethod("Asin", new Type[] { typeof(double) }), argument1);
                    case FunctionType.Arccosine:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);

                        return Expression.Call(null, typeof(Math).GetRuntimeMethod("Acos", new Type[] { typeof(double) }), argument1);
                    case FunctionType.Tangent:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);

                        return Expression.Call(null, typeof(Math).GetRuntimeMethod("Tan", new Type[] { typeof(double) }), argument1);
                    case FunctionType.Cotangent:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);

                        return Expression.Call(null, typeof(MathUtil).GetRuntimeMethod("Cot", new Type[] { typeof(double) }), argument1);
                    case FunctionType.Arctangent:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);

                        return Expression.Call(null, typeof(Math).GetRuntimeMethod("Atan", new Type[] { typeof(double) }), argument1);
                    case FunctionType.Arccotangent:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);

                        return Expression.Call(null, typeof(MathUtil).GetRuntimeMethod("Acot", new Type[] { typeof(double) }), argument1);
                    case FunctionType.Loge:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);

                        return Expression.Call(null, typeof(Math).GetRuntimeMethod("Log", new Type[] { typeof(double) }), argument1);
                    case FunctionType.Log10:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);

                        return Expression.Call(null, typeof(Math).GetRuntimeMethod("Log10", new Type[] { typeof(double) }), argument1);
                    case FunctionType.Logn:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);
                        argument2 = GenerateMethodBody(function.Arguments[1], dictionaryParameter);

                        return Expression.Call(null, 
                            typeof(Math).GetRuntimeMethod("Log", new Type[] { typeof(double), typeof(double) }), 
                            argument1, 
                            argument2);
                    case FunctionType.SquareRoot:
                        argument1 = GenerateMethodBody(function.Arguments[0], dictionaryParameter);

                        return Expression.Call(null, typeof(Math).GetRuntimeMethod("Sqrt", new Type[] { typeof(double) }), argument1);
                    default:
                        throw new ArgumentException(string.Format("Unsupported function \"{0}\".", function.FunctionType), "operation");
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }
    }
#endif
}
