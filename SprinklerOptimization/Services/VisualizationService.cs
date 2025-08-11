using SprinklerOptimization.Contracts;
using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Services
{
    public class VisualizationService : IVisualizationService
    {
        private readonly IPolygonService _geometry;

        public VisualizationService(IPolygonService geometry)
        {
            _geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
        }

        /// <summary>
        /// Generate ASCII art visualization of the sprinkler layout
        /// Useful for quick visual validation and debugging
        /// </summary>
        public string GenerateAsciiVisualization(SprinklerLayoutResults result, List<Point> roomCorners, List<Pipe> waterPipes)
        {
            if (!roomCorners.Any()) return "No room data available";

            var sb = new StringBuilder();
            sb.AppendLine("SPRINKLER LAYOUT VISUALIZATION");
            sb.AppendLine("===============================");
            sb.AppendLine();

            // Calculate bounds
            var minX = roomCorners.Min(p => p.X);
            var maxX = roomCorners.Max(p => p.X);
            var minY = roomCorners.Min(p => p.Y);
            var maxY = roomCorners.Max(p => p.Y);

            const int width = 80;
            const int height = 25;

            var scaleX = (width - 2) / (maxX - minX);
            var scaleY = (height - 2) / (maxY - minY);

            // Initialize grid
            var grid = new char[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    grid[i, j] = ' ';

            // Draw room boundary
            for (int i = 0; i < roomCorners.Count; i++)
            {
                var p1 = roomCorners[i];
                var p2 = roomCorners[(i + 1) % roomCorners.Count];
                DrawLine(grid, p1, p2, minX, minY, scaleX, scaleY, width, height, '█');
            }

            // Draw water pipes
            foreach (var pipe in waterPipes)
            {
                DrawLine(grid, pipe.Start, pipe.End, minX, minY, scaleX, scaleY, width, height, '║');
            }
            char sprinklerChar = '⬤';
            int radius = 1;
            // Draw sprinklers
            foreach (var sprinkler in result.SprinklerPositions)
            {
                var x = (int)Math.Round((sprinkler.X - minX) * scaleX);
                var y = (int)Math.Round((sprinkler.Y - minY) * scaleY);

                if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
                {
                    //grid[height - 1 - y, x] = '◎'; // Flip Y for proper display
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        for (int dx = -radius; dx <= radius; dx++)
                        {
                            double adjDy = dy * 0.5; // Fix aspect ratio
                            if (Math.Sqrt(dx * dx + adjDy * adjDy) <= radius)
                            {
                                int gx = x + dx;
                                int gy = height - 1 - y + dy;
                                if (gx >= 0 && gx < width && gy >= 0 && gy < height)
                                    grid[gy, gx] = sprinklerChar;
                            }
                        }
                    }
                }
                    
            }

            // Convert grid to string
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                    sb.Append(grid[i, j]);
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("Legend: █ = Room walls, ║ = Water pipes, ● = Sprinklers");
            sb.AppendLine();

            return sb.ToString();
        }


        /// <summary>
        /// Draw a line on the ASCII grid using Bresenham-like algorithm
        /// </summary>
        private void DrawLine(char[,] grid, Point p1, Point p2, double minX, double minY,
                             double scaleX, double scaleY, int width, int height, char symbol)
        {
            var x1 = (int)Math.Round((p1.X - minX) * scaleX);
            var y1 = (int)Math.Round((p1.Y - minY) * scaleY);
            var x2 = (int)Math.Round((p2.X - minX) * scaleX);
            var y2 = (int)Math.Round((p2.Y - minY) * scaleY);

            var dx = Math.Abs(x2 - x1);
            var dy = Math.Abs(y2 - y1);
            var steps = Math.Max(dx, dy);

            if (steps == 0) return;

            for (int i = 0; i <= steps; i++)
            {
                var x = x1 + (x2 - x1) * i / steps;
                var y = y1 + (y2 - y1) * i / steps;

                if (x >= 0 && x < width && y >= 0 && y < height)
                    grid[height - 1 - y, x] = symbol; // Flip Y coordinate
            }
        }
    }
}
