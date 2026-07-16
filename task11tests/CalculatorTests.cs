using task11;
using Xunit;

namespace task11tests;

public class CalculatorTests
{
    [Fact]
    public void Calculator_ShouldBeCreatedDynamically()
    {
        var calculator = CalculatorCompiler.CreateCalculator();

        Assert.NotNull(calculator);
        Assert.IsAssignableFrom<ICalculator>(calculator);
    }


    [Fact]
    public void Add_ShouldReturnCorrectResult()
    {
        var calculator = CalculatorCompiler.CreateCalculator();

        var result = calculator.Add(10, 5);

        Assert.Equal(15, result);
    }


    [Fact]
    public void Minus_ShouldReturnCorrectResult()
    {
        var calculator = CalculatorCompiler.CreateCalculator();

        var result = calculator.Minus(10, 5);

        Assert.Equal(5, result);
    }


    [Fact]
    public void Mul_ShouldReturnCorrectResult()
    {
        var calculator = CalculatorCompiler.CreateCalculator();

        var result = calculator.Mul(10, 5);

        Assert.Equal(50, result);
    }


    [Fact]
    public void Div_ShouldReturnCorrectResult()
    {
        var calculator = CalculatorCompiler.CreateCalculator();

        var result = calculator.Div(10, 5);

        Assert.Equal(2, result);
    }


    [Fact]
    public void Div_ByZero_ShouldThrowException()
    {
        var calculator = CalculatorCompiler.CreateCalculator();

        Assert.Throws<DivideByZeroException>(
            () => calculator.Div(10, 0));
    }
}