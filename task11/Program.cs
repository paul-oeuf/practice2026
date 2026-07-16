namespace task11;

public static class Program
{
    public static void Main()
    {
        ICalculator calculator = CalculatorCompiler.CreateCalculator();

        Console.WriteLine($"2 + 3 = {calculator.Add(2, 3)}");
        Console.WriteLine($"7 - 5 = {calculator.Minus(7, 5)}");
        Console.WriteLine($"4 * 6 = {calculator.Mul(4, 6)}");
        Console.WriteLine($"20 / 4 = {calculator.Div(20, 4)}");
    }
}