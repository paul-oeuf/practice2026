namespace task10;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PluginDependencyAttribute : Attribute
{
    public string DependencyName { get; }

    public PluginDependencyAttribute(string dependencyName)
    {
        DependencyName = dependencyName;
    }
}