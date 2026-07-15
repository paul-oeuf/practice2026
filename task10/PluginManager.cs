using System.Reflection;

namespace task10;

public class PluginManager
{
    private readonly List<Assembly> assemblies = new();


    private readonly Dictionary<string, Type> plugins = new();



    public void LoadPlugins(string folder)
    {
        var files = Directory.GetFiles(
            folder,
            "*.dll");


        foreach (var file in files)
        {
            var assembly =
                Assembly.LoadFrom(file);


            assemblies.Add(assembly);
        }


        FindPlugins();
    }



    private void FindPlugins()
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetCustomAttribute<PluginLoadAttribute>() != null)
                {
                    plugins[type.Name] = type;
                }
            }
        }
    }



    public void ExecutePlugins()
    {
        var orderedPlugins =
            SortPlugins();


        foreach (var plugin in orderedPlugins)
        {
            ExecutePlugin(plugin);
        }
    }



    private void ExecutePlugin(Type plugin)
    {
        var constructor =
            plugin.GetConstructor(
                Type.EmptyTypes);


        if (constructor == null)
            return;


        var instance =
            Activator.CreateInstance(plugin);



        var method =
            plugin.GetMethod("Execute");


        method?.Invoke(instance, null);
    }



    private List<Type> SortPlugins()
    {
        var result = new List<Type>();

        var visited =
            new HashSet<Type>();


        foreach (var plugin in plugins.Values)
        {
            Visit(
                plugin,
                visited,
                result);
        }


        return result;
    }




    private void Visit(
        Type plugin,
        HashSet<Type> visited,
        List<Type> result)
    {
        if (result.Contains(plugin))
            return;

        if (visited.Contains(plugin))
            throw new Exception(
                "Обнаружена циклическая зависимость плагинов");


        visited.Add(plugin);



        var dependencies =
            plugin.GetCustomAttributes<PluginDependencyAttribute>();


        foreach (var dependency in dependencies)
        {
            if (plugins.TryGetValue(
                dependency.DependencyName,
                out var dependencyType))
            {
                Visit(
                    dependencyType,
                    visited,
                    result);
            }
        }


        result.Add(plugin);
    }
}