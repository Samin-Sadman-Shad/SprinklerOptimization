using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Helpers
{
    public static class PointUtils
    {
        /// <summary>
        /// Return the angle between two 3d points in degrees
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double AngleBetweenPoints(Point point1, Point point2)
        {
            var dx = point2.X - point1.X;
            var dy = point2.Y - point1.Y;
            var angleInRadians = Math.Atan2(dy, dx);
            var angleInDegrees = angleInRadians * (180.0 / Math.PI);
            if (angleInDegrees < 0)
            {
                angleInDegrees += 360;
            }
            return angleInDegrees;
        }
    }
}
