using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.LayoutStrategy
{
    public interface ILayoutStrategy
    {
        List<Point> CalculateSprinklerPosition(List<Point> roomCorners, List<Pipe> pipes);
    }
}
