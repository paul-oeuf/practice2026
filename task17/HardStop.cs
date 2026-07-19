namespace task17;

public sealed class HardStop : ICommand
{
    private readonly ServerThread _serverThread;

    public HardStop(ServerThread serverThread)
    {
        _serverThread =
            serverThread ??
            throw new ArgumentNullException(nameof(serverThread));
    }

    public void Execute()
    {
        _serverThread.RequestHardStop();
    }
}