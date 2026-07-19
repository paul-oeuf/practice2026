using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using task17;
using ScottPlot;

const int SliceCount = 200;
const int WorkPerSlice = 20_000;
const int Repeats = 5;

var outputDirectory = Path.Combine(
    Directory.GetCurrentDirectory(),
    "report");

Directory.CreateDirectory(outputDirectory);

Console.WriteLine(
    "Задача 18. Экспериментальное исследование планировщика команд");

Console.WriteLine();

var roundRobinOrder = new ConcurrentQueue<string>();

var scheduler = new RoundRobinScheduler();

using var orderServer = new ServerThread(scheduler);

var commandA = new ExperimentalLongCommand(
    "A",
    scheduler,
    10,
    1,
    roundRobinOrder);

var commandB = new ExperimentalLongCommand(
    "B",
    scheduler,
    10,
    1,
    roundRobinOrder);

scheduler.Add(commandA);
scheduler.Add(commandB);

orderServer.Start();

if (!SpinWait.SpinUntil(
        () => roundRobinOrder.Count == 20,
        TimeSpan.FromSeconds(5)))
{
    throw new Exception(
        "Эксперимент Round Robin не завершился за отведенное время.");
}

orderServer.Enqueue(new SoftStop(orderServer));

orderServer.Join();

var order = roundRobinOrder.ToArray();

Console.WriteLine(
    "Порядок выполнения длительных команд:");

Console.WriteLine(
    string.Join(" -> ", order));

var roundRobinCorrect = true;

for (var i = 0; i < order.Length; i++)
{
    var expected = i % 2 == 0
        ? "A"
        : "B";

    if (order[i] != expected)
    {
        roundRobinCorrect = false;

        break;
    }
}

if (!roundRobinCorrect)
{
    throw new Exception(
        "Стратегия Round Robin не обеспечила циклическое выполнение команд.");
}

Console.WriteLine();

Console.WriteLine(
    "Round Robin: корректно.");

Console.WriteLine();

var commandCounts = new[]
{
    2,
    4,
    8,
    16
};

var performanceResults = new List<PerformanceResult>();

foreach (var commandCount in commandCounts)
{
    var measurements = new List<double>();

    for (var repeat = 0; repeat < Repeats; repeat++)
    {
        var measurementScheduler =
            new RoundRobinScheduler();

        using var measurementServer =
            new ServerThread(measurementScheduler);

        using var completedCommands =
            new CountdownEvent(commandCount);

        for (var i = 0; i < commandCount; i++)
        {
            measurementScheduler.Add(
                new ExperimentalLongCommand(
                    $"Command-{i + 1}",
                    measurementScheduler,
                    SliceCount,
                    WorkPerSlice,
                    null,
                    completedCommands));
        }

        var stopwatch = Stopwatch.StartNew();

        measurementServer.Start();

        if (!completedCommands.Wait(TimeSpan.FromSeconds(30)))
        {
            throw new Exception(
                $"Эксперимент с {commandCount} командами не завершился.");
        }

        stopwatch.Stop();

        measurementServer.Enqueue(
            new SoftStop(measurementServer));

        measurementServer.Join();

        measurements.Add(
            stopwatch.Elapsed.TotalMilliseconds);
    }

    var average = measurements.Average();

    performanceResults.Add(
        new PerformanceResult(
            commandCount,
            average));

    Console.WriteLine(
        $"Команд: {commandCount}; " +
        $"среднее время: {average:F3} мс");
}

Console.WriteLine();

var roundRobinPlot = new Plot();

var orderX = Enumerable
    .Range(1, order.Length)
    .Select(item => (double)item)
    .ToArray();

var orderY = order
    .Select(item => item == "A" ? 1.0 : 2.0)
    .ToArray();

roundRobinPlot.Add.Scatter(
    orderX,
    orderY);

roundRobinPlot.Title(
    "Циклическое выполнение длительных команд");

roundRobinPlot.XLabel(
    "Номер вызова Execute");

roundRobinPlot.YLabel(
    "Команда");

roundRobinPlot.Axes.Left.TickGenerator =
    new ScottPlot.TickGenerators.NumericManual(
        new[] { 1.0, 2.0 },
        new[] { "A", "B" });

roundRobinPlot.SavePng(
    Path.Combine(
        outputDirectory,
        "task18-round-robin.png"),
    1200,
    700);

var performancePlot = new Plot();

performancePlot.Add.Scatter(
    performanceResults
        .Select(item => (double)item.CommandCount)
        .ToArray(),
    performanceResults
        .Select(item => item.AverageMilliseconds)
        .ToArray());

performancePlot.Title(
    "Время обработки длительных команд");

performancePlot.XLabel(
    "Количество длительных команд");

performancePlot.YLabel(
    "Среднее время выполнения, мс");

performancePlot.SavePng(
    Path.Combine(
        outputDirectory,
        "task18-performance.png"),
    1200,
    700);

var report = new StringBuilder();

report.AppendLine(
    "# Задача 18. Реализация планировщика команд");

report.AppendLine();

report.AppendLine(
    "## Цель работы");

report.AppendLine();

report.AppendLine(
    "Целью работы является реализация механизма выполнения длительных операций в отдельном потоке с использованием планировщика команд. Основная идея заключается в разбиении длительной операции на небольшие части, каждая из которых выполняется за один вызов метода Execute.");

report.AppendLine();

report.AppendLine(
    "## Реализация");

report.AppendLine();

