using Jace.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jace
{
    public interface ICalculationEngine<T>
    {

        IFunctionRegistry<T> FunctionRegistry { get; }

        IConstantRegistry<T> ConstantRegistry { get; }

        IEnumerable<FunctionInfo> Functions { get; }

        IEnumerable<ConstantInfo<T>> Constants { get; }

        T Calculate(string formulaText);

        T Calculate(string formulaText, IDictionary<string, T> variables);

        FormulaBuilder<T> Formula(string formulaText);

        Func<IDictionary<string, T>, T> Build(string formulaText);

        Func<IDictionary<string, T>, T> Build(string formulaText, IDictionary<string,T> constants);

        void AddConstant(string constantName, T value);
        
        void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T, T> function, bool isIdempotent = true);
        void AddFunction(string functionName, Func<T> function, bool isIdempotent = true);

        void AddFunction(string functionName, DynamicFunc<T, T> functionDelegate, bool isIdempotent = true);
        
    }

    public interface IInternalCalculationEngine<T> : ICalculationEngine<T>
    {
        void VerifyVariableNames(IDictionary<string, T> variables);

        IExecutor<T> CreateDynamicCompiler(bool? caseSensitive);

        IExecutor<T> CreateInterpreter(bool? caseSensitive);

        void RegisterDefaultConstants();

        void RegisterDefaultFunctions();
    }

}
