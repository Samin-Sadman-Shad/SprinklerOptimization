using SprinklerOptimization.Contracts;
using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Services
{
    public class PolygonService : IPolygonService
    {
        public double CalculatePolygonArea(List<Point> polygon)
        {
            if (polygon.Count < 3) return 0;

            double area = 0;
            for (int i = 0; i < polygon.Count; i++)
            {
                int j = (i + 1) % polygon.Count;
                area += polygon[i].X * polygon[j].Y - polygon[j].X * polygon[i].Y;
            }

            return Math.Abs(area) / 2.0;
        }

        /// <summary>
        /// use standard shoelace / polygon centroid formula
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Point CalculatePolygonCentroid(List<Point> polygon)
        {
            if (polygon.Count < 3)
                throw new ArgumentException("Polygon does not contain sufficent points");
                //return polygon.Count > 0 ? polygon[0] : new Point(0, 0, 0);

            double area = CalculatePolygonArea(polygon);
            if (area < 1e-9) // Degenerate polygon, area is zero
                //average of vertices
                return new Point(polygon.Average(p => p.X), polygon.Average(p => p.Y), polygon[0].Z);

            double cx = 0, cy = 0;
            for (int i = 0; i < polygon.Count; i++)
            {
                int j = (i + 1) % polygon.Count;
                var cross = polygon[i].X * polygon[j].Y - polygon[j].X * polygon[i].Y;
                cx += (polygon[i].X + polygon[j].X) * cross;
                cy += (polygon[i].Y + polygon[j].Y) * cross;
            }

            var factor = 1.0 / (6.0 * area);
            return new Point(cx * factor, cy * factor, polygon[0].Z);
        }

        /// <summary>
        /// Used for creating the valid sprinkler positioning area with wall clearances
        /// </summary>
        /// <param name="original"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public List<Point> CreateOffsetPolygon(List<Point> original, double offset)
        {
            // Simplified implementation: move each vertex inward along angle bisector
            var offsetVertices = new List<Point>();

            for (int i = 0; i < original.Count; i++)
            {
                var prev = original[(i - 1 + original.Count) % original.Count];
                var current = original[i];
                var next = original[(i + 1) % original.Count];

                // Calculate angle bisector direction
                var toPrev = prev.Subtract(current);
                var toNext = next.Subtract(current);

                // Normalize vectors
                var toPrevLen = Math.Sqrt(toPrev.DotProduct(toPrev));
                var toNextLen = Math.Sqrt(toNext.DotProduct(toNext));

                if (toPrevLen > 1e-9) toPrev = toPrev.Scale(1.0 / toPrevLen);
                if (toNextLen > 1e-9) toNext = toNext.Scale(1.0 / toNextLen);

                // Bisector direction (inward)
                var bisector = toPrev.Add(toNext);
                var bisectorLen = Math.Sqrt(bisector.DotProduct(bisector));

                if (bisectorLen > 1e-9)
                {
                    bisector = bisector.Scale(1.0 / bisectorLen);
                    var offsetPoint = current.Add(bisector.Scale(offset));
                    offsetVertices.Add(offsetPoint);
                }
                else
                {
                    offsetVertices.Add(current); // Fallback for degenerate cases
                }
            }

            return offsetVertices;
        }
    }
}
