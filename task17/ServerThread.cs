using System.Collections.Concurrent;

namespace task17;

public sealed class ServerThread : IDisposable
{
    private readonly BlockingCollection<ICommand> _commands = new();

    private readonly Thread _thread;

    private readonly Action<Exception, ICommand>? _exceptionHandler;

    private int _threadId;

    private int _started;

    private int _hardStopRequested;

    private int _disposed;

    public ServerThread(
        Action<Exception, ICommand>? exceptionHandler = null)
    {
        _exceptionHandler = exceptionHandler;

        _thread = new Thread(Run)
        {
            IsBackground = true,
            Name = "ServerThread"
        };
    }

    public bool IsRunning =>
        _thread.IsAlive;

    public int ThreadId =>
        Volatile.Read(ref _threadId);

    public void Start()
    {
        ThrowIfDisposed();

        if (Interlocked.Exchange(
                ref _started,
                1) != 0)
        {
            throw new InvalidOperationException(
                "ServerThread уже был запущен.");
        }

        _thread.Start();
    }

    public void Enqueue(ICommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        ThrowIfDisposed();

        if (_commands.IsAddingCompleted)
        {
            throw new InvalidOperationException(
                "Добавление команд завершено.");
        }

        _commands.Add(command);
    }

    public void Join()
    {
        ThrowIfDisposed();

        _thread.Join();
    }

    internal bool IsCurrentThread =>
        Thread.CurrentThread.ManagedThreadId == ThreadId;

    internal void RequestHardStop()
    {
        if (!IsCurrentThread)
        {
            throw new InvalidOperationException(
                "Команда HardStop должна выполняться в останавливаемом потоке.");
        }

        Volatile.Write(
            ref _hardStopRequested,
            1);

        _commands.CompleteAdding();
    }

    internal void RequestSoftStop()
    {
        if (!IsCurrentThread)
        {
            throw new InvalidOperationException(
                "Команда SoftStop должна выполняться в останавливаемом потоке.");
        }

        _commands.CompleteAdding();
    }

    private void Run()
    {
        Volatile.Write(
            ref _threadId,
            Thread.CurrentThread.ManagedThreadId);

        try
        {
            foreach (var command in _commands.GetConsumingEnumerable())
            {
                try
                {
                    command.Execute();
                }
                catch (Exception exception)
                {
                    _exceptionHandler?.Invoke(
                        exception,
                        command);
                }

                if (Volatile.Read(
                        ref _hardStopRequested) != 0)
                {
                    break;
                }
            }
        }
        catch (ObjectDisposedException)
        {
            if (Volatile.Read(ref _disposed) == 0)
            {
                throw;
            }
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            Volatile.Read(ref _disposed) != 0,
            this);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(
                ref _disposed,
                1) != 0)
        {
            return;
        }

        try
        {
            _commands.CompleteAdding();
        }
        catch (ObjectDisposedException)
        {
        }

        if (_thread.IsAlive &&
            Thread.CurrentThread != _thread)
        {
            _thread.Join();
        }

        _commands.Dispose();
    }
}