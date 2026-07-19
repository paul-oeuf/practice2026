namespace task17;

public sealed class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> _commands = new();
    private readonly object _sync = new();

    public bool HasCommand()
    {
        lock (_sync)
        {
            return _commands.Count > 0;
        }
    }

    public ICommand Select()
    {
        lock (_sync)
        {
            if (_commands.Count == 0)
            {
                throw new InvalidOperationException(
                    "Планировщик не содержит команд.");
            }

            return _commands.Dequeue();
        }
    }

    public void Add(ICommand cmd)
    {
        ArgumentNullException.ThrowIfNull(cmd);

        lock (_sync)
        {
            _commands.Enqueue(cmd);
        }
    }
}