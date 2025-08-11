using Microsoft.Extensions.Logging;
using SprinklerOptimization.Contracts;
using SprinklerOptimization.Helpers;
using SprinklerOptimization.Models;
using SprinklerOptimization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.LayoutStrategy
{
    public class MinimumPipeDistanceLayout : ILayoutStrategy
    {
        private readonly ISprinklerConfiguration _configuration;
        private readonly IMetricsCalculationService _metricsCalculationService;
        private readonly IPolygonService _polygonService;
        private readonly ILogger<SprinklerLayoutService> _logger;

        public MinimumPipeDistanceLayout(ISprinklerConfiguration configuration,
            IMetricsCalculationService metricsCalculationService,
            IPolygonService polygonService,
            ILogger<SprinklerLayoutService> logger)
        {
            _configuration = configuration;
            _metricsCalculationService = metricsCalculationService;
            _polygonService = polygonService;
            _logger = logger;
        }

        public List<Point> CalculateSprinklerPosition(List<Point> roomCorners, List<Pipe> pipes)
        {
            return CalculatePipeProximityLayout(roomCorners, pipes);
        }

        /// <summary>
        /// Pipe proximity optimization strategy
        /// Minimizes total connection distance from water pipe while maintaining coverage
        /// </summary>
        private List<Point> CalculatePipeProximityLayout(List<Point> roomCorners, List<Pipe> waterPipes)
        {
            _logger.LogDebug("Calculating pipe proximity optimized layout");

            var sprinklers = new List<Point>();
            var bounds = PointUtils.CalculateRoomBoundary(roomCorners);

            // Create candidate positions near water pipes
            var candidates = new List<Point>();

            foreach (var pipe in waterPipes)
            {
                // Sample points along each pipe and project to ceiling
                var sampleCount = Math.Max(3, (int)(pipe.Length / _configuration.SprinklerSpacing));

                for (int i = 0; i <= sampleCount; i++)
                {
                    var t = (double)i / sampleCount;
                    var pipePoint = pipe.Start.Add(pipe.Direction.Scale(t));
                    var candidate = new Point(pipePoint.X, pipePoint.Y, _configuration.CeilingHeight);

                    if (PointUtils.IsPointInBoundary(candidate, roomCorners) &&
                        PointUtils.DoesPointMaintainClearance(candidate, roomCorners, _configuration.WallClearance))
                    {
                        candidates.Add(candidate);
                    }
                }
            }

            // Select candidates that maintain proper spacing
            candidates = candidates.OrderBy(c => waterPipes.Min(p => p.ShortestDistanceToPoint(c))).ToList();

            foreach (var candidate in candidates)
            {
                bool hasConflict = sprinklers.Any(existing =>
                    existing.Distance2DTo(candidate) < _configuration.SprinklerSpacing);

                if (!hasConflict)
                {
                    sprinklers.Add(candidate);
                }
            }

            _logger.LogDebug("Pipe proximity layout: {Count} sprinklers placed", sprinklers.Count);
            return sprinklers;
        }
    }
}
