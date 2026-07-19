using task17;
using Xunit;

namespace task17tests;

public sealed class ServerThreadTests
{
    [Fact]
    public void HardStop_ShouldStopProcessingRemainingCommands()
    {
        using var server = new ServerThread();

        var executedCommands = new List<int>();

        server.Enqueue(
            new ActionCommand(
                () => executedCommands.Add(1)));

        server.Enqueue(
            new HardStop(server));

        server.Enqueue(
            new ActionCommand(
                () => executedCommands.Add(2)));

        server.Enqueue(
            new ActionCommand(
                () => executedCommands.Add(3)));

        server.Start();

        server.Join();

        Assert.Equal(
            new[] { 1 },
            executedCommands);
    }

    [Fact]
    public void SoftStop_ShouldProcessCommandsAlreadyInQueue()
    {
        using var server = new ServerThread();

        var executedCommands = new List<int>();

        server.Enqueue(
            new ActionCommand(
                () => executedCommands.Add(1)));

        server.Enqueue(
            new SoftStop(server));

        server.Enqueue(
            new ActionCommand(
                () => executedCommands.Add(2)));

        server.Enqueue(
            new ActionCommand(
                () => executedCommands.Add(3)));

        server.Start();

        server.Join();

        Assert.Equal(
            new[] { 1, 2, 3 },
            executedCommands);
    }

    [Fact]
    public void HardStop_ShouldThrowWhenExecutedOutsideTargetThread()
    {
        using var server = new ServerThread();

        var command = new HardStop(server);

        Assert.Throws<InvalidOperationException>(
            command.Execute);
    }

    [Fact]
    public void SoftStop_ShouldThrowWhenExecutedOutsideTargetThread()
    {
        using var server = new ServerThread();

        var command = new SoftStop(server);

        Assert.Throws<InvalidOperationException>(
            command.Execute);
    }

    [Fact]
    public void ServerThread_ShouldProcessManyCommands()
    {
        using var server = new ServerThread();

        var executedCount = 0;

        const int commandsCount = 1000;

        for (int i = 0; i < commandsCount; i++)
        {
            server.Enqueue(
                new ActionCommand(
                    () => Interlocked.Increment(
                        ref executedCount)));
        }

        server.Enqueue(
            new SoftStop(server));

        server.Start();

        server.Join();

        Assert.Equal(
            commandsCount,
            executedCount);
    }

    [Fact]
    public void ServerThread_ShouldPassCommandExceptionToExceptionHandler()
    {
        Exception? receivedException = null;

        ICommand? receivedCommand = null;

        using var server = new ServerThread(
            (exception, command) =>
            {
                receivedException = exception;
                receivedCommand = command;
            });

        var command = new ActionCommand(
            () => throw new InvalidOperationException(
                "Test exception"));

        server.Enqueue(command);

        server.Enqueue(
            new SoftStop(server));

        server.Start();

        server.Join();

        Assert.NotNull(receivedException);

        Assert.Same(
            command,
            receivedCommand);
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
}