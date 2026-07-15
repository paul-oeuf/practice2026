namespace task10;

[AttributeUsage(AttributeTargets.Class)]
public class PluginDependencyAttribute : Attribute
{
    public string DependencyName { get; }

    public PluginDependencyAttribute(string dependencyName)
    {
        DependencyName = dependencyName;
    }
}