using Microsoft.Extensions.DependencyInjection;
using SprinklerOptimization.Contracts;
using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SprinklerOptimization.Enums.Enums;

namespace SprinklerOptimization
{
    public class SprinklerManager
    {
        private ISprinklerLayoutService _layoutService;
        private IReportService _reportService;
        public SprinklerManager(ISprinklerLayoutService layoutService, IReportService reportService) 
        {
            _layoutService = layoutService;
            _reportService = reportService;
        }
        public void DesignSprinkler()
        {
            Console.WriteLine("ADVANCED SPRINKLER LAYOUT OPTIMIZATION SYSTEM");
            Console.WriteLine("=============================================");
            Console.WriteLine("Demonstrating professional software engineering practices:");
            Console.WriteLine("• Dependency Injection");
            Console.WriteLine("• Comprehensive Unit Testing");
            Console.WriteLine("• Advanced 2D/3D Spatial Reasoning");
            Console.WriteLine("• Multiple Optimization Strategies");
            Console.WriteLine("• Professional Documentation");
            Console.WriteLine();

            try
            {

                // Define the actual problem room (irregular quadrilateral)
                var roomCorners = new List<Point>
                {
                    new Point(97500.00, 34000.00, 2500.00),  // Corner 1
                    new Point(85647.67, 43193.61, 2500.00),  // Corner 2  
                    new Point(91776.75, 51095.16, 2500.00),  // Corner 3
                    new Point(103629.07, 41901.55, 2500.00)  // Corner 4
                };

                var waterPipes = new List<Pipe>
                {
                    new Pipe(new Point(98242.11, 36588.29, 3000.00), new Point(87970.10, 44556.09, 3500.00)),
                    new Pipe(new Point(99774.38, 38563.68, 3500.00), new Point(89502.37, 46531.47, 3000.00)),
                    new Pipe(new Point(101306.65, 40539.07, 3000.00), new Point(91034.63, 48506.86, 3000.00))
                };

                // Get services from DI container

                Console.WriteLine("Problem Definition:");
                Console.WriteLine($"• Room: Irregular quadrilateral with {roomCorners.Count} corners");
                Console.WriteLine($"• Water Pipes: {waterPipes.Count} available for connections");
                Console.WriteLine($"• Requirements: 2500mm wall clearance, 2500mm sprinkler spacing");
                Console.WriteLine();

                // PHASE 4: Multi-strategy analysis
                Console.WriteLine("PHASE 4: MULTI-STRATEGY OPTIMIZATION ANALYSIS");
                Console.WriteLine("=============================================");

                var strategies = new[]
                {
                    SprinklerLayouts.Grid,
                    SprinklerLayouts.MaximumCoverage,
                    SprinklerLayouts.MinimumPipeDistance
                };

                var results = new List<SprinklerLayoutResults>();

                foreach (var strategy in strategies)
                {
                    Console.WriteLine($"\n--- ANALYZING STRATEGY: {strategy} ---");

                    var result = _layoutService.CalculateLayoutResults(roomCorners, waterPipes, strategy);
                    results.Add(result);

                    if (result.IsValid)
                    {
                        Console.WriteLine($"✓ Strategy completed successfully");
                        Console.WriteLine($"  Sprinklers placed: {result.SprinklerPositions.Count}");
                        Console.WriteLine($"  Avg connection distance: {result.Metrics.AverageConnectionDistance:F2} mm");
                        Console.WriteLine($"  Coverage efficiency: {result.Metrics.CoverageEfficiency:P1}");
                        Console.WriteLine($"  Calculation time: {result.CalculationTimeMs:F2} ms");
                    }
                    else
                    {
                        Console.WriteLine($"❌ Strategy failed: {result.ValidationMessage}");
                    }
                }

                // Find and display the optimal strategy
                var validResults = results.Where(r => r.IsValid).ToList();
                if (validResults.Any())
                {
                    Console.WriteLine("===================================");
                    Console.WriteLine("VISUALIZATION AND DETAILED REPORTING");
                    Console.WriteLine("============================================");

                    // Generate comprehensive report
                    foreach (var result in validResults)
                    {
                        Console.WriteLine($"Generating report for {result.StrategyUsed}");
                        var detailedReport = _reportService.GenerateDetailedReport(result);
                        Console.WriteLine(detailedReport);
                    }

                }
                else
                {
                    Console.WriteLine("❌ No valid strategies found. Please check input parameters.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ System Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            finally
            {
                Console.WriteLine("\n" + new string('=', 80));
                Console.WriteLine("Analysis complete. Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
