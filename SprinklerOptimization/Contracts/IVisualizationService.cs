using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Contracts
{
    public interface IVisualizationService
    {
        string GenerateAsciiVisualization(SprinklerLayoutResults result, List<Point> roomCorners, List<Pipe> waterPipes);
    }
}
