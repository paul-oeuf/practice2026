using System.Reflection;
using CommandLib;

string dllPath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "FileSystemCommands.dll");


if (!File.Exists(dllPath))
{
    Console.WriteLine("DLL not found");
    return;
}


Assembly assembly = Assembly.LoadFrom(dllPath);


var commandTypes = assembly
    .GetTypes()
    .Where(type =>
        typeof(ICommand).IsAssignableFrom(type)
        && !type.IsInterface);


foreach (var type in commandTypes)
{
    Console.WriteLine($"Loaded command: {type.Name}");
}