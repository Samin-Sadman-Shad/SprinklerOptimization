using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Obsolete
{
    internal class SprinklerCalculator
    {
        private const double WALL_CLEARANCE = 2500.0; // mm
        private const double SPRINKLER_SPACING = 2500.0; // mm
        private const double CEILING_HEIGHT = 2500.0; // mm

        private List<Point> ceilingCorners;
        private List<Pipe> waterPipes;

        /// <summary>
        /// Input provided with the assignment question
        /// </summary>
        public SprinklerCalculator()
        {
            ceilingCorners = new List<Point>
            {
                new Point(97500.00, 34000.00, 2500.00),
                new Point(85647.67, 43193.61, 2500.00),
                new Point(91776.75, 51095.16, 2500.00),
                new Point(103629.07, 41901.55, 2500.00)
            };

            waterPipes = new List<Pipe>
            {
                new Pipe(new Point(98242.11, 36588.29, 3000.00), new Point(87970.10, 44556.09, 3500.00)),
                new Pipe(new Point(99774.38, 38563.68, 3500.00), new Point(89502.37, 46531.47, 3000.00)),
                new Pipe(new Point(101306.65, 40539.07, 3000.00), new Point(91034.63, 48506.86, 3000.00))
            };
        }

        /// <summary>
        /// This constrcutor can be used for custom inputs
        /// </summary>
        /// <param name="ceilingCorners"></param>
        /// <param name="waterPipes"></param>
        public SprinklerCalculator(List<Point> ceilingCorners, List<Pipe> waterPipes)
        {
            this.ceilingCorners = ceilingCorners;
            this.waterPipes = waterPipes;
        }

        public List<Point> CalculateSprinklerPositions()
        {
            var sprinklers = new List<Point>();

            //border of the celing
            double minX = ceilingCorners.Min(p => p.X);
            double maxX = ceilingCorners.Max(p => p.X);
            double minY = ceilingCorners.Min(p => p.Y);
            double maxY = ceilingCorners.Max(p => p.Y);

            //consider wall clearance
            double startX = minX + WALL_CLEARANCE;
            double startY = minY + WALL_CLEARANCE;
            double endX = maxX - WALL_CLEARANCE;
            double endY = maxY - WALL_CLEARANCE;

            //Generate grid points
            for (double x = startX; x <= endX; x += SPRINKLER_SPACING)
            {
                for (double y = startY; y <= endY; y += SPRINKLER_SPACING)
                {
                    Point candidate = new Point(x, y, CEILING_HEIGHT);

                    if (IsPointInsideRoomWithClearance(candidate))
                    {
                        sprinklers.Add(candidate);
                    }
                }
            }

            return sprinklers;
        }

        private bool IsPointInsideRoomWithClearance(Point point)
        {
            if (!IsPointInsidePolygon(point))
                return false;

            // Check distance from all edges
            for (int i = 0; i < ceilingCorners.Count; i++)
            {
                Point p1 = ceilingCorners[i];
                Point p2 = ceilingCorners[(i + 1) % ceilingCorners.Count];

                double distance = DistanceFromPointToLineSegment(point, p1, p2);
                if (distance < WALL_CLEARANCE)
                    return false;
            }

            return true;
        }

        private bool IsPointInsidePolygon(Point point)
        {
            int intersectionCount = 0;

            for (int i = 0; i < ceilingCorners.Count; i++)
            {
                Point p1 = ceilingCorners[i];
                Point p2 = ceilingCorners[(i + 1) % ceilingCorners.Count];

                if (p1.Y <= point.Y && point.Y < p2.Y || p2.Y <= point.Y && point.Y < p1.Y)
                {
                    double intersectionX = p1.X + (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y);
                    if (point.X < intersectionX)
                        intersectionCount++;
                }
            }

            return intersectionCount % 2 == 1;
        }

        private double DistanceFromPointToLineSegment(Point point, Point lineStart, Point lineEnd)
        {
            double dx = lineEnd.X - lineStart.X;
            double dy = lineEnd.Y - lineStart.Y;
            double lengthSquared = dx * dx + dy * dy;

            if (lengthSquared == 0)
                return Math.Sqrt((point.X - lineStart.X) * (point.X - lineStart.X) +
                               (point.Y - lineStart.Y) * (point.Y - lineStart.Y));

            double t = ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / lengthSquared;
            t = Math.Max(0, Math.Min(1, t));

            double projX = lineStart.X + t * dx;
            double projY = lineStart.Y + t * dy;

            return Math.Sqrt((point.X - projX) * (point.X - projX) + (point.Y - projY) * (point.Y - projY));
        }

        private Point FindNearestPipeConnection(Point sprinkler)
        {
            Point nearestConnection = new Point(0, 0, 0);
            double minDistance = double.MaxValue;

            foreach (var pipe in waterPipes)
            {
                Point closestPoint = pipe.GetClosestPoint(sprinkler);
                double distance = sprinkler.DistanceTo(closestPoint);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestConnection = closestPoint;
                }
            }

            return nearestConnection;
        }

        private int FindNearestPipeNumber(Point sprinkler)
        {
            int closestPipe = 0;
            double minDistance = double.MaxValue;
            for (int i =0; i< waterPipes.Count; i++)
            {
                Point closestPoint = waterPipes[i].GetClosestPoint(sprinkler);
                double distance = sprinkler.DistanceTo(closestPoint);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPipe = i;
                }
            }
            return closestPipe;
        }

        public void PrintResults()
        {
            Console.WriteLine("=== SPRINKLER LAYOUT CALCULATION ===\n");

            Console.WriteLine("Room Ceiling Corners:");
            for (int i = 0; i < ceilingCorners.Count; i++)
            {
                Console.WriteLine($"  Corner {i + 1}: {ceilingCorners[i]}");
            }

            Console.WriteLine("\nWater Pipes:");
            for (int i = 0; i < waterPipes.Count; i++)
            {
                Console.WriteLine($"  Pipe {i + 1}: {waterPipes[i].Start} to {waterPipes[i].End}");
            }

            Console.WriteLine($"\nSprinkler Configuration:");
            Console.WriteLine($"  Wall clearance: {WALL_CLEARANCE} mm");
            Console.WriteLine($"  Sprinkler spacing: {SPRINKLER_SPACING} mm");
            Console.WriteLine($"  Ceiling height: {CEILING_HEIGHT} mm");

            var sprinklers = CalculateSprinklerPositions();

            Console.WriteLine($"\n=== RESULTS ===");
            Console.WriteLine($"Total number of sprinklers: {sprinklers.Count}\n");

            Console.WriteLine("Sprinkler Positions and Pipe Connections:");
            Console.WriteLine("----------------------------------------");

            for (int i = 0; i < sprinklers.Count; i++)
            {
                Point sprinkler = sprinklers[i];
                Point connection = FindNearestPipeConnection(sprinkler);
                int closestPipe = FindNearestPipeNumber(sprinkler);
                double connectionDistance = sprinkler.DistanceTo(connection);

                Console.WriteLine($"Sprinkler {i + 1,2}:");
                Console.WriteLine($"  Position:   {sprinkler}");
                Console.WriteLine($"  Pipe no: {closestPipe}");
                Console.WriteLine($"  Connection: {connection}");
                Console.WriteLine($"  Distance:   {connectionDistance:F2} mm");
                Console.WriteLine();
            }

            if (sprinklers.Count > 0)
            {
                var distances = sprinklers.Select(s => s.DistanceTo(FindNearestPipeConnection(s))).ToList();
                Console.WriteLine("Connection Statistics:");
                Console.WriteLine($"  Average connection distance: {distances.Average():F2} mm");
                Console.WriteLine($"  Maximum connection distance: {distances.Max():F2} mm");
                Console.WriteLine($"  Minimum connection distance: {distances.Min():F2} mm");
            }
        }
    }
}
