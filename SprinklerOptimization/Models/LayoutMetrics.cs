using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Models
{
    public class LayoutMetrics
    {
        public int TotalSprinklers { get; set; }
        //public double RoomArea { get; set; }
        public double AverageSpacingDistance { get; set; }
        public double AverageConnectionDistance { get; set; }
        public double MaxConnectionDistance { get; set; }
        public double MinConnectionDistance { get; set; }

        public double CoveragePerSprinkler { get; set; }
        public double CoverageEfficiency { get; set; }
    }
}
