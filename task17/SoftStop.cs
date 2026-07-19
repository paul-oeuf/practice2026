namespace task17;

public sealed class SoftStop : ICommand
{
    private readonly ServerThread _serverThread;

    public SoftStop(ServerThread serverThread)
    {
        ArgumentNullException.ThrowIfNull(serverThread);

        _serverThread = serverThread;
    }

    public void Execute()
    {
        _serverThread.RequestSoftStop();
    }
}