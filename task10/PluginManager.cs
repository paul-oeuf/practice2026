using System.Reflection;

namespace task10;

public class PluginManager
{
    private readonly List<Assembly> assemblies = new();


    public void LoadPlugins(string folder)
    {
        var files = Directory.GetFiles(
            folder,
            "*.dll");


        foreach (var file in files)
        {
            assemblies.Add(
                Assembly.LoadFrom(file));
        }
    }



    public void ExecutePlugins()
    {
        var plugins = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t.GetCustomAttribute<PluginLoadAttribute>() != null)
            .ToList();


        foreach (var plugin in plugins)
        {
            if (plugin.GetConstructor(Type.EmptyTypes) == null)
                continue;


            var instance = Activator.CreateInstance(plugin);


            var method = plugin.GetMethod("Execute");


            method?.Invoke(instance, null);
        }
    }
}