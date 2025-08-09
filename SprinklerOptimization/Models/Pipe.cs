using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Models
{
    public class Pipe
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        public double Length => Start.DistanceTo(End);
        public Point Direction => End.Substract(Start);

        public Point MidPoint
        {
            get
            {
                var x = (Start.X + End.X) / 2;
                var y = (Start.Y + End.Y) / 2;
                var z = (Start.Z + End.Z) / 2;
                return new Point(x, y, z);
            }
        }

        public Pipe(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Find the closest point on the pipe to a given sprinker position
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculate the shortest distance from a point to this pipe
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public double ShortestDistanceToPoint(Point point)
        {
            if(point != null)
            {
                return point.DistanceTo(GetClosestPoint(point));
            }
            else
            {
                throw new ArgumentNullException("point can not be null");
            }
            
        }

        public override string ToString() => $"Pipe: {Start} → {End}";
    }
}
