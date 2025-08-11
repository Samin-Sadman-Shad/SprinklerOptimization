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
    public class GridLayout : ILayoutStrategy
    {
        private readonly ISprinklerConfiguration _configuration;
        private readonly ILogger<SprinklerLayoutService> _logger;

        public GridLayout(ISprinklerConfiguration configuration,
            ILogger<SprinklerLayoutService> logger
            )
        {
            _configuration = configuration;
            _logger = logger;
        }
        public List<Point> CalculateSprinklerPosition(List<Point> roomCorners, List<Pipe> pipes)
        {
            return CalculateSprinklerPositionForGridLayout(roomCorners);
        }

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
            for (var x = startX; x <= endX; x += _configuration.SprinklerSpacing)
            {
                for (var y = startY; y <= endY; y += _configuration.SprinklerSpacing)
                {
                    var point = new Point(x, y, _configuration.CeilingHeight);
                    //check if the point in the celing is inside room boundary and have sufficient distance from the wall
                    if (PointUtils.IsPointInBoundary(point, roomCorners)
                        && PointUtils.DoesPointMaintainClearance(point, roomCorners, _configuration.WallClearance))
                    {
                        sprinklerPositions.Add(point);
                    }
                }
            }

            return sprinklerPositions;
        }
    }
}
