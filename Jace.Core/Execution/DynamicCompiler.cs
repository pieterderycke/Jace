using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Jace.Operations;
using Jace.Util;

namespace Jace.Execution
{
#if !NETFX_CORE
    public class DynamicCompiler : IExecutor
    {
        public double Execute(Operation operation, IFunctionRegistry functionRegistry)
        {
            return Execute(operation, functionRegistry, new Dictionary<string, double>());
        }

        public double Execute(Operation operation, IFunctionRegistry functionRegistry, 
            IDictionary<string, double> variables)
        {
            return BuildFormula(operation, functionRegistry)(variables);
        }

        public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
            IFunctionRegistry functionRegistry)
        {
            Func<FormulaContext, double> func = BuildFormulaInternal(operation, functionRegistry);
            return variables =>
                {
                    variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
                    FormulaContext context = new FormulaContext(variables, functionRegistry);
                    return func(context);
                };
        }

        private Func<FormulaContext, double> BuildFormulaInternal(Operation operation,
            IFunctionRegistry functionRegistry)
        {
            DynamicMethod method = new DynamicMethod("MyCalcMethod", typeof(double),
                new Type[] { typeof(FormulaContext) });
            GenerateMethodBody(method, operation, functionRegistry);

            Func<FormulaContext, double> function =
                (Func<FormulaContext, double>)method.CreateDelegate(typeof(Func<FormulaContext, double>));

            return function;
        }

        private void GenerateMethodBody(DynamicMethod method, Operation operation, 
            IFunctionRegistry functionRegistry)
        {
            ILGenerator generator = method.GetILGenerator();
            generator.DeclareLocal(typeof(double));
            generator.DeclareLocal(typeof(object[]));
            GenerateMethodBody(generator, operation, functionRegistry);
            generator.Emit(OpCodes.Ret);
        }

        private void GenerateMethodBody(ILGenerator generator, Operation operation, 
            IFunctionRegistry functionRegistry)
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
                Type dictionaryType = typeof(IDictionary<string, double>);

                Variable variable = (Variable)operation;

