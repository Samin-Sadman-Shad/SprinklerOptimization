using SprinklerOptimization.Contracts;
using SprinklerOptimization.Helpers;
using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Services
{
    public class MetricsCalculationService : IMetricsCalculationService
    {
        private readonly ISprinklerConfiguration _configuration;
        private readonly IPolygonService _polygonService;

        public MetricsCalculationService(ISprinklerConfiguration configuration, IPolygonService polygonService)
        {
            _configuration = configuration;
            _polygonService = polygonService;
        }
        public LayoutMetrics CalculateLayoutMetrics(List<Point> sprinklers, 
            IDictionary<Point, Point> connections, 
            List<Point> roomCorners)
        {
            var roomArea = _polygonService.CalculatePolygonArea(roomCorners);
            var metrics = new LayoutMetrics
            {
                TotalSprinklers = sprinklers.Count,
            };

            if (metrics.TotalSprinklers > 0)
            {
                metrics.CoveragePerSprinkler = roomArea / metrics.TotalSprinklers;

                // Connection distance statistics
                var connectionDistances = connections.Values.Select(conn =>
                    connections.Keys.First(k => connections[k].Equals(conn)).DistanceTo(conn)).ToList();

                if (connectionDistances.Any())
                {
                    metrics.AverageConnectionDistance = connectionDistances.Average();
                    metrics.MaxConnectionDistance = connectionDistances.Max();
                    metrics.MinConnectionDistance = connectionDistances.Min();
                }

                // Spacing analysis
                var spacingDistances = new List<double>();
                for (int i = 0; i < sprinklers.Count; i++)
                {
                    for (int j = i + 1; j < sprinklers.Count; j++)
                    {
                        spacingDistances.Add(sprinklers[i].Distance2DTo(sprinklers[j]));
                    }
                }

                if (spacingDistances.Any())
                {
                    metrics.AverageSpacingDistance = spacingDistances.Average();
                    metrics.SpacingUniformity = _configuration.SprinklerSpacing / metrics.AverageSpacingDistance;
                }

                // Coverage efficiency (higher is better)
                var theoreticalOptimalCount = roomArea / (Math.PI * _configuration.MinimumCoverageRadius * _configuration.MinimumCoverageRadius);
                metrics.CoverageEfficiency = Math.Min(1.0, theoreticalOptimalCount / metrics.TotalSprinklers);

            }

            return metrics;
        }
    }
}
