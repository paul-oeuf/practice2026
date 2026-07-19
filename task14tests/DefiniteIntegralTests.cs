using task14;
using Xunit;

namespace task14tests;

public class DefiniteIntegralTests
{
    [Fact]
    public void Solve_LinearFunctionOnSymmetricInterval_ReturnsZero()
    {
        var result =
            DefiniteIntegral.Solve(
                -1,
                1,
                x => x,
                1e-4,
                2);

        Assert.Equal(
            0.0,
            result,
            4);
    }

    [Fact]
    public void Solve_SinOnSymmetricInterval_ReturnsZero()
    {
        var result =
            DefiniteIntegral.Solve(
                -1,
                1,
                Math.Sin,
                1e-5,
                8);

        Assert.Equal(
            0.0,
            result,
            4);
    }

    [Fact]
    public void Solve_LinearFunction_ReturnsCorrectIntegral()
    {
        var result =
            DefiniteIntegral.Solve(
                0,
                5,
                x => x,
                1e-6,
                8);

        Assert.Equal(
            12.5,
            result,
            5);
    }

    [Fact]
    public void SingleThreadSolve_ReturnsCorrectIntegral()
    {
        var result =
            SingleThreadDefiniteIntegral.Solve(
                0,
                5,
                x => x,
                1e-6);

        Assert.Equal(
            12.5,
            result,
            5);
    }
}