                Label throwExceptionLabel = generator.DefineLabel();
                Label returnLabel = generator.DefineLabel();

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Callvirt, typeof(FormulaContext).GetProperty("Variables").GetGetMethod());
                generator.Emit(OpCodes.Ldstr, variable.Name);
                generator.Emit(OpCodes.Ldloca_S, (byte)0);
                generator.Emit(OpCodes.Callvirt, dictionaryType.GetMethod("TryGetValue", new Type[] { typeof(string), typeof(double).MakeByRefType() }));
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ceq);
                generator.Emit(OpCodes.Brtrue_S, throwExceptionLabel);

                generator.Emit(OpCodes.Ldloc_0);
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
                GenerateMethodBody(generator, multiplication.Argument1, functionRegistry);
                GenerateMethodBody(generator, multiplication.Argument2, functionRegistry);

                generator.Emit(OpCodes.Mul);
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition)operation;
                GenerateMethodBody(generator, addition.Argument1, functionRegistry);
                GenerateMethodBody(generator, addition.Argument2, functionRegistry);

                generator.Emit(OpCodes.Add);
            }
            else if (operation.GetType() == typeof(Subtraction))
            {
                Subtraction addition = (Subtraction)operation;
                GenerateMethodBody(generator, addition.Argument1, functionRegistry);
                GenerateMethodBody(generator, addition.Argument2, functionRegistry);

                generator.Emit(OpCodes.Sub);
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division)operation;
                GenerateMethodBody(generator, division.Dividend, functionRegistry);
                GenerateMethodBody(generator, division.Divisor, functionRegistry);

                generator.Emit(OpCodes.Div);
            }
            else if (operation.GetType() == typeof(Modulo))
            {
                Modulo modulo = (Modulo)operation;
                GenerateMethodBody(generator, modulo.Dividend, functionRegistry);
                GenerateMethodBody(generator, modulo.Divisor, functionRegistry);

                generator.Emit(OpCodes.Rem);
            }
            else if (operation.GetType() == typeof(Exponentiation))
            {
                Exponentiation exponentation = (Exponentiation)operation;
                GenerateMethodBody(generator, exponentation.Base, functionRegistry);
                GenerateMethodBody(generator, exponentation.Exponent, functionRegistry);

                generator.Emit(OpCodes.Call, typeof(Math).GetMethod("Pow"));
            }
            else if (operation.GetType() == typeof(UnaryMinus))
            {
                UnaryMinus unaryMinus = (UnaryMinus)operation;
                GenerateMethodBody(generator, unaryMinus.Argument, functionRegistry);

                generator.Emit(OpCodes.Neg);
            }
            else if (operation.GetType() == typeof(LessThan))
            {
                LessThan lessThan = (LessThan)operation;

                Label ifLabel = generator.DefineLabel();
                Label endLabel = generator.DefineLabel();

                GenerateMethodBody(generator, lessThan.Argument1, functionRegistry);
                GenerateMethodBody(generator, lessThan.Argument2, functionRegistry);

                generator.Emit(OpCodes.Blt_S, ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 0.0);
                generator.Emit(OpCodes.Br_S, endLabel);
                generator.MarkLabel(ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 1.0);
                generator.MarkLabel(endLabel);
            }
            else if (operation.GetType() == typeof(LessOrEqualThan))
            {
                LessOrEqualThan lessOrEqualThan = (LessOrEqualThan)operation;

                Label ifLabel = generator.DefineLabel();
                Label endLabel = generator.DefineLabel();

                GenerateMethodBody(generator, lessOrEqualThan.Argument1, functionRegistry);
                GenerateMethodBody(generator, lessOrEqualThan.Argument2, functionRegistry);

                generator.Emit(OpCodes.Ble_S, ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 0.0);
                generator.Emit(OpCodes.Br_S, endLabel);
                generator.MarkLabel(ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 1.0);
                generator.MarkLabel(endLabel);
            }
            else if (operation.GetType() == typeof(GreaterThan))
            {
                GreaterThan greaterThan = (GreaterThan)operation;

                Label ifLabel = generator.DefineLabel();
                Label endLabel = generator.DefineLabel();

                GenerateMethodBody(generator, greaterThan.Argument1, functionRegistry);
                GenerateMethodBody(generator, greaterThan.Argument2, functionRegistry);

                generator.Emit(OpCodes.Bgt_S, ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 0.0);
                generator.Emit(OpCodes.Br_S, endLabel);
                generator.MarkLabel(ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 1.0);
                generator.MarkLabel(endLabel);
            }
            else if (operation.GetType() == typeof(GreaterOrEqualThan))
            {
                GreaterOrEqualThan greaterOrEqualThan = (GreaterOrEqualThan)operation;

                Label ifLabel = generator.DefineLabel();
                Label endLabel = generator.DefineLabel();

                GenerateMethodBody(generator, greaterOrEqualThan.Argument1, functionRegistry);
                GenerateMethodBody(generator, greaterOrEqualThan.Argument2, functionRegistry);

                generator.Emit(OpCodes.Bge_S, ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 0.0);
                generator.Emit(OpCodes.Br_S, endLabel);
                generator.MarkLabel(ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 1.0);
                generator.MarkLabel(endLabel);
            }
            else if (operation.GetType() == typeof(Equal))
            {
                Equal equal = (Equal)operation;

                Label ifLabel = generator.DefineLabel();
                Label endLabel = generator.DefineLabel();

                GenerateMethodBody(generator, equal.Argument1, functionRegistry);
                GenerateMethodBody(generator, equal.Argument2, functionRegistry);

                generator.Emit(OpCodes.Beq_S, ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 0.0);
                generator.Emit(OpCodes.Br_S, endLabel);
                generator.MarkLabel(ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 1.0);
                generator.MarkLabel(endLabel);
            }
            else if (operation.GetType() == typeof(NotEqual))
            {
                NotEqual notEqual = (NotEqual)operation;

                Label ifLabel = generator.DefineLabel();
                Label endLabel = generator.DefineLabel();

                GenerateMethodBody(generator, notEqual.Argument1, functionRegistry);
                GenerateMethodBody(generator, notEqual.Argument2, functionRegistry);

                generator.Emit(OpCodes.Beq, ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 1.0);
                generator.Emit(OpCodes.Br_S, endLabel);
                generator.MarkLabel(ifLabel);
                generator.Emit(OpCodes.Ldc_R8, 0.0);
                generator.MarkLabel(endLabel);
            }
            else if (operation.GetType() == typeof(Function))
            {
                Function function = (Function)operation;

                FunctionInfo functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);
                Type funcType = GetFuncType(functionInfo.NumberOfParameters);

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Callvirt, typeof(FormulaContext).GetProperty("FunctionRegistry").GetGetMethod());
                generator.Emit(OpCodes.Ldstr, function.FunctionName);
                generator.Emit(OpCodes.Callvirt, typeof(IFunctionRegistry).GetMethod("GetFunctionInfo", new Type[] { typeof(string) }));
                generator.Emit(OpCodes.Callvirt, typeof(FunctionInfo).GetProperty("Function").GetGetMethod());
                generator.Emit(OpCodes.Castclass, funcType);

                for (int i = 0; i < functionInfo.NumberOfParameters; i++)
                    GenerateMethodBody(generator, function.Arguments[i], functionRegistry);

                generator.Emit(OpCodes.Call, funcType.GetMethod("Invoke"));
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }

        private Type GetFuncType(int numberOfParameters)
        {
            string funcTypeName = string.Format("System.Func`{0}", numberOfParameters + 1);
            Type funcType = Type.GetType(funcTypeName);

            Type[] typeArguments = new Type[numberOfParameters + 1];
            for (int i = 0; i < typeArguments.Length; i++)
                typeArguments[i] = typeof(double);

            return funcType.MakeGenericType(typeArguments);
        }
    }
