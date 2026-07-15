using System.Reflection;
using task07;

namespace task07tests;

public class AttributeReflectionTests
{
    [Fact]
    public void Class_HasDisplayNameAttribute()
    {
        var type = typeof(SampleClass);

        var attribute = type.GetCustomAttribute<DisplayNameAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("Пример класса", attribute!.DisplayName);
    }

    [Fact]
    public void Method_HasDisplayNameAttribute()
    {
        var method = typeof(SampleClass).GetMethod("TestMethod");

        Assert.NotNull(method);

        var attribute = method!.GetCustomAttribute<DisplayNameAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("Тестовый метод", attribute!.DisplayName);
    }

    [Fact]
    public void Property_HasDisplayNameAttribute()
    {
        var property = typeof(SampleClass).GetProperty("Number");

        Assert.NotNull(property);

        var attribute = property!.GetCustomAttribute<DisplayNameAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("Числовое свойство", attribute!.DisplayName);
    }

    [Fact]
    public void Class_HasVersionAttribute()
    {
        var type = typeof(SampleClass);

        var attribute = type.GetCustomAttribute<VersionAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal(1, attribute!.Major);
        Assert.Equal(0, attribute.Minor);
    }

    [Fact]
    public void ReflectionHelper_MethodExists()
    {
        var method = typeof(ReflectionHelper).GetMethod("PrintTypeInfo");

        Assert.NotNull(method);
    }

    [Fact]
    public void SampleClass_HasOneProperty()
    {
        var properties = typeof(SampleClass).GetProperties();

        Assert.Single(properties);
    }

    [Fact]
    public void SampleClass_HasTestMethod()
    {
        var method = typeof(SampleClass).GetMethod(nameof(SampleClass.TestMethod));

        Assert.NotNull(method);
    }
}