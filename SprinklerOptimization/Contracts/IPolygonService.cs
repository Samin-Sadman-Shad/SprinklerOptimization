using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Contracts
{
    public interface IPolygonService
    {
        double CalculatePolygonArea(List<Point> polygon);
        Point CalculatePolygonCentroid(List<Point> polygon);
        List<Point> CreateOffsetPolygon(List<Point> original, double offset);
    }
}
