using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Models
{
    internal struct Pipe
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        public Pipe(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public Point GetClosestPoint(Point point)
        {
            double dx = End.X - Start.X;
            double dy = End.Y - Start.Y;
            double dz = End.Z - Start.Z;

            double t = ((point.X - Start.X) * dx + (point.Y - Start.Y) * dy + (point.Z - Start.Z) * dz) /
                      (dx * dx + dy * dy + dz * dz);

            t = Math.Max(0, Math.Min(1, t));

            return new Point(
                Start.X + t * dx,
                Start.Y + t * dy,
                Start.Z + t * dz
            );
        }
    }
}
