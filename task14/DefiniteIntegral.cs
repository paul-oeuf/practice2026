using System.Collections.Concurrent;
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
        ArgumentNullException.ThrowIfNull(function);

        if (!double.IsFinite(a))
        {
            throw new ArgumentOutOfRangeException(
                nameof(a),
                "Левая граница должна быть конечным числом.");
        }

        if (!double.IsFinite(b))
        {
            throw new ArgumentOutOfRangeException(
                nameof(b),
                "Правая граница должна быть конечным числом.");
        }

        if (!double.IsFinite(step) || step <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(step),
                "Шаг должен быть положительным конечным числом.");
        }

        if (threadsNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(threadsNumber),
                "Количество потоков должно быть положительным.");
        }

        if (a == b)
        {
            return 0.0;
        }

        var sign = 1.0;

        if (a > b)
        {
            (a, b) = (b, a);
            sign = -1.0;
        }

        var segmentLength = (b - a) / threadsNumber;
        var sharedResult = 0.0;

        using var barrier = new Barrier(threadsNumber + 1);

        var threads = new Thread[threadsNumber];
        var exceptions = new ConcurrentQueue<Exception>();

        for (var threadIndex = 0;
             threadIndex < threadsNumber;
             threadIndex++)
        {
            var index = threadIndex;

            var segmentStart =
                a + index * segmentLength;

            var segmentEnd =
                index == threadsNumber - 1
                    ? b
                    : a + (index + 1) * segmentLength;

            threads[index] = new Thread(() =>
            {
                try
                {
                    var partialResult = CalculateSegment(
                        segmentStart,
                        segmentEnd,
                        function,
                        step);

                    AddAtomically(
                        ref sharedResult,
                        partialResult);
                }
                catch (Exception exception)
                {
                    exceptions.Enqueue(exception);
                }
                finally
                {
                    barrier.SignalAndWait();
                }
            });

            threads[index].Start();
        }

        barrier.SignalAndWait();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        if (!exceptions.IsEmpty)
        {
            throw new AggregateException(exceptions);
        }

        return sign * sharedResult;
    }

    private static double CalculateSegment(
        double start,
        double end,
        Func<double, double> function,
        double step)
    {
        var result = 0.0;
        var current = start;

        while (current < end)
        {
            var next = Math.Min(
                current + step,
                end);

            var width = next - current;

            if (width <= 0)
            {
                throw new InvalidOperationException(
                    "Невозможно продолжить вычисление.");
            }

            var leftValue = function(current);
            var rightValue = function(next);

            ValidateFunctionValue(leftValue);
            ValidateFunctionValue(rightValue);

            result +=
                width *
                (leftValue + rightValue) /
                2.0;

            current = next;
        }

        return result;
    }

    private static void AddAtomically(
        ref double sharedValue,
        double value)
    {
        double initialValue;
        double computedValue;

        do
        {
            initialValue = sharedValue;

            computedValue =
                initialValue + value;
        }
        while (
            Interlocked.CompareExchange(
                ref sharedValue,
                computedValue,
                initialValue) != initialValue);
    }

    private static void ValidateFunctionValue(
        double value)
    {
        if (!double.IsFinite(value))
        {
            throw new InvalidOperationException(
                "Функция вернула нечисловое или бесконечное значение.");
        }
    }
}