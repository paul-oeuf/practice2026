using task09;


if (args.Length == 0)
{
    Console.WriteLine("Передайте путь к DLL");
    return;
}


var reader = new MetadataReader();

string result = reader.ReadMetadata(args[0]);

Console.WriteLine(result);