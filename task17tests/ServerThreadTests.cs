using System.Collections.Concurrent;
using task17;

namespace task17tests;

public class ServerThreadTests
{
    [Fact]
    public void HardStop_ShouldStopProcessingRemainingCommands()
    {
        using var server = new ServerThread();

        var executed = new ConcurrentQueue<int>();

        server.Enqueue(
            new ActionCommand(
                () => executed.Enqueue(1)));

        server.Enqueue(new HardStop(server));

        server.Enqueue(
            new ActionCommand(
                () => executed.Enqueue(2)));

        server.Start();

        server.Join();

        Assert.Equal(
            new[] { 1 },
            executed.ToArray());
    }

    [Fact]
    public void SoftStop_ShouldProcessCommandsAlreadyInQueue()
    {
        using var server = new ServerThread();

        var executed = new ConcurrentQueue<int>();

        server.Enqueue(
            new ActionCommand(
                () => executed.Enqueue(1)));

        server.Enqueue(new SoftStop(server));

        server.Enqueue(
            new ActionCommand(
                () => executed.Enqueue(2)));

        server.Start();

        server.Join();

        Assert.Equal(
            new[] { 1, 2 },
            executed.ToArray());
    }

    [Fact]
    public void StopCommands_ShouldRejectExecutionOutsideTargetThread()
    {
        using var server = new ServerThread();

        Assert.Throws<InvalidOperationException>(
            () => new HardStop(server).Execute());

        Assert.Throws<InvalidOperationException>(
            () => new SoftStop(server).Execute());
    }

    [Fact]
    public void ServerThread_ShouldProcessManyCommands()
    {
        using var server = new ServerThread();

        var count = 0;

        const int commandCount = 1000;

        for (var i = 0; i < commandCount; i++)
        {
            server.Enqueue(
                new ActionCommand(
                    () => Interlocked.Increment(ref count)));
        }

        server.Enqueue(new SoftStop(server));

        server.Start();

        server.Join();

        Assert.Equal(
            commandCount,
            count);
    }

    [Fact]
    public void ServerThread_ShouldPassCommandExceptionToExceptionHandler()
    {
        using var received = new ManualResetEventSlim();

        ICommand? receivedCommand = null;
        Exception? receivedException = null;

        using var server = new ServerThread(
            (command, exception) =>
            {
                receivedCommand = command;
                receivedException = exception;

                received.Set();
            });

        var commandToFail =
            new ActionCommand(
                () => throw new InvalidOperationException("test"));

        server.Enqueue(commandToFail);

        server.Enqueue(new SoftStop(server));

        server.Start();

        server.Join();

        Assert.True(
            received.Wait(TimeSpan.FromSeconds(1)));

        Assert.Same(
            commandToFail,
            receivedCommand);

        Assert.IsType<InvalidOperationException>(
            receivedException);
    }

    [Fact]
    public void RoundRobinScheduler_ShouldSelectCommandsCyclically()
    {
        var scheduler = new RoundRobinScheduler();

        var first =
            new ActionCommand(() => { });

        var second =
            new ActionCommand(() => { });

        scheduler.Add(first);
        scheduler.Add(second);

        Assert.Same(
            first,
            scheduler.Select());

        Assert.Same(
            second,
            scheduler.Select());

        scheduler.Add(first);
        scheduler.Add(second);

        Assert.Same(
            first,
            scheduler.Select());

        Assert.Same(
            second,
            scheduler.Select());

        Assert.False(
            scheduler.HasCommand());
    }

    [Fact]
    public void ServerThread_ShouldExecuteLongCommandsInRoundRobinOrder()
    {
        var scheduler = new RoundRobinScheduler();

        using var server =
            new ServerThread(scheduler);

        var executionOrder =
            new ConcurrentQueue<string>();

        var first =
            new LongCommand(
                "A",
                scheduler,
                3,
                executionOrder);

        var second =
            new LongCommand(
                "B",
                scheduler,
                3,
                executionOrder);

        scheduler.Add(first);
        scheduler.Add(second);

        server.Start();

        Assert.True(
            SpinWait.SpinUntil(
                () => executionOrder.Count == 6,
                TimeSpan.FromSeconds(2)));

        server.Enqueue(
            new SoftStop(server));

        server.Join();

        Assert.Equal(
            new[]
            {
                "A",
                "B",
                "A",
                "B",
                "A",
                "B"
            },
            executionOrder.ToArray());
    }

