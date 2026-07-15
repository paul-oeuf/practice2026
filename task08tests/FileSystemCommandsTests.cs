using FileSystemCommands;

namespace task08tests;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(
            Path.GetTempPath(),
            "TestDirSize");


        Directory.CreateDirectory(testDir);


        File.WriteAllText(
            Path.Combine(testDir, "file1.txt"),
            "Hello");


        File.WriteAllText(
            Path.Combine(testDir, "file2.txt"),
            "World");


        var command = new DirectorySizeCommand(testDir);


        command.Execute();


        Assert.True(command.Size > 0);


        Directory.Delete(testDir, true);
    }



    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(
            Path.GetTempPath(),
            "TestDirFind");


        Directory.CreateDirectory(testDir);


        File.WriteAllText(
            Path.Combine(testDir, "file1.txt"),
            "Text");


        File.WriteAllText(
            Path.Combine(testDir, "file2.log"),
            "Log");


        var command = new FindFilesCommand(
            testDir,
            "*.txt");


        command.Execute();


        Assert.Single(command.FoundFiles);


        Directory.Delete(testDir, true);
    }



    [Fact]
    public void DirectorySizeCommand_InvalidDirectory_ShouldThrow()
    {
        var command =
            new DirectorySizeCommand(
                "UnknownDirectory");


        Assert.Throws<DirectoryNotFoundException>(
            () => command.Execute());
    }



    [Fact]
    public void FindFilesCommand_NoMatches_ShouldReturnEmpty()
    {
        var testDir = Path.Combine(
            Path.GetTempPath(),
            "EmptyFindDir");


        Directory.CreateDirectory(testDir);


        var command =
            new FindFilesCommand(
                testDir,
                "*.json");


        command.Execute();


        Assert.Empty(command.FoundFiles);


        Directory.Delete(testDir, true);
    }
}