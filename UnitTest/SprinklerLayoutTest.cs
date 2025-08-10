using Microsoft.Extensions.DependencyInjection;
using SprinklerOptimization.Contracts;
using SprinklerOptimization.Helpers;
using SprinklerOptimization.Models;
using SprinklerOptimization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using static SprinklerOptimization.Enums.Enums;

namespace UnitTest
{
    public class SprinklerLayoutTest
    {
        [Fact]
        public static void TestPoint3DMathOperations()
        {
            Console.WriteLine("Testing Point3D mathematical operations...");

            var p1 = new Point(0, 0, 0);
            var p2 = new Point(3, 4, 0);

            // Test distance calculation
            var distance = p1.DistanceTo(p2);
            var expected = 5.0; // 3-4-5 triangle
            Assert(Math.Abs(distance - expected) < 1e-9, $"Distance calculation failed. Expected: {expected}, Got: {distance}");

            // Test vector operations
            var sum = p1.Add(p2);
            var expectedSum = new Point(3, 4, 0);
            Assert(sum.Equals(expectedSum), "Vector addition failed");

            var diff = p2.Subtract(p1);
            Assert(diff.Equals(p2), "Vector subtraction failed");

            // Test dot product
            var dot = p2.DotProduct(new Point(1, 0, 0));
            Assert(Math.Abs(dot - 3.0) < 1e-9, "Dot product calculation failed");

            Console.WriteLine("✓ Point3D tests passed");
        }

        [Fact]
        /// <summary>
        /// Test geometric service functionality
        /// Validates polygon operations and spatial reasoning
        /// </summary>
        public static void TestGeometry()
        {
            Console.WriteLine("Testing Geometry calculation...");

            var geometry = new PolygonService();

            // Test simple rectangle
            var rectangle = new List<Point>
            {
                new Point(0, 0, 0),
                new Point(10, 0, 0),
                new Point(10, 10, 0),
                new Point(0, 10, 0)
            };

            // Test point inside rectangle
            var insidePoint = new Point(5, 5, 0);
            Assert(PointUtils.IsPointInBoundary(insidePoint, rectangle), "Point should be inside rectangle");

            // Test point outside rectangle
            var outsidePoint = new Point(15, 5, 0);
            Assert(!PointUtils.IsPointInBoundary(outsidePoint, rectangle), "Point should be outside rectangle");

            // Test area calculation
            var area = geometry.CalculatePolygonArea(rectangle);
            Assert(Math.Abs(area - 100.0) < 1e-9, $"Area calculation failed. Expected: 100, Got: {area}");

            // Test centroid calculation
            var centroid = geometry.CalculatePolygonCentroid(rectangle);
            var expectedCentroid = new Point(5, 5, 0);
            Assert(centroid.Distance2DTo(expectedCentroid) < 1e-9, "Centroid calculation failed");

            Console.WriteLine("✓ GeometryService tests passed");
        }

        [Fact]
        /// <summary>
        /// Test water pipe connection calculations
        /// Validates 3D spatial reasoning for pipe connections
        /// </summary>
        public static void TestWaterPipeConnections()
        {
            Console.WriteLine("Testing WaterPipe connections...");

            var pipe = new Pipe(new Point(0, 0, 3000), new Point(10, 0, 3000));

            // Test connection to point directly above pipe start
            var sprinkler1 = new Point(0, 0, 2500);
            var connection1 = pipe.GetClosestPoint(sprinkler1);
            var expectedConnection1 = new Point(0, 0, 3000);
            Assert(connection1.DistanceTo(expectedConnection1) < 1e-9, "Pipe connection calculation failed for start point");

            // Test connection to point at pipe midpoint
            var sprinkler2 = new Point(5, 2, 2500);
            var connection2 = pipe.GetClosestPoint(sprinkler2);
            var expectedConnection2 = new Point(5, 0, 3000);
            Assert(connection2.DistanceTo(expectedConnection2) < 1e-9, "Pipe connection calculation failed for midpoint");

            // Test connection beyond pipe end
            var sprinkler3 = new Point(15, 0, 2500);
            var connection3 = pipe.GetClosestPoint(sprinkler3);
            var expectedConnection3 = new Point(10, 0, 3000); // Should clamp to pipe end
            Assert(connection3.DistanceTo(expectedConnection3) < 1e-9, "Pipe connection calculation failed for end point");

            Console.WriteLine("✓ WaterPipe tests passed");
        }

        [Fact]
        /// <summary>
        /// Test sprinkler layout service integration
        /// Validates end-to-end functionality with dependency injection
        /// </summary>
        public static void TestSprinklerLayoutService()
        {
            Console.WriteLine("Testing SprinklerLayoutService integration...");

            // Setup dependencies
            var config = new SprinklerConfiguration();
            var geometry = new PolygonService();
            var metricsService = new MetricsCalculationService(config, geometry);
            var logger = new ConsoleLogger<SprinklerLayoutService>();

            var layoutService = new SprinklerLayoutService(config,  metricsService, geometry, logger);

            // Test with simple rectangular room
            var roomCorners = new List<Point>
            {
                new Point(0, 0, 2500),
                new Point(10000, 0, 2500),
                new Point(10000, 10000, 2500),
                new Point(0, 10000, 2500)
            };

            var waterPipes = new List<Pipe>
            {
                new Pipe(new Point(2000, 2000, 3000), new Point(8000, 8000, 3000))
            };

            // Test uniform grid strategy
            var result = layoutService.CalculateLayoutResults(roomCorners, waterPipes, SprinklerLayouts.Grid);

            Assert(result.IsValid, $"Layout should be valid. Validation message: {result.ValidationMessage}");
            Assert(result.SprinklerPositions.Count > 0, "Should place at least one sprinkler");
            Assert(result.ConnectionPoints.Count == result.SprinklerPositions.Count, "Should have connection for each sprinkler");

            // Test layout validation
            Assert(layoutService.ValidateLayout(result, out _), "Layout should pass validation");

            Console.WriteLine($"✓ Layout service tests passed. Placed {result.SprinklerPositions.Count} sprinklers");
        }

        [Fact]
        /// <summary>
        /// Run all unit tests
        /// Comprehensive test suite execution
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("RUNNING COMPREHENSIVE TEST SUITE");
            Console.WriteLine("================================");
            Console.WriteLine();

            try
            {
                TestPoint3DMathOperations();
                TestGeometry();
                TestWaterPipeConnections();
                TestSprinklerLayoutService();

                Console.WriteLine();
                Console.WriteLine("🎉 ALL TESTS PASSED! 🎉");
                Console.WriteLine("The sprinkler layout system is functioning correctly.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ TEST FAILED: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Simple assertion helper for unit tests
        /// </summary>
        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"Assertion failed: {message}");
            }
        }
    }
}
