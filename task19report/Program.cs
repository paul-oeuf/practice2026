using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using ScottPlot;
using task17;

const int CommandCount = 5;
const int ExecutionsPerCommand = 3;
const int RequiredExecutions = CommandCount * ExecutionsPerCommand;

var executionRecords =
    new ConcurrentQueue<ExecutionRecord>();

var totalExecutions = 0;

using var server = new ServerThread();

var commands =
    Enumerable
        .Range(1, CommandCount)
        .Select(
            id =>
                new TestCommand(
                    id,
                    server.Scheduler,
                    (commandId, commandNumber) =>
                    {
                        var globalNumber =
                            Interlocked.Increment(
                                ref totalExecutions);

                        executionRecords.Enqueue(
                            new ExecutionRecord(
                                globalNumber,
                                commandId,
                                commandNumber));

                        if (globalNumber == RequiredExecutions)
                        {
                            server.Enqueue(
                                new HardStop(server));
                        }
                    }))
        .ToArray();

foreach (var command in commands)
{
    server.Scheduler.Add(command);
}

var stopwatch = Stopwatch.StartNew();

server.Start();
server.Join();

stopwatch.Stop();

var records =
    executionRecords
        .OrderBy(item => item.GlobalNumber)
        .ToList();

if (records.Count != RequiredExecutions)
{
    throw new InvalidOperationException(
        $"Ожидалось {RequiredExecutions} выполнений, " +
        $"получено {records.Count}.");
}

if (commands.Any(
        command =>
            command.ExecutionCount != ExecutionsPerCommand))
{
    throw new InvalidOperationException(
        "Не все команды были выполнены ровно три раза.");
}

Console.WriteLine(
    "Демонстрация задания 19 завершена.");

Console.WriteLine();

foreach (var record in records)
{
    Console.WriteLine(
        $"Поток {record.CommandId} " +
        $"вызов {record.CommandNumber}");
}

Console.WriteLine();

Console.WriteLine(
    $"Количество команд: {CommandCount}");

Console.WriteLine(
    $"Выполнений каждой команды: {ExecutionsPerCommand}");

Console.WriteLine(
    $"Всего выполнений: {records.Count}");

Console.WriteLine(
    "Остановка выполнена с помощью HardStop.");

Console.WriteLine(
    $"Время выполнения: {stopwatch.Elapsed.TotalMilliseconds:F3} мс");

var reportDirectory =
    Path.Combine(
        AppContext.BaseDirectory,
        "..",
        "..",
        "..",
        "report");

Directory.CreateDirectory(reportDirectory);

var reportPath =
    Path.Combine(
        reportDirectory,
        "task19-report.md");

var executionOrderChartPath =
    Path.Combine(
        reportDirectory,
        "task19-execution-order.png");

var commandCountChartPath =
    Path.Combine(
        reportDirectory,
        "task19-command-count.png");

CreateExecutionOrderChart(
    records,
    executionOrderChartPath);

CreateCommandCountChart(
    records,
    commandCountChartPath);

var report =
    new StringBuilder();

report.AppendLine("# Отчёт по задаче №19");
report.AppendLine();
report.AppendLine(
    "## Реализация длительных операций");
report.AppendLine();

report.AppendLine(
    "### Цель работы");
report.AppendLine();

report.AppendLine(
    "Целью работы является предоставление возможности " +
    "выполнения команд, для полного завершения которых " +
    "требуется более одного вызова метода Execute().");
report.AppendLine();

report.AppendLine(
    "### Реализация");
report.AppendLine();

report.AppendLine(
    "Для демонстрации работы планировщика была реализована " +
    "команда TestCommand. Команда хранит собственный счётчик " +
    "выполнений. После каждого вызова Execute() она либо " +
    "завершает работу, либо повторно добавляет себя в " +
    "RoundRobinScheduler.");
report.AppendLine();

report.AppendLine(
    $"В эксперименте использовано {CommandCount} экземпляров " +
    "TestCommand. Каждая команда должна была выполнить " +
    $"{ExecutionsPerCommand} вызова Execute().");
report.AppendLine();

report.AppendLine(
    "Таким образом, общее количество выполнений составило " +
    $"{RequiredExecutions}.");
report.AppendLine();

report.AppendLine(
    "Для выбора следующей команды использовалась циклическая " +
    "стратегия Round Robin. После завершения всех требуемых " +
    "выполнений в очередь ServerThread добавлялась команда " +
    "HardStop.");
report.AppendLine();

report.AppendLine(
    "### Результаты эксперимента");
report.AppendLine();

report.AppendLine(
    $"Фактически выполнено команд: {records.Count}.");
report.AppendLine();

report.AppendLine(
    "Количество выполнений каждой команды:");
report.AppendLine();

report.AppendLine("| Команда | Количество выполнений |");
report.AppendLine("|---:|---:|");

foreach (var command in commands)
{
    report.AppendLine(
        $"| {command.Id} | {command.ExecutionCount} |");
}

report.AppendLine();

report.AppendLine(
    "Порядок выполнения команд:");
report.AppendLine();

report.AppendLine("| Номер выполнения | Команда | Номер вызова |");
report.AppendLine("|---:|---:|---:|");

