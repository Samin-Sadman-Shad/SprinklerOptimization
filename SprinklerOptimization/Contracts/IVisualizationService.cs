using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = SprinklerOptimization.Models.Point;

namespace SprinklerOptimization.Contracts
{
    public interface IVisualizationService
    {
        /*string GenerateAsciiVisualization(SprinklerLayoutResults result, List<Point> roomCorners, List<Pipe> waterPipes);*/
        public void Generate2DVisualization(
        SprinklerLayoutResults result,
        List<Point> roomCorners,
        List<Pipe> waterPipes,
        string outputFilePath);

/*        void ShowPlot(
        SprinklerLayoutResults result,
        List<Point> roomCorners,
        List<Pipe> waterPipes);*/
    }
}
