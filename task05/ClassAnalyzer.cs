using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace task05;

public class ClassAnalyzer
{
    private readonly Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Select(method => method.Name);
    }

    public IEnumerable<string> GetMethodParams(string methodName)
    {
        var method = _type
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .FirstOrDefault(m => m.Name == methodName);

        if (method == null)
            return Enumerable.Empty<string>();

        var parameters = method
            .GetParameters()
            .Select(parameter => $"{parameter.ParameterType.Name} {parameter.Name}");

        return parameters.Append($"Returns: {method.ReturnType.Name}");
    }

    public IEnumerable<string> GetAllFields()
    {
        return _type
            .GetFields(BindingFlags.Instance |
                       BindingFlags.Public |
                       BindingFlags.NonPublic |
                       BindingFlags.DeclaredOnly)
            .Select(field => field.Name);
    }

    public IEnumerable<string> GetProperties()
    {
        return _type
            .GetProperties(BindingFlags.Instance |
                           BindingFlags.Public |
                           BindingFlags.DeclaredOnly)
            .Select(property => property.Name);
    }

    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.IsDefined(typeof(T), false);
    }
}