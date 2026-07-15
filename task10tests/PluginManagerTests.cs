using task10;

namespace task10tests;


public class PluginManagerTests
{
    [Fact]
    public void PluginLoadAttribute_ShouldExist()
    {
        var attribute =
            typeof(TestPlugin)
            .GetCustomAttributes(
                typeof(PluginLoadAttribute),
                false);


        Assert.Single(attribute);
    }



    [Fact]
    public void Plugin_ShouldHaveExecuteMethod()
    {
        var method =
            typeof(TestPlugin)
            .GetMethod("Execute");


        Assert.NotNull(method);
    }
}



[PluginLoad]
public class TestPlugin
{
    public void Execute()
    {

    }
}