#else
    public class DynamicCompiler : IExecutor
    {
        public double Execute(Operation operation, IFunctionRegistry functionRegistry)
        {
            return Execute(operation, functionRegistry, new Dictionary<string, double>());
        }

        public double Execute(Operation operation, IFunctionRegistry functionRegistry, 
            IDictionary<string, double> variables)
        {
            return BuildFormula(operation, functionRegistry)(variables);
        }

        public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
            IFunctionRegistry functionRegistry)
        {
            Func<FormulaContext, double> func = BuildFormulaInternal(operation, functionRegistry);
            return variables =>
                {
                    variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
                    FormulaContext context = new FormulaContext(variables, functionRegistry);
                    return func(context);
                };
        }

        private Func<FormulaContext, double> BuildFormulaInternal(Operation operation, 
            IFunctionRegistry functionRegistry)
        {
            ParameterExpression contextParameter = Expression.Parameter(typeof(FormulaContext), "context");

            LabelTarget returnLabel = Expression.Label(typeof(double));

            return Expression.Lambda<Func<FormulaContext, double>>(
                Expression.Block(
                    Expression.Return(returnLabel, GenerateMethodBody(operation, contextParameter, functionRegistry)),
                    Expression.Label(returnLabel, Expression.Constant(0.0))
                ),
                contextParameter
            ).Compile();
        }

        private Expression GenerateMethodBody(Operation operation, ParameterExpression contextParameter,
            IFunctionRegistry functionRegistry)
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
                Type contextType = typeof(FormulaContext);
                Type dictionaryType = typeof(IDictionary<string, double>);

                Variable variable = (Variable)operation;

                Expression getVariables = Expression.Property(contextParameter, "Variables");
                ParameterExpression value = Expression.Variable(typeof(double), "value");

                Expression variableFound = Expression.Call(getVariables,
                    dictionaryType.GetRuntimeMethod("TryGetValue", new Type[] { typeof(string), typeof(double).MakeByRefType() }),
                    Expression.Constant(variable.Name),
                    value);

                Expression throwException = Expression.Throw(
                    Expression.New(typeof(VariableNotDefinedException).GetConstructor(new Type[] { typeof(string) }),
                        Expression.Constant(string.Format("The variable \"{0}\" used is not defined.", variable.Name))));

                LabelTarget returnLabel = Expression.Label(typeof(double));

                return Expression.Block(
                    new[] { value },
                    Expression.IfThenElse(
                        variableFound,
                        Expression.Return(returnLabel, value),
                        throwException
                    ),
                    Expression.Label(returnLabel, Expression.Constant(0.0))
                );
            }
            else if (operation.GetType() == typeof(Multiplication))
            {
                Multiplication multiplication = (Multiplication)operation;
                Expression argument1 = GenerateMethodBody(multiplication.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(multiplication.Argument2, contextParameter, functionRegistry);

                return Expression.Multiply(argument1, argument2);
            }
            else if (operation.GetType() == typeof(Addition))
            {
                Addition addition = (Addition)operation;
                Expression argument1 = GenerateMethodBody(addition.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(addition.Argument2, contextParameter, functionRegistry);

                return Expression.Add(argument1, argument2);
            }
            else if (operation.GetType() == typeof(Subtraction))
            {
                Subtraction addition = (Subtraction)operation;
                Expression argument1 = GenerateMethodBody(addition.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(addition.Argument2, contextParameter, functionRegistry);

                return Expression.Subtract(argument1, argument2);
            }
            else if (operation.GetType() == typeof(Division))
            {
                Division division = (Division)operation;
                Expression dividend = GenerateMethodBody(division.Dividend, contextParameter, functionRegistry);
                Expression divisor = GenerateMethodBody(division.Divisor, contextParameter, functionRegistry);

                return Expression.Divide(dividend, divisor);
            }
            else if (operation.GetType() == typeof(Modulo))
            {
                Modulo modulo = (Modulo)operation;
                Expression dividend = GenerateMethodBody(modulo.Dividend, contextParameter, functionRegistry);
                Expression divisor = GenerateMethodBody(modulo.Divisor, contextParameter, functionRegistry);

                return Expression.Modulo(dividend, divisor);
            }
            else if (operation.GetType() == typeof(Exponentiation))
            {
                Exponentiation exponentation = (Exponentiation)operation;
                Expression @base = GenerateMethodBody(exponentation.Base, contextParameter, functionRegistry);
                Expression exponent = GenerateMethodBody(exponentation.Exponent, contextParameter, functionRegistry);

                return Expression.Call(null, typeof(Math).GetRuntimeMethod("Pow", new Type[] { typeof(double), typeof(double) }), @base, exponent);
            }
            else if (operation.GetType() == typeof(UnaryMinus))
            {
                UnaryMinus unaryMinus = (UnaryMinus)operation;
                Expression argument = GenerateMethodBody(unaryMinus.Argument, contextParameter, functionRegistry);
                return Expression.Negate(argument);
            }
            else if (operation.GetType() == typeof(LessThan))
            {
                LessThan lessThan = (LessThan)operation;
                Expression argument1 = GenerateMethodBody(lessThan.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(lessThan.Argument2, contextParameter, functionRegistry);

                return Expression.Condition(Expression.LessThan(argument1, argument2),
                    Expression.Constant(1.0),
                    Expression.Constant(0.0));
            }
            else if (operation.GetType() == typeof(LessOrEqualThan))
            {
                LessOrEqualThan lessOrEqualThan = (LessOrEqualThan)operation;
                Expression argument1 = GenerateMethodBody(lessOrEqualThan.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(lessOrEqualThan.Argument2, contextParameter, functionRegistry);

                return Expression.Condition(Expression.LessThanOrEqual(argument1, argument2),
                    Expression.Constant(1.0),
                    Expression.Constant(0.0));
            }
            else if (operation.GetType() == typeof(GreaterThan))
            {
                GreaterThan greaterThan = (GreaterThan)operation;
                Expression argument1 = GenerateMethodBody(greaterThan.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(greaterThan.Argument2, contextParameter, functionRegistry);

                return Expression.Condition(Expression.GreaterThan(argument1, argument2),
                    Expression.Constant(1.0),
                    Expression.Constant(0.0));
            }
            else if (operation.GetType() == typeof(GreaterOrEqualThan))
            {
                GreaterOrEqualThan greaterOrEqualThan = (GreaterOrEqualThan)operation;
                Expression argument1 = GenerateMethodBody(greaterOrEqualThan.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(greaterOrEqualThan.Argument2, contextParameter, functionRegistry);

                return Expression.Condition(Expression.GreaterThanOrEqual(argument1, argument2),
                    Expression.Constant(1.0),
                    Expression.Constant(0.0));
            }
            else if (operation.GetType() == typeof(Equal))
            {
                Equal equal = (Equal)operation;
                Expression argument1 = GenerateMethodBody(equal.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(equal.Argument2, contextParameter, functionRegistry);

                return Expression.Condition(Expression.Equal(argument1, argument2),
                    Expression.Constant(1.0),
                    Expression.Constant(0.0));
            }
            else if (operation.GetType() == typeof(NotEqual))
            {
                NotEqual notEqual = (NotEqual)operation;
                Expression argument1 = GenerateMethodBody(notEqual.Argument1, contextParameter, functionRegistry);
                Expression argument2 = GenerateMethodBody(notEqual.Argument2, contextParameter, functionRegistry);

                return Expression.Condition(Expression.NotEqual(argument1, argument2),
                    Expression.Constant(1.0),
                    Expression.Constant(0.0));
            }
            else if (operation.GetType() == typeof(Function))
            {
                Function function = (Function)operation;

                FunctionInfo functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);
                Type funcType = GetFuncType(functionInfo.NumberOfParameters);
                Type[] parameterTypes = (from i in Enumerable.Range(0, functionInfo.NumberOfParameters)
                                            select typeof(double)).ToArray();

                Expression[] arguments = new Expression[functionInfo.NumberOfParameters];
                for (int i = 0; i < functionInfo.NumberOfParameters; i++)
                    arguments[i] = GenerateMethodBody(function.Arguments[i], contextParameter, functionRegistry);

                Expression getFunctionRegistry = Expression.Property(contextParameter, "FunctionRegistry");

                ParameterExpression functionInfoVariable = Expression.Variable(typeof(FunctionInfo));

                return Expression.Block(
                    new[] { functionInfoVariable },
                    Expression.Assign(
                        functionInfoVariable,
                        Expression.Call(getFunctionRegistry, typeof(IFunctionRegistry).GetRuntimeMethod("GetFunctionInfo", new Type[] { typeof(string) }), Expression.Constant(function.FunctionName))
                    ),
                    Expression.Call(
                        Expression.Convert(Expression.Property(functionInfoVariable, "Function"), funcType),
                        funcType.GetRuntimeMethod("Invoke", parameterTypes),
                        arguments));
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported operation \"{0}\".", operation.GetType().FullName), "operation");
            }
        }

        private Type GetFuncType(int numberOfParameters)
        {
            string funcTypeName = string.Format("System.Func`{0}", numberOfParameters + 1);
            Type funcType = Type.GetType(funcTypeName);

            Type[] typeArguments = new Type[numberOfParameters + 1];
            for (int i = 0; i < typeArguments.Length; i++)
                typeArguments[i] = typeof(double);

            return funcType.MakeGenericType(typeArguments);
        }
    }
#endif
}
