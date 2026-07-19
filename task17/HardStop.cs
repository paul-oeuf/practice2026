namespace task17;

public sealed class HardStop : ICommand
{
    private readonly ServerThread _serverThread;

    public HardStop(ServerThread serverThread)
    {
        ArgumentNullException.ThrowIfNull(serverThread);

        _serverThread = serverThread;
    }

    public void Execute()
    {
        _serverThread.RequestHardStop();
    }
}