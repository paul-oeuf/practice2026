using System;

namespace task14;

public static class SingleThreadDefiniteIntegral
{
    public static double Solve(
        double a,
        double b,
        Func<double, double> function,
        double step)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        if (step <= 0)
            throw new ArgumentOutOfRangeException(nameof(step));

        if (a == b)
            return 0.0;

        if (b < a)
        {
            return -Solve(
                b,
                a,
                function,
                step);
        }

        var segments =
            (long)Math.Ceiling((b - a) / step);

        var actualStep =
            (b - a) / segments;

        var result = 0.0;

        for (
            long segment = 0;
            segment < segments;
            segment++)
        {
            var x1 =
                a + segment * actualStep;

            var x2 =
                x1 + actualStep;

            result +=
                (
                    function(x1)
                    + function(x2)
                )
                * actualStep
                / 2.0;
        }

        return result;
    }
}