using System;
using System.Threading;

namespace task14;

public static class DefiniteIntegral
{
    public static double Solve(
        double a,
        double b,
        Func<double, double> function,
        double step,
        int threadsNumber)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        if (step <= 0)
            throw new ArgumentOutOfRangeException(nameof(step));

        if (threadsNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(threadsNumber));

        if (a == b)
            return 0.0;

        if (b < a)
        {
            return -Solve(
                b,
                a,
                function,
                step,
                threadsNumber);
        }

        var segments =
            (long)Math.Ceiling((b - a) / step);

        var actualStep =
            (b - a) / segments;

        var partialSums =
            new double[threadsNumber];

        var barrier =
            new Barrier(threadsNumber);

        var threads =
            new Thread[threadsNumber];

        for (
            int threadIndex = 0;
            threadIndex < threadsNumber;
            threadIndex++)
        {
            var localIndex = threadIndex;

            threads[localIndex] =
                new Thread(() =>
                {
                    var startSegment =
                        segments
                        * localIndex
                        / threadsNumber;

                    var endSegment =
                        segments
                        * (localIndex + 1)
                        / threadsNumber;

                    var localResult = 0.0;

                    for (
                        long segment = startSegment;
                        segment < endSegment;
                        segment++)
                    {
                        var x1 =
                            a + segment * actualStep;

                        var x2 =
                            x1 + actualStep;

                        localResult +=
                            (
                                function(x1)
                                + function(x2)
                            )
                            * actualStep
                            / 2.0;
                    }

                    partialSums[localIndex] =
                        localResult;

                    barrier.SignalAndWait();
                });

            threads[localIndex].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        barrier.Dispose();

        var result = 0.0;

        for (
            int i = 0;
            i < partialSums.Length;
            i++)
        {
            result += partialSums[i];
        }

        return result;
    }
}