using task14;

namespace task14tests;

public class DefiniteIntegralTests
{
    [Fact]
    public void Solve_LinearFunctionOnSymmetricInterval_ReturnsZero()
    {
        var result = DefiniteIntegral.Solve(
            -1,
            1,
            x => x,
            1e-4,
            2);

        Assert.Equal(
            0,
            result,
            1e-4);
    }

    [Fact]
    public void Solve_SineOnSymmetricInterval_ReturnsZero()
    {
        var result = DefiniteIntegral.Solve(
            -1,
            1,
            Math.Sin,
            1e-5,
            8);

        Assert.Equal(
            0,
            result,
            1e-4);
    }

    [Fact]
    public void Solve_LinearFunction_ReturnsCorrectIntegral()
    {
        var result = DefiniteIntegral.Solve(
            0,
            5,
            x => x,
            1e-6,
            8);

        Assert.Equal(
            12.5,
            result,
            1e-5);
    }

    [Fact]
    public void Solve_ReversedInterval_ReturnsNegativeIntegral()
    {
        var result = DefiniteIntegral.Solve(
            5,
            0,
            x => x,
            1e-6,
            8);

        Assert.Equal(
            -12.5,
            result,
            1e-5);
    }

    [Fact]
    public void Solve_ZeroLengthInterval_ReturnsZero()
    {
        var result = DefiniteIntegral.Solve(
            5,
            5,
            x => x,
            1e-6,
            8);

        Assert.Equal(
            0,
            result);
    }

    [Fact]
    public void Solve_InvalidStep_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => DefiniteIntegral.Solve(
                0,
                1,
                x => x,
                0,
                2));
    }

    [Fact]
    public void Solve_InvalidThreadCount_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => DefiniteIntegral.Solve(
                0,
                1,
                x => x,
                1e-4,
                0));
    }
}