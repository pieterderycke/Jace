# Jace.NET
Jace.NET is a high performance calculation engine for the .NET platform. It stands for "Just Another Calculation Engine".

## What does it do?
Jace.NET can interprete and execute strings containing mathematical functions. These functions can rely on variables. If variables are used, values can be provided for these variables at execution time of the mathematical function.

Jace can execute functions in two modes: in interpreted mode and in a dynamic compilation mode. If dynamic compilation mode is used, Jace will create a dynamic method at runtime and will generate the necessary MSIL opcodes for native execution of the function. If a function is re-executed with other variables, Jace will take the dynamically generated method from its cache. It is recommended to use Jace in dynamic compilation mode.

## Architecture
Jace.NET follows a design similar to most of the modern compilers. Interpretation and execution is done in a number of phases:

### Tokenizing
During the tokenizing phase, the string is converted into the different kind of tokens: variables, operators and constants.
### Abstract Syntax Tree Creation
During the abstract syntax tree creation phase, the tokenized input is converted into a hierarchical tree representing the mathematically function. This tree unambiguously stores the mathematical calculations that must be executed.
### Optimization
During the optimization phase, the abstract syntax tree is optimized for executing.
### Interpreted Execution/Dynamic Compilation
In this phase the abstract syntax tree is executed in either interpreted mode or in dynamic compilation mode.

## Examples
Jace.NET can be used in a couple of ways:

To directly execute a given mathematical function using the provided variables:
```csharp
Dictionary<string, double> variables = new Dictionary<string, double>();
variables.Add("var1", 2.5);
variables.Add("var2", 3.4);

CalculationEngine engine = new CalculationEngine();
double result = engine.Calculate("var1*var2", variables);
```

To build a .NET Func accepting a dictionary as input containing the values for each variable:
```csharp
CalculationEngine engine = new CalculationEngine();
Func<Dictionary<string, double>, double> function = engine.Build("var1+2/(3*otherVariable)");

Dictionary<string, double> variables = new Dictionary<string, double>();
variables.Add("var1", 2);
variables.Add("otherVariable", 4.2);
	
double result = function(2, 4.2);
```

To build a typed .NET Func:
```csharp
CalculationEngine engine = new CalculationEngine();
Func<int, double, double> function = (Func<int, double, double>)engine.Function("var1+2/(3*otherVariable)")
	.Parameter("var1", DataType.Integer)
    .Parameter("otherVariable", DataType.FloatingPoint)
    .Result(DataType.FloatingPoint)
    .Build();
	
double result = function(2, 4.2);
```

## Performance
Below you can find the results of Jace.NET benchmark that show its high performance calculation engine. Tests were done on an Intel i7 2640M laptop.
1000 random functions were generated, each containing 3 variables and a number of constants (a mix of integers and floating point numbers). Each random generated function was executed 10 000 times. So in total 10 000 000 calculations are done during the benchmark. You can find the benchmark application in "Jace.Benchmark" if you want to run it on your system.

Calculation Mode:
* Interpreted: 00:00:06.7860119
* Compiled: 00:00:02.5584045