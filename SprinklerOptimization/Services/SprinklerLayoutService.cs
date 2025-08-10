using Microsoft.Extensions.Logging;
using SprinklerOptimization.Contracts;
using SprinklerOptimization.Helpers;
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
        private readonly ILogger<SprinklerLayoutService> _logger;

        public SprinklerLayoutService(
            ISprinklerConfiguration configuration,
            IMetricsCalculationService metricsCalculationService,
            ILogger<SprinklerLayoutService> logger
            )
        {
            _configuration = configuration;
            _metricsCalculationService = metricsCalculationService;
            _logger = logger;
        }

/*        public SprinklerLayoutResults CalculateLayoutResults(
            List<Point> roomCorners, 
            List<Pipe> pipes, 
            Enums.Enums.SprinklerLayouts layout = Enums.Enums.SprinklerLayouts.Grid)
        {
            try
            {
                if(roomCorners is null || roomCorners.Count < 3 )
                {
                    throw new ArgumentException("Room must have at least 3 corners");
                }

                if(pipes is null || pipes.Count == 0)
                {
                    throw new ArgumentException("At least one water pipe must be provided");
                }

                var sprinklers = layout switch
                {
                    SprinklerLayouts.Grid => CalculateSprinklerPositionForGridLayout(roomCorners),
                    //SprinklerLayouts.MaximumCoverage => CalculateOptimizedCoverageLayout(roomCorners),
                    //SprinklerLayouts.MinimumPipeDistance => CalculatePipeProximityLayout(roomCorners, waterPipes),
                    _ => throw new ArgumentException($"Unknown strategy: {layout}")
                };

                var result = new SprinklerLayoutResults
                {
                    SprinklerPositions = sprinklers,
                    ConnectionPoints = connections,
                    StrategyUsed = layout,
                    QualityMetrics = metrics,
                    CalculationTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds,
                    IsValid = true
                };
            }
            catch (Exception ex) { }
        }*/

        /// <summary>
        /// Sprinkler will be positioned on a rectangular grid
        /// 
        /// </summary>
        /// <param name="roomCorners"></param>
        /// <returns></returns>
        private List<Point> CalculateSprinklerPositionForGridLayout(List<Point> roomCorners)
        {
            _logger.LogDebug("Calculating uniform grid layout");

            var boundaries = PointUtils.CalculateRoomBoundary(roomCorners);
            var sprinklerPositions = new List<Point>();

            //consider wall clearance from room boundaries for placing sprinkler
            var startX = boundaries.MinX + _configuration.WallClearance;
            var endX = boundaries.MaxX - _configuration.WallClearance;
            var startY = boundaries.MinY + _configuration.WallClearance;
            var endY = boundaries.MaxY - _configuration.WallClearance;

            //sprinkler will be at distance of at least sprinkler_spacing from each other
            for(var x = startX; x<= endX; x += _configuration.SprinklerSpacing)
            {
                for(var y = startY; y<= endY; y += _configuration.SprinklerSpacing)
                {
                    var point = new Point(x, y, _configuration.CeilingHeight);
                    //check if the point in the celing is inside room boundary and have sufficient distance from the wall
                    if(PointUtils.IsPointInBoundary(point, roomCorners) 
                        && PointUtils.DoesPointMaintainClearance(point, roomCorners, _configuration.WallClearance))
                    {
                        sprinklerPositions.Add(point);
                    }
                }
            }

            return sprinklerPositions;
        }

        public bool ValidateLayout(SprinklerLayoutResults result, out string validationMessage)
        {
            throw new NotImplementedException();
        }

        public SprinklerLayoutResults CalculateLayoutResults(List<Point> roomCorners, List<Pipe> pipes, SprinklerLayouts layout = SprinklerLayouts.Grid)
        {
            throw new NotImplementedException();
        }
    }
}
