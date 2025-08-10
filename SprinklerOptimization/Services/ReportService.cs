using SprinklerOptimization.Contracts;
using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Services
{
    public class ReportService : IReportService
    {
        public string GenerateDetailedReport(SprinklerLayoutResults result)
        {
            var sb = new StringBuilder();

            sb.AppendLine("COMPREHENSIVE SPRINKLER LAYOUT REPORT");
            sb.AppendLine("====================================");
            sb.AppendLine();

            sb.AppendLine($"Strategy Used: {result.StrategyUsed}");
            sb.AppendLine($"Calculation Time: {result.CalculationTimeMs:F2} ms");
            sb.AppendLine($"Layout Valid: {result.IsValid}");
            if (!result.IsValid)
                sb.AppendLine($"Validation Issues: {result.ValidationMessage}");
            sb.AppendLine();

            var metrics = result.Metrics;
            sb.AppendLine("LAYOUT METRICS");
            sb.AppendLine("--------------");
            sb.AppendLine($"Total Sprinklers: {metrics.TotalSprinklers}");
/*            sb.AppendLine($"Room Area: {metrics.RoomArea:F2} mm²");*/
            sb.AppendLine($"Coverage per Sprinkler: {metrics.CoveragePerSprinkler:F2} mm²");
            sb.AppendLine($"Coverage Efficiency: {metrics.CoverageEfficiency:P1}");
            sb.AppendLine();

            sb.AppendLine("CONNECTION ANALYSIS");
            sb.AppendLine("------------------");
            sb.AppendLine($"Average Connection Distance: {metrics.AverageConnectionDistance:F2} mm");
            sb.AppendLine($"Min Connection Distance: {metrics.MinConnectionDistance:F2} mm");
            sb.AppendLine($"Max Connection Distance: {metrics.MaxConnectionDistance:F2} mm");
            sb.AppendLine();

            sb.AppendLine("SPACING ANALYSIS");
            sb.AppendLine("---------------");
            sb.AppendLine($"Average Sprinkler Spacing: {metrics.AverageSpacingDistance:F2} mm");
            sb.AppendLine($"Spacing Uniformity: {metrics.SpacingUniformity:P1}");
            sb.AppendLine();

            sb.AppendLine("DETAILED SPRINKLER POSITIONS");
            sb.AppendLine("---------------------------");
            for (int i = 0; i < result.SprinklerPositions.Count; i++)
            {
                var sprinkler = result.SprinklerPositions[i];
                if (result.ConnectionPoints.TryGetValue(sprinkler, out var connection))
                {
                    var distance = sprinkler.DistanceTo(connection);
                    sb.AppendLine($"Sprinkler {i + 1,2}: {sprinkler} → Connection: {connection} (Distance: {distance:F2} mm)");
                }
                else
                {
                    sb.AppendLine($"Sprinkler {i + 1,2}: {sprinkler} → No connection found");
                }
            }

            return sb.ToString();
        }
    }
}