    [Fact]
    public void ServerThread_ShouldWakeWhenNewCommandArrivesAfterIdle()
    {
        using var server = new ServerThread();

        using var executed =
            new ManualResetEventSlim();

        server.Start();

        Assert.True(
            server.IsRunning);

        server.Enqueue(
            new ActionCommand(
                () => executed.Set()));

        Assert.True(
            executed.Wait(TimeSpan.FromSeconds(1)));

        server.Enqueue(
            new SoftStop(server));

        server.Join();
    }

    [Fact]
    public void Dispose_ShouldStopIdleServerThread()
    {
        using var server = new ServerThread();

        server.Start();

        Assert.True(
            server.IsRunning);

        server.Dispose();

        Assert.False(
            server.IsRunning);
    }

    [Fact]
    public void Task19_ShouldExecuteFiveLongCommandsThreeTimes()
    {
        var scheduler = new RoundRobinScheduler();

        using var server =
            new ServerThread(scheduler);

        var executionLog =
            new ConcurrentQueue<string>();

        const int commandCount = 5;
        const int executionsPerCommand = 3;

        for (var i = 1; i <= commandCount; i++)
        {
            scheduler.Add(
                new TestCommand(
                    i,
                    scheduler,
                    executionsPerCommand,
                    executionLog));
        }

        server.Start();

        Assert.True(
            SpinWait.SpinUntil(
                () =>
                    executionLog.Count ==
                    commandCount * executionsPerCommand,
                TimeSpan.FromSeconds(2)));

        server.Enqueue(
            new HardStop(server));

        server.Join();

        Assert.Equal(
            commandCount * executionsPerCommand,
            executionLog.Count);

        for (var i = 1; i <= commandCount; i++)
        {
            Assert.Equal(
                executionsPerCommand,
                executionLog.Count(
                    item =>
                        item.StartsWith(
                            $"Поток {i} вызов")));
        }
    }

    [Fact]
    public void Task19_ShouldExecuteCommandsInRoundRobinOrder()
    {
        var scheduler = new RoundRobinScheduler();

        using var server =
            new ServerThread(scheduler);

        var executionLog =
            new ConcurrentQueue<string>();

        scheduler.Add(
            new TestCommand(
                1,
                scheduler,
                3,
                executionLog));

        scheduler.Add(
            new TestCommand(
                2,
                scheduler,
                3,
                executionLog));

        server.Start();

        Assert.True(
            SpinWait.SpinUntil(
                () => executionLog.Count == 6,
                TimeSpan.FromSeconds(2)));

        server.Enqueue(
            new HardStop(server));

        server.Join();

        Assert.Equal(
            new[]
            {
                "Поток 1 вызов 1",
                "Поток 2 вызов 1",
                "Поток 1 вызов 2",
                "Поток 2 вызов 2",
                "Поток 1 вызов 3",
                "Поток 2 вызов 3"
            },
            executionLog.ToArray());
    }

    private sealed class ActionCommand : ICommand
    {
        private readonly Action _action;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action();
        }
    }

    private sealed class LongCommand : ICommand
    {
        private readonly string _name;
        private readonly IScheduler _scheduler;
        private readonly ConcurrentQueue<string> _executionOrder;

        private int _remainingExecutions;

        public LongCommand(
            string name,
            IScheduler scheduler,
            int executions,
            ConcurrentQueue<string> executionOrder)
        {
            _name = name;
            _scheduler = scheduler;
            _remainingExecutions = executions;
            _executionOrder = executionOrder;
        }

        public void Execute()
        {
            _executionOrder.Enqueue(_name);

            if (Interlocked.Decrement(
                    ref _remainingExecutions) > 0)
            {
                _scheduler.Add(this);
            }
        }
    }

    private sealed class TestCommand : ICommand
    {
        private readonly int _id;
        private readonly IScheduler _scheduler;
        private readonly ConcurrentQueue<string> _executionLog;

        private int _counter;
        private int _remainingExecutions;

        public TestCommand(
            int id,
            IScheduler scheduler,
            int executions,
            ConcurrentQueue<string> executionLog)
        {
            _id = id;
            _scheduler = scheduler;
            _remainingExecutions = executions;
            _executionLog = executionLog;
        }

        public void Execute()
        {
            var currentCall =
                Interlocked.Increment(ref _counter);

            _executionLog.Enqueue(
                $"Поток {_id} вызов {currentCall}");

            if (Interlocked.Decrement(
                    ref _remainingExecutions) > 0)
            {
                _scheduler.Add(this);
            }
        }
    }
}