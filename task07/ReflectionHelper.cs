using System.Reflection;

namespace task07;

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        var displayName = type.GetCustomAttribute<DisplayNameAttribute>();
        var version = type.GetCustomAttribute<VersionAttribute>();

        if (displayName != null)
        {
            Console.WriteLine($"Display Name: {displayName.DisplayName}");
        }

        if (version != null)
        {
            Console.WriteLine($"Version: {version.Major}.{version.Minor}");
        }

        Console.WriteLine("Methods:");

        foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var methodDisplayName = method.GetCustomAttribute<DisplayNameAttribute>();

            if (methodDisplayName != null)
            {
                Console.WriteLine($"{method.Name} - {methodDisplayName.DisplayName}");
            }
            else
            {
                Console.WriteLine(method.Name);
            }
        }

        Console.WriteLine("Properties:");

        foreach (var property in type.GetProperties())
        {
            var propertyDisplayName = property.GetCustomAttribute<DisplayNameAttribute>();

            if (propertyDisplayName != null)
            {
                Console.WriteLine($"{property.Name} - {propertyDisplayName.DisplayName}");
            }
            else
            {
                Console.WriteLine(property.Name);
            }
        }
    }
}