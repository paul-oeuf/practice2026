using CommandLib;

namespace FileSystemCommands;

public class DirectorySizeCommand : ICommand
{
    private readonly string directoryPath;

    public long Size { get; private set; }

    public DirectorySizeCommand(string directoryPath)
    {
        this.directoryPath = directoryPath;
    }

    public void Execute()
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException(directoryPath);
        }

        Size = Directory
            .GetFiles(directoryPath, "*", SearchOption.AllDirectories)
            .Sum(file => new FileInfo(file).Length);

        Console.WriteLine($"Directory size: {Size} bytes");
    }
}