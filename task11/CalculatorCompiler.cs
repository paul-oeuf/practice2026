using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11;

public static class CalculatorCompiler
{
    private static readonly string CalculatorCode =
        """
        using task11;

        public class Calculator : ICalculator
        {
            public int Add(int a, int b) => a + b;

            public int Minus(int a, int b) => a - b;

            public int Mul(int a, int b) => a * b;

            public int Div(int a, int b) => a / b;
        }
        """;


    public static ICalculator CreateCalculator()
    {
        var syntaxTree =
            CSharpSyntaxTree.ParseText(CalculatorCode);


        var references = new[]
        {
            MetadataReference.CreateFromFile(
                typeof(object).Assembly.Location),

            MetadataReference.CreateFromFile(
                typeof(ICalculator).Assembly.Location),

            MetadataReference.CreateFromFile(
                typeof(Console).Assembly.Location)
        };


        var compilation =
            CSharpCompilation.Create(
                "DynamicCalculator",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary));


        using var stream = new MemoryStream();


        var result =
            compilation.Emit(stream);


        if (!result.Success)
        {
            var errors =
                string.Join(
                    "\n",
                    result.Diagnostics
                        .Where(x =>
                            x.Severity ==
                            DiagnosticSeverity.Error));


            throw new Exception(errors);
        }


        stream.Seek(0, SeekOrigin.Begin);


        var assembly =
            Assembly.Load(
                stream.ToArray());


        var calculatorType =
            assembly.GetType("Calculator");


        if (calculatorType == null)
        {
            throw new Exception(
                "Calculator class was not found");
        }


        var instance =
            Activator.CreateInstance(
                calculatorType);


        if (instance is not ICalculator calculator)
        {
            throw new Exception(
                "Generated class does not implement ICalculator");
        }


        return calculator;
    }
}