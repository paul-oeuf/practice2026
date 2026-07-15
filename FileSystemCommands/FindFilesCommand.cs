using CommandLib;

namespace FileSystemCommands;

public class FindFilesCommand : ICommand
{
    private readonly string directoryPath;
    private readonly string searchPattern;

    public List<string> FoundFiles { get; private set; } = new();

    public FindFilesCommand(string directoryPath, string searchPattern)
    {
        this.directoryPath = directoryPath;
        this.searchPattern = searchPattern;
    }

    public void Execute()
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException(directoryPath);
        }

        FoundFiles = Directory
            .GetFiles(
                directoryPath,
                searchPattern,
                SearchOption.AllDirectories)
            .ToList();

        foreach (var file in FoundFiles)
        {
            Console.WriteLine(file);
        }
    }
}