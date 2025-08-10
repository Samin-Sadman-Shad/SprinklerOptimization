using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Models
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Calculate Euclidean distance between two points
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Point other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            double dz = Z - other.Z;

            return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2) + Math.Pow(dz, 2));
        }

        public double Distance2DTo(Point other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;

            return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2) );
        }

        public Point Substract(Point other) => new Point(X-other.X, Y-other.Y, Z-other.Z);
        public Point Add(Point other) => new Point(X + other.X, Y + other.Y, Z + other.Z);
        public Point Scale(double factor) => new Point(X * factor, Y * factor, Z * factor);
        public double DotProduct(Point other) => X * other.X + Y * other.Y + Z * other.Z;
        
        /// <summary>
        /// While calculating the cross product, the calling point is considered A in A*B 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Point CrossProduct(Point other) => new Point(
            x : Y * other.Z - Z * other.Y,
            y : Z * other.X - X * other.Z,
            z : X * other.Y - Y * other.X);

        public bool Equals(Point other)
        {
            var dx = Math.Abs(X - other.X);
            var dy = Math.Abs(Y - other.Y);
            var dz = Math.Abs(Z - other.Z);

            return ((dx < Double.MinValue) && (dy < Double.MinValue) && (dz < Double.MinValue));
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return (obj is Point) && Equals((Point)obj);
        }

        public override string ToString()
        {
            return $"({X:F2}, {Y:F2}, {Z:F2})";
        }
    }
}
