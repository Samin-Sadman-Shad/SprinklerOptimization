using SprinklerOptimization.Models;
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

        public static (double MinX, double MinY, double MaxX,  double MaxY) CalculateRoomBoundary(List<Point> roomCorners)
        {
            if(roomCorners.Count <= 0)
            {
                return (0.0,0.0, 0.0, 0.0);
            }

            return (
                roomCorners.Min(c => c.X),
                roomCorners.Min(c => c.Y),
                roomCorners.Max(c => c.X),
                roomCorners.Max(c => c.Y));
        }

        /// <summary>
        /// Determine if a point maintains the given distance from all the edges
        /// </summary>
        /// <param name="point"></param>
        /// <param name="polygon"></param>
        /// <param name="clearnce"></param>
        /// <returns></returns>
        public static bool DoesPointMaintainClearance(Point point, List<Point> polygon, double clearance)
        {
            for(int i = 0; i<polygon.Count; i++)
            {
                int j = (i + 1) % polygon.Count;
                if (DistanceFromPointToLine(point, polygon[i], polygon[j]) < clearance)
                    return false;
            }

            return true;
        }

        public static double DistanceFromPointToLine(Point point, Point start, Point end)
        {
            if(point is null)
            {
                throw new ArgumentNullException($"{nameof(point)} can not be null");
            }
            var lineSegment = end.Substract(start);
            var toPoint = point.Substract(start);

            var segmentLengthSquare = lineSegment.DotProduct(lineSegment);

            //line segment is just a point
            if(segmentLengthSquare < Double.MinValue)
            {
                return point.Distance2DTo(start);
            }

            var projection = (toPoint.DotProduct(lineSegment)) / segmentLengthSquare;

            //clamp projection between 0 and 1
            projection = Math.Max(0, Math.Min(0, projection));

            var closest = start.Add(lineSegment.Scale(projection));

            return point.Distance2DTo(closest);
        }

        public static bool IsPointInBoundary(Point point, List<Point> boundary)
        {
            if(boundary == null || boundary.Count < 3)
            {
                throw new ArgumentException("At least 3 points needed for the boundary");
            }

            bool inside = false;
            int j = boundary.Count - 1;

            try
            {
                for (int i = 0; i < boundary.Count; i++)
                {
                    //consider the edge P[i] ----> p[j]
                    Point current = boundary[i];
                    Point previous = boundary[j];

                    //check if the given point's coordinate lies between the edge vertexes
                    var yBetween = (previous.Y > point.Y) != (current.Y > point.Y);
                    if (yBetween)
                    {
                        //can cause divided by zero error
                        double xAtIntersection = previous.X + (point.Y - current.Y) * (previous.X - current.X) / (previous.Y - current.Y);
                        if (xAtIntersection > point.X)
                        {
                            // for every intersection, flip the result
                            //even times -> outside
                            //odd times -> inside
                            inside = !inside;
                        }
                    }
                    //consider the current vertex as previous for the next loop
                    j = i;
                }
            }
            catch(DivideByZeroException ex)
            {
                throw new Exception("boundary coordinates are not valid");
            }
            return inside;
        }
    }
}
