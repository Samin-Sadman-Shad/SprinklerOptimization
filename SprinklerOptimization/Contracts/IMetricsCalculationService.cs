using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Contracts
{
    public interface IMetricsCalculationService
    {
        LayoutMetrics CalculateLayoutMetrics(
            List<Point> sprinklers,
            IDictionary<Point, Point> connections,
            List<Point> roomCorners);
    }
}
