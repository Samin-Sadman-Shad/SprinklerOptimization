using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SprinklerOptimization;
using SprinklerOptimization.Contracts;
using SprinklerOptimization.Services;

try
{
    var services = new ServiceCollection();

    // Configuration
    services.AddSingleton<ISprinklerConfiguration>(provider => new SprinklerConfiguration
    {
        WallClearance = 2500.0,
        SprinklerSpacing = 2500.0,
        CeilingHeight = 2500.0,
        MinimumCoverageRadius = 1800.0,
        MaximumConnectionDistance = 8000.0
    });

    services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

    // Core services
    services.AddSingleton<IPolygonService, PolygonService>();
    services.AddSingleton<IMetricsCalculationService, MetricsCalculationService>();
    services.AddSingleton<ISprinklerLayoutService, SprinklerLayoutService>();
    services.AddSingleton<IReportService, ReportService>();
    services.AddSingleton<ISprinklerConfiguration, SprinklerConfiguration>();
    services.AddSingleton<IVisualizationService, VisualizationService>();
    services.AddSingleton<SprinklerManager>();


    var serviceProvider = services.BuildServiceProvider();

    var manager = serviceProvider.GetRequiredService<SprinklerManager>();
    manager.DesignSprinkler();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

