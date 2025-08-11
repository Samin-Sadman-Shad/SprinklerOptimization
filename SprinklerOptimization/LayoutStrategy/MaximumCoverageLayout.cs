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
    public class MaximumCoverageLayout : ILayoutStrategy
    {
        private readonly ISprinklerConfiguration _configuration;
        private readonly IMetricsCalculationService _metricsCalculationService;
        private readonly IPolygonService _polygonService;
        private readonly ILogger<SprinklerLayoutService> _logger;

        public MaximumCoverageLayout(ISprinklerConfiguration configuration,
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
            return CalculateOptimizedCoverageLayout(roomCorners);
        }

        /// <summary>
        /// It follows a greedy algorithm. It initially places the first sprinkler in the centroid.
        /// Then find the next best position that maximizes coverage.
        /// </summary>
        /// <param name="roomCorners"></param>
        /// <returns></returns>
        private List<Point> CalculateOptimizedCoverageLayout(List<Point> roomCorners)
        {
            _logger.LogDebug("Calculating optimized coverage layout");

            var sprinklers = new List<Point>();

            // Start with room centroid as first placement
            var centroid = _polygonService.CalculatePolygonCentroid(roomCorners);
            centroid = new Point(centroid.X, centroid.Y, _configuration.CeilingHeight);

            if (PointUtils.IsPointInBoundary(centroid, roomCorners) &&
                PointUtils.DoesPointMaintainClearance(centroid, roomCorners, _configuration.WallClearance))
            {
                sprinklers.Add(centroid);
            }

            // Use iterative placement to cover remaining areas
            var coverageRadius = _configuration.MinimumCoverageRadius;
            var maxIterations = 100; // Prevent infinite loops
            var iteration = 0;

            while (iteration < maxIterations)
            {
                var newSprinkler = FindNextBestSprinklerPosition(roomCorners, sprinklers, coverageRadius);
                if (newSprinkler == null) break;

                sprinklers.Add(newSprinkler);
                iteration++;
            }

            _logger.LogDebug("Optimized coverage layout: {Count} sprinklers placed in {Iterations} iterations",
                sprinklers.Count, iteration);
            return sprinklers;
        }

        private Point? FindNextBestSprinklerPosition(List<Point> roomCorners, List<Point> existingSprinklers, double coverageRadius)
        {
            var bounds = PointUtils.CalculateRoomBoundary(roomCorners);
            //smaller than spacing to explore potential position between ideal spacing
            //if four sprinklers are positioned at four indices of a rectangle,
            //the distnace between two sprinkler on the two ends of the diagonal is greater than the required min distance
            var searchStep = _configuration.SprinklerSpacing / 4.0; // Higher resolution search

            Point? bestPosition = null;
            double bestScore = 0.0;

            for (var x = bounds.MinX + _configuration.WallClearance; x <= bounds.MaxX - _configuration.WallClearance; x += searchStep)
            {
                for (var y = bounds.MinY + _configuration.WallClearance; y <= bounds.MaxY - _configuration.WallClearance; y += searchStep)
                {
                    var candidate = new Point(x, y, _configuration.CeilingHeight);

                    //skip if the candidate point is outside considered area or does not maintain the clearance
                    if (!PointUtils.IsPointInBoundary(candidate, roomCorners) ||
                        !PointUtils.DoesPointMaintainClearance(candidate, roomCorners, _configuration.WallClearance))
                        continue;

                    // Check minimum spacing with existing sprinklers
                    //if min distance is less than required distance, reject that candidate point
                    var minDistanceToExisting = existingSprinklers.Any()
                        ? existingSprinklers.Min(s => s.Distance2DTo(candidate))
                        : double.MaxValue;

                    //candidate is not allowed
                    if (minDistanceToExisting < _configuration.SprinklerSpacing)
                        continue;

                    //this candidate point is allowed
                    // Calculate coverage improvement score for this candidate point
                    var coverageScore = CalculateUncoveredAreaReduction(candidate, roomCorners, existingSprinklers, coverageRadius);

                    if (coverageScore > bestScore)
                    {
                        bestScore = coverageScore;
                        bestPosition = candidate;
                    }
                }
            }

            return bestPosition;
        }

        /// <summary>
        /// How much uncovered area the candidate would cover
        /// 
        /// COVERAGE SCORING ALGORITHM:
        /// 1. Generate sample points across the room (like a testing grid)
        /// 2. For each sample point, check current coverage quality
        /// 3. If poorly covered, see how much this candidate would improve it
        /// 4. Sum all improvements to get total score
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="roomCorners"></param>
        /// <param name="existingSprinklers"></param>
        /// <param name="coverageRadius"></param>
        /// <returns></returns>
        private double CalculateUncoveredAreaReduction(Point candidate, List<Point> roomCorners, List<Point> existingSprinklers, double coverageRadius)
        {

            // calculate based on distance to room areas not well covered by existing sprinklers

            var roomCentroid = _polygonService.CalculatePolygonCentroid(roomCorners);
            var samplePoints = GenerateRoomSamplePoints(roomCorners, 20); // Sample points across room

            double uncoveredAreaReduction = 0.0;

            foreach (var samplePoint in samplePoints)
            {
                // Find distance to nearest existing sprinkler
                var nearestExistingDistance = existingSprinklers.Any()
                    ? existingSprinklers.Min(s => s.Distance2DTo(samplePoint))
                    : double.MaxValue;

                // sample point is uncovered by any of the exisiting sprinklers
                // then check if the candidate point would cover the sample point
                if (nearestExistingDistance > coverageRadius)
                {
                    var candidateDistance = candidate.Distance2DTo(samplePoint);
                    if (candidateDistance <= coverageRadius)
                    {
                        // This candidate would improve coverage of this area
                        uncoveredAreaReduction += Math.Max(0.0, nearestExistingDistance - candidateDistance);
                    }
                }
            }

            return uncoveredAreaReduction;
        }

        /// <summary>
        /// Create a grid of test points that represent areas needing sprinkler coverage.
        /// This sample points will not filled with sprinkler
        /// Rather these will be under the sprinkler coverage.
        /// 
        /// HOW IT WORKS:
        /// 1. Create a regular grid across the room's bounding rectangle
        /// 2. Filter out points that fall outside the actual room shape
        /// 3. Return only points inside the room boundary
        /// </summary>
        /// <param name="roomCorners"></param>
        /// <param name="samplesPerAxis"></param>
        /// <returns></returns>
        private List<Point> GenerateRoomSamplePoints(List<Point> roomCorners, int samplesPerAxis)
        {
            var bounds = PointUtils.CalculateRoomBoundary(roomCorners);
            var samplePoints = new List<Point>();

            var stepX = (bounds.MaxX - bounds.MinX) / (samplesPerAxis - 1);
            var stepY = (bounds.MaxY - bounds.MinY) / (samplesPerAxis - 1);

            _logger.LogTrace($"Generating sample points: {samplesPerAxis}×{samplesPerAxis} grid with steps ({stepX:F1}, {stepY:F1})mm");

            for (int i = 0; i < samplesPerAxis; i++)
            {
                for (int j = 0; j < samplesPerAxis; j++)
                {
                    var x = bounds.MinX + i * stepX;
                    var y = bounds.MinY + j * stepY;
                    var point = new Point(x, y, _configuration.CeilingHeight);

                    if (PointUtils.IsPointInBoundary(point, roomCorners))
                    {
                        samplePoints.Add(point);
                    }
                }
            }

            return samplePoints;
        }
    }
}
