using SprinklerOptimization.Services;

try
{
    var calculator = new SprinklerCalculator();
    calculator.PrintResults();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
