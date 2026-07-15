using System.Reflection;
using task10;

namespace task10tests;


public class PluginManagerTests
{
    [Fact]
    public void PluginLoadAttribute_ShouldExist()
    {
        var attributes =
            typeof(MainPlugin)
            .GetCustomAttributes(
                typeof(PluginLoadAttribute),
                false);


        Assert.Single(attributes);
    }



    [Fact]
    public void DependencyAttribute_ShouldBeDetected()
    {
        var attributes =
            typeof(MainPlugin)
            .GetCustomAttributes(
                typeof(PluginDependencyAttribute),
                false);


        Assert.Single(attributes);


        var dependency =
            attributes[0] as PluginDependencyAttribute;


        Assert.NotNull(dependency);


        Assert.Equal(
            nameof(BasePlugin),
            dependency.DependencyName);
    }



    [Fact]
    public void Plugin_ShouldHaveExecuteMethod()
    {
        var method =
            typeof(MainPlugin)
            .GetMethod("Execute");


        Assert.NotNull(method);
    }
}



[PluginLoad]
public class BasePlugin
{
    public void Execute()
    {

    }
}



[PluginLoad]
[PluginDependency(nameof(BasePlugin))]
public class MainPlugin
{
    public void Execute()
    {

    }
}