foreach (var record in records)
{
    report.AppendLine(
        $"| {record.GlobalNumber} | " +
        $"{record.CommandId} | " +
        $"{record.CommandNumber} |");
}

report.AppendLine();

report.AppendLine(
    "### Интерпретация результатов");
report.AppendLine();

report.AppendLine(
    "Полученная последовательность выполнения демонстрирует " +
    "чередование команд. Ни одна длительная команда не " +
    "занимает поток на всё время своей работы.");
report.AppendLine();

report.AppendLine(
    "После каждого вызова Execute() команда, если её работа " +
    "ещё не завершена, снова добавляется в планировщик. " +
    "Благодаря этому RoundRobinScheduler получает возможность " +
    "выбрать другую команду.");
report.AppendLine();

report.AppendLine(
    "Таким образом, пять длительных операций выполняются " +
    "псевдопараллельно в одном потоке. На каждом шаге " +
    "обрабатывается небольшая часть работы конкретной команды.");
report.AppendLine();

report.AppendLine(
    "### Остановка выполнения");
report.AppendLine();

report.AppendLine(
    "После выполнения пятнадцатого вызова Execute() в очередь " +
    "ServerThread добавляется HardStop. Команда HardStop " +
    "выполняется непосредственно серверным потоком и " +
    "немедленно завершает его работу.");
report.AppendLine();

report.AppendLine(
    "В результате все пять команд были выполнены ровно по " +
    "три раза, после чего поток был корректно остановлен.");
report.AppendLine();

report.AppendLine(
    "### Вывод");
report.AppendLine();

report.AppendLine(
    "В ходе работы была продемонстрирована возможность " +
    "выполнения длительных операций без монопольного " +
    "использования потока одной командой. Использование " +
    "планировщика Round Robin позволяет распределять " +
    "время выполнения между несколькими командами.");
report.AppendLine();

report.AppendLine(
    "Эксперимент подтвердил, что для реализации длительных " +
    "операций достаточно разбить их выполнение на несколько " +
    "вызовов Execute() и возвращать незавершённую команду " +
    "в планировщик.");
report.AppendLine();

report.AppendLine(
    "Графики эксперимента:");
report.AppendLine();

report.AppendLine(
    "![Порядок выполнения команд](task19-execution-order.png)");
report.AppendLine();

report.AppendLine(
    "![Количество выполнений команд](task19-command-count.png)");

File.WriteAllText(
    reportPath,
    report.ToString(),
    Encoding.UTF8);

Console.WriteLine();

Console.WriteLine(
    $"Отчёт сохранён: {reportPath}");

Console.WriteLine(
    $"График порядка выполнения: {executionOrderChartPath}");

Console.WriteLine(
    $"График количества выполнений: {commandCountChartPath}");

static void CreateExecutionOrderChart(
    IReadOnlyList<ExecutionRecord> records,
    string path)
{
    var plot = new Plot();

    var xs =
        records
            .Select(
                record =>
                    (double)record.GlobalNumber)
            .ToArray();

    var ys =
        records
            .Select(
                record =>
                    (double)record.CommandId)
            .ToArray();

    plot.Add.Scatter(xs, ys);

    plot.Title(
        "Порядок выполнения длительных команд");

    plot.XLabel(
        "Номер выполнения");

    plot.YLabel(
        "Идентификатор команды");

    plot.SavePng(
        path,
        1200,
        700);
}

static void CreateCommandCountChart(
    IReadOnlyList<ExecutionRecord> records,
    string path)
{
    var plot = new Plot();

    var grouped =
        records
            .GroupBy(
                record =>
                    record.CommandId)
            .OrderBy(
                group =>
                    group.Key)
            .ToList();

    var xs =
        grouped
            .Select(
                group =>
                    (double)group.Key)
            .ToArray();

    var ys =
        grouped
            .Select(
                group =>
                    (double)group.Count())
            .ToArray();

    plot.Add.Scatter(xs, ys);

    plot.Title(
        "Количество выполнений каждой команды");

    plot.XLabel(
        "Идентификатор команды");

    plot.YLabel(
        "Количество выполнений");

    plot.SavePng(
        path,
        1200,
        700);
}

public sealed record ExecutionRecord(
    int GlobalNumber,
    int CommandId,
    int CommandNumber);

public sealed class TestCommand : ICommand
{
    private readonly IScheduler _scheduler;
    private readonly Action<int, int> _onExecute;

    private int _executionCount;

    public TestCommand(
        int id,
        IScheduler scheduler,
        Action<int, int> onExecute)
    {
        Id = id;

        _scheduler =
            scheduler ??
            throw new ArgumentNullException(
                nameof(scheduler));

        _onExecute =
            onExecute ??
            throw new ArgumentNullException(
                nameof(onExecute));
    }

    public int Id { get; }

    public int ExecutionCount =>
        Volatile.Read(
            ref _executionCount);

    public void Execute()
    {
        var executionNumber =
            Interlocked.Increment(
                ref _executionCount);

        _onExecute(
            Id,
            executionNumber);

        if (executionNumber < 3)
        {
            _scheduler.Add(this);
        }
    }
}