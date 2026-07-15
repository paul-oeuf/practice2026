using System;
using System.Linq;
using Xunit;
using task05;

namespace task05tests;

public class TestClass
{
    public int PublicField;
    private string _privateField = string.Empty;

    public int Property { get; set; }

    public void Method() { }

    public int Sum(int a, int b)
    {
        return a + b;
    }
}

[Serializable]
public class AttributedClass
{
}

public class NonAttributedClass
{
}

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var methods = analyzer.GetPublicMethods().ToList();

        Assert.Contains("Method", methods);
        Assert.Contains("Sum", methods);
    }

    [Fact]
    public void GetMethodParams_ReturnsCorrectParametersAndReturnType()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var parameters = analyzer.GetMethodParams("Sum").ToList();

        Assert.Contains("Int32 a", parameters);
        Assert.Contains("Int32 b", parameters);
        Assert.Contains("Returns: Int32", parameters);
    }

    [Fact]
    public void GetMethodParams_ReturnsEmpty_WhenMethodDoesNotExist()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var parameters = analyzer.GetMethodParams("UnknownMethod");

        Assert.Empty(parameters);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var fields = analyzer.GetAllFields().ToList();

        Assert.Contains("PublicField", fields);
        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var properties = analyzer.GetProperties().ToList();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_ReturnsTrue_WhenAttributeExists()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));

        Assert.True(analyzer.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void HasAttribute_ReturnsFalse_WhenAttributeDoesNotExist()
    {
        var analyzer = new ClassAnalyzer(typeof(NonAttributedClass));

        Assert.False(analyzer.HasAttribute<SerializableAttribute>());
    }
}