report.AppendLine(
    "В рамках работы был расширен поток обработки команд, реализованный в практической работе 17. Для обработки длительных операций был введен интерфейс IScheduler, отвечающий за хранение и выбор следующей команды для выполнения.");

report.AppendLine();

report.AppendLine(
    "В качестве стратегии планирования реализован алгоритм Round Robin. Планировщик последовательно выбирает команды из очереди. Длительная команда после выполнения очередной части работы добавляет себя обратно в планировщик, если операция еще не завершена.");

report.AppendLine();

report.AppendLine(
    "Таким образом, команда не блокирует ServerThread длительным выполнением метода Execute. Между двумя последовательными вызовами Execute одной команды могут быть выполнены другие команды.");

report.AppendLine();

report.AppendLine(
    "## Эксперимент 1. Проверка Round Robin");

report.AppendLine();

report.AppendLine(
    "Для проверки справедливости планирования были созданы две длительные команды A и B. Каждая команда должна была выполнить десять частей работы.");

report.AppendLine();

report.AppendLine(
    $"Полученный порядок выполнения: {string.Join(" → ", order)}.");

report.AppendLine();

report.AppendLine(
    "Результат соответствует стратегии Round Robin: команды выполнялись циклически и поочередно. Это показывает, что одна длительная команда не может полностью занять поток и вытеснить остальные команды из обработки.");

report.AppendLine();

report.AppendLine(
    "График циклического выполнения представлен на рисунке task18-round-robin.png.");

report.AppendLine();

report.AppendLine(
    "## Эксперимент 2. Исследование времени обработки");

report.AppendLine();

report.AppendLine(
    $"Для каждого варианта выполнялось {Repeats} повторных измерений. Длительная операция была разбита на {SliceCount} частей. В каждой части выполнялась вычислительная нагрузка.");

report.AppendLine();

report.AppendLine(
    "| Количество команд | Среднее время, мс |");

report.AppendLine(
    "|---:|---:|");

foreach (var result in performanceResults)
{
    report.AppendLine(
        $"| {result.CommandCount} | {result.AverageMilliseconds:F3} |");
}

report.AppendLine();

report.AppendLine(
    "По результатам эксперимента видно, что увеличение количества длительных команд приводит к увеличению общего времени обработки. Это объясняется тем, что ServerThread является одним рабочим потоком и последовательно выполняет части всех операций.");

report.AppendLine();

report.AppendLine(
    "При этом важным результатом планировщика является не ускорение вычислений за счет параллельного выполнения, а возможность справедливого псевдопараллельного обслуживания большого количества длительных операций.");

report.AppendLine();

report.AppendLine(
    "График зависимости времени обработки от количества длительных команд представлен на рисунке task18-performance.png.");

report.AppendLine();

report.AppendLine(
    "## Вывод");

report.AppendLine();

report.AppendLine(
    "В ходе работы был реализован планировщик длительных команд на основе стратегии Round Robin. Длительные операции были разделены на небольшие части, выполняемые за отдельные вызовы Execute.");

report.AppendLine();

report.AppendLine(
    "Эксперимент подтвердил корректность циклического выбора команд. Команды A и B выполнялись поочередно, что подтверждает справедливость реализованной стратегии планирования.");

report.AppendLine();

report.AppendLine(
    "Использование отдельного планировщика позволяет избежать блокировки очереди новыми командами при повторной постановке длительной операции на выполнение. ServerThread может обрабатывать новые команды и продолжать выполнение ранее начатых длительных операций.");

report.AppendLine();

report.AppendLine(
    "Таким образом, поставленная цель работы достигнута. Реализованный механизм позволяет обслуживать несколько длительных операций в одном рабочем потоке в режиме псевдопараллельной обработки.");

File.WriteAllText(
    Path.Combine(
        outputDirectory,
        "task18-report.md"),
    report.ToString(),
    Encoding.UTF8);

Console.WriteLine();

Console.WriteLine(
    "Отчет создан: report/task18-report.md");

Console.WriteLine(
    "График Round Robin создан: report/task18-round-robin.png");

Console.WriteLine(
    "График производительности создан: report/task18-performance.png");

public sealed record PerformanceResult(
    int CommandCount,
    double AverageMilliseconds);

public sealed class ExperimentalLongCommand : ICommand
{
    private readonly string _name;

    private readonly IScheduler _scheduler;

    private readonly int _workPerExecute;

    private readonly ConcurrentQueue<string>?
        _executionOrder;

    private readonly CountdownEvent?
        _completedCommands;

    private int _remainingExecutions;

    public ExperimentalLongCommand(
        string name,
        IScheduler scheduler,
        int executions,
        int workPerExecute,
        ConcurrentQueue<string>? executionOrder = null,
        CountdownEvent? completedCommands = null)
    {
        _name = name;

        _scheduler = scheduler;

        _remainingExecutions = executions;

        _workPerExecute = workPerExecute;

        _executionOrder = executionOrder;

        _completedCommands = completedCommands;
    }

    public void Execute()
    {
        _executionOrder?.Enqueue(_name);

        var value = 0.0;

        for (var i = 0; i < _workPerExecute; i++)
        {
            value += Math.Sin(i) * Math.Cos(i);
        }

        GC.KeepAlive(value);

        if (Interlocked.Decrement(
                ref _remainingExecutions) > 0)
        {
            _scheduler.Add(this);

            return;
        }

        _completedCommands?.Signal();
    }
}