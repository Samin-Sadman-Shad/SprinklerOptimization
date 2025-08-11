using Microsoft.Extensions.Logging;
using SprinklerOptimization.Contracts;
using SprinklerOptimization.Helpers;
using SprinklerOptimization.LayoutStrategy;
using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SprinklerOptimization.Enums.Enums;

namespace SprinklerOptimization.Services
{
    public class SprinklerLayoutService : ISprinklerLayoutService
    {
        private readonly ISprinklerConfiguration _configuration;
        private readonly IMetricsCalculationService _metricsCalculationService;
        private readonly IPolygonService _polygonService;
        private readonly ILogger<SprinklerLayoutService> _logger;

        public SprinklerLayoutService(
            ISprinklerConfiguration configuration,
            IMetricsCalculationService metricsCalculationService,
            IPolygonService polygonService,
            ILogger<SprinklerLayoutService> logger
            )
        {
            _configuration = configuration;
            _metricsCalculationService = metricsCalculationService;
            _polygonService = polygonService;
            _logger = logger;

            LayoutStrategyFactory.Register(SprinklerLayouts.Grid, 
                () => new GridLayout(configuration, logger));
            LayoutStrategyFactory.Register(SprinklerLayouts.MaximumCoverage, 
                () => new MaximumCoverageLayout(configuration, metricsCalculationService, polygonService, logger));
            LayoutStrategyFactory.Register(SprinklerLayouts.MinimumPipeDistance,
                () => new MinimumPipeDistanceLayout(configuration, metricsCalculationService, polygonService, logger));
        }

        public SprinklerLayoutResults CalculateLayoutResults(
            List<Point> roomCorners,
            List<Pipe> pipes,
            Enums.Enums.SprinklerLayouts layout = Enums.Enums.SprinklerLayouts.Grid)
        {
            var startTime = DateTime.UtcNow;
            try
            {
                if (roomCorners is null || roomCorners.Count < 3)
                {
                    throw new ArgumentException("Room must have at least 3 corners");
                }

                if (pipes is null || pipes.Count == 0)
                {
                    throw new ArgumentException("At least one water pipe must be provided");
                }

                //var sprinklers = layout switch
                //{
                //    SprinklerLayouts.Grid => CalculateSprinklerPositionForGridLayout(roomCorners),
                //    SprinklerLayouts.MaximumCoverage => CalculateOptimizedCoverageLayout(roomCorners),
                //    SprinklerLayouts.MinimumPipeDistance => CalculatePipeProximityLayout(roomCorners, pipes),
                //    _ => throw new ArgumentException($"Unknown strategy: {layout}")
                //};

                var layoutStrategy = LayoutStrategyFactory.Create(layout);
                var sprinklers = layoutStrategy.CalculateSprinklerPosition(roomCorners, pipes);


                // Calculate optimal connections
                var connectionPoints = CalculateOptimalConnectionPoints(sprinklers, pipes);
                //var closestPipes = CalculateOptimalConnectionPipe(sprinklers, pipes);

                // Calculate quality metrics
                var metrics = _metricsCalculationService.CalculateLayoutMetrics(sprinklers, connectionPoints, roomCorners);

                var result = new SprinklerLayoutResults
                {
                    SprinklerPositions = sprinklers,
                    ConnectionPoints = connectionPoints,
                    StrategyUsed = layout,
                    Metrics = metrics,
                    CalculationTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds,
                    IsValid = true
                };

                // Validate result
                if (!ValidateLayout(result, out string validationMessage))
                {
                    result.IsValid = false;
                    result.ValidationMessage = validationMessage;
                    _logger.LogWarning("Layout validation failed: {Message}", validationMessage);
                }

                _logger.LogInformation("Layout calculation completed. {Count} sprinklers placed in {Time:F2}ms",
                    sprinklers.Count, result.CalculationTimeMs);

                return result;

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error calculating sprinkler layout");
                return new SprinklerLayoutResults
                {
                    IsValid = false,
                    ValidationMessage = $"Calculation failed: {ex.Message}",
                    CalculationTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds
                };
            }
        }

        /// <summary>
        /// Calculate optimal pipe connections for all sprinklers
        /// Implements efficient nearest-neighbor search
        /// </summary>
        private Dictionary<Point, Point> CalculateOptimalConnectionPoints(List<Point> sprinklers, List<Pipe> waterPipes)
        {
            var connections = new Dictionary<Point, Point>();

            foreach (var sprinkler in sprinklers)
            {
                Point bestConnection = new Point(0,0,0);
                double minDistance = double.MaxValue;

                foreach (var pipe in waterPipes)
                {
                    var connectionPoint = pipe.GetClosestPoint(sprinkler);
                    var distance = sprinkler.DistanceTo(connectionPoint);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        bestConnection = connectionPoint;
                    }
                }

                connections[sprinkler] = bestConnection;
            }

            return connections;
        }

        private Dictionary<int, int> CalculateOptimalConnectionPipe(List<Point> sprinklers, List<Pipe> waterPipes)
        {

            Dictionary<int, int> connections = new();
            for(int i = 0; i<sprinklers.Count; i++)
            {
                int bestPipe = 0;
                double minDistance = Double.MaxValue;

                for(int j = 0; i<waterPipes.Count; j++)
                {
                    var connectionPoint = waterPipes[j].GetClosestPoint(sprinklers[i]);
                    var distance = sprinklers[i].DistanceTo(connectionPoint);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        bestPipe = j;
                    }
                }

                connections[i] = bestPipe;
            }

            return connections;
        }

        public bool ValidateLayout(SprinklerLayoutResults result, out string validationMessage)
        {
            var issues = new List<string>();

            if (result.SprinklerPositions.Count == 0)
            {
                issues.Add("No sprinklers placed");
            }

            // Check minimum spacing requirements
            for (int i = 0; i < result.SprinklerPositions.Count; i++)
            {
                for (int j = i + 1; j < result.SprinklerPositions.Count; j++)
                {
                    var distance = result.SprinklerPositions[i].Distance2DTo(result.SprinklerPositions[j]);
                    if (distance < _configuration.SprinklerSpacing - 1e-6) // Small tolerance for floating point
                    {
                        issues.Add($"Sprinklers {i + 1} and {j + 1} are too close ({distance:F2} mm < {_configuration.SprinklerSpacing} mm)");
                    }
                }
            }

            // Check connection distances
            foreach (var kvp in result.ConnectionPoints)
            {
                var connectionDistance = kvp.Key.DistanceTo(kvp.Value);
                if (connectionDistance > _configuration.MaximumConnectionDistance)
                {
                    issues.Add($"Connection distance {connectionDistance:F2} mm exceeds maximum {_configuration.MaximumConnectionDistance} mm");
                }
            }

            validationMessage = string.Join("; ", issues);
            return !issues.Any();
        }

    }
}
