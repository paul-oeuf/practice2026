using task10;


if (args.Length == 0)
{
    Console.WriteLine(
        "Укажите папку с плагинами");
    return;
}


var manager = new PluginManager();


manager.LoadPlugins(args[0]);


manager.ExecutePlugins();