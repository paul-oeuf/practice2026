using System.Collections.Concurrent;

namespace task17;

public sealed class ServerThread : IDisposable
{
    private readonly BlockingCollection<ICommand> _commands = new();
    private readonly Thread _thread;
    private readonly IScheduler _scheduler;
    private readonly Action<ICommand, Exception>? _exceptionHandler;

    private int _started;
    private int _disposed;
    private int _hardStopRequested;
    private int _softStopRequested;

    public ServerThread()
        : this(new RoundRobinScheduler(), null)
    {
    }

    public ServerThread(
        Action<ICommand, Exception>? exceptionHandler)
        : this(new RoundRobinScheduler(), exceptionHandler)
    {
    }

    public ServerThread(IScheduler scheduler)
        : this(scheduler, null)
    {
    }

    public ServerThread(
        IScheduler scheduler,
        Action<ICommand, Exception>? exceptionHandler)
    {
        _scheduler =
            scheduler ??
            throw new ArgumentNullException(nameof(scheduler));

        _exceptionHandler = exceptionHandler;

        _thread = new Thread(Run)
        {
            IsBackground = true,
            Name = "ServerThread"
        };
    }

    public IScheduler Scheduler => _scheduler;

    public bool IsRunning => _thread.IsAlive;

    public int ThreadId => _thread.ManagedThreadId;

    public void Start()
    {
        ObjectDisposedException.ThrowIf(
            Volatile.Read(ref _disposed) != 0,
            this);

        if (Interlocked.Exchange(ref _started, 1) != 0)
        {
            throw new InvalidOperationException(
                "ServerThread уже был запущен.");
        }

        _thread.Start();
    }

    public void Enqueue(ICommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        ObjectDisposedException.ThrowIf(
            Volatile.Read(ref _disposed) != 0,
            this);

        _commands.Add(command);
    }

    public void Join()
    {
        if (Volatile.Read(ref _started) == 0)
        {
            throw new InvalidOperationException(
                "ServerThread еще не был запущен.");
        }

        _thread.Join();
    }

    internal void RequestHardStop()
    {
        EnsureWorkerThread();

        Volatile.Write(ref _hardStopRequested, 1);

        _commands.CompleteAdding();
    }

    internal void RequestSoftStop()
    {
        EnsureWorkerThread();

        Volatile.Write(ref _softStopRequested, 1);

        _commands.CompleteAdding();
    }

    private void EnsureWorkerThread()
    {
        if (Thread.CurrentThread.ManagedThreadId != ThreadId)
        {
            throw new InvalidOperationException(
                "Команда остановки должна выполняться " +
                "в целевом ServerThread.");
        }
    }

    private void Run()
    {
        var preferQueue = true;

        while (true)
        {
            if (Volatile.Read(ref _hardStopRequested) != 0)
            {
                return;
            }

            if (preferQueue &&
                _commands.TryTake(out var command))
            {
                ExecuteCommand(command);

                preferQueue = false;

                continue;
            }

            if (!preferQueue &&
                _scheduler.HasCommand())
            {
                ExecuteCommand(_scheduler.Select());

                preferQueue = true;

                continue;
            }

            if (!preferQueue &&
                _commands.TryTake(out command))
            {
                ExecuteCommand(command);

                preferQueue = false;

                continue;
            }

            if (preferQueue &&
                _scheduler.HasCommand())
            {
                ExecuteCommand(_scheduler.Select());

                preferQueue = true;

                continue;
            }

            if (Volatile.Read(ref _softStopRequested) != 0)
            {
                return;
            }

            if (_commands.TryTake(out command, 50))
            {
                ExecuteCommand(command);

                preferQueue = false;
            }
        }
    }

    private void ExecuteCommand(ICommand command)
    {
        try
        {
            command.Execute();
        }
        catch (Exception exception)
        {
            _exceptionHandler?.Invoke(
                command,
                exception);
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        if (Volatile.Read(ref _started) != 0)
        {
            Volatile.Write(ref _hardStopRequested, 1);
        }

        _commands.CompleteAdding();

        if (Volatile.Read(ref _started) != 0 &&
            Thread.CurrentThread != _thread)
        {
            _thread.Join();
        }

        _commands.Dispose();
    }
}