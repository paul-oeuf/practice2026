using System.Reflection;

namespace task09;

public class MetadataReader
{
    public string ReadMetadata(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Library not found",
                path);
        }


        Assembly assembly = Assembly.LoadFrom(path);

        StringWriter writer = new();


        writer.WriteLine(
            $"Library: {assembly.FullName}");



        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsClass)
                continue;


            writer.WriteLine();
            writer.WriteLine("====================");
            writer.WriteLine(
                $"Class: {type.FullName}");



            writer.WriteLine(
                "Class attributes:");

            foreach (var attribute in type.GetCustomAttributes()
                .Where(a => a.GetType().Namespace == type.Namespace))
            {
                writer.WriteLine(
                    $"  {attribute.GetType().Name}");
            }



            writer.WriteLine(
                "\nConstructors:");

            foreach (var constructor in type.GetConstructors())
            {
                writer.WriteLine(
                    $"  {constructor.Name}");


                foreach (var parameter in constructor.GetParameters())
                {
                    writer.WriteLine(
                        $"    Parameter: {parameter.Name}, Type: {parameter.ParameterType.Name}");
                }
            }



            writer.WriteLine(
                "\nProperties:");

            foreach (var property in type.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly))
            {
                writer.WriteLine(
                    $"  Property: {property.Name}, Type: {property.PropertyType.Name}");


                foreach (var attribute in property.GetCustomAttributes())
                {
                    writer.WriteLine(
                        $"    Attribute: {attribute.GetType().Name}");
                }
            }



            writer.WriteLine(
                "\nMethods:");

            foreach (var method in type.GetMethods(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly))
            {
                writer.WriteLine(
                    $"  Method: {method.Name}");



                writer.WriteLine(
                    "    Attributes:");

                foreach (var attribute in method.GetCustomAttributes())
                {
                    writer.WriteLine(
                        $"      {attribute.GetType().Name}");
                }



                foreach (var parameter in method.GetParameters())
                {
                    writer.WriteLine(
                        $"    Parameter: {parameter.Name}, Type: {parameter.ParameterType.Name}");
                }
            }
        }


        return writer.ToString();
    }
}