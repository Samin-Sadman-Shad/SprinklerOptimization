using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SprinklerOptimization.Enums.Enums;

namespace SprinklerOptimization.Models
{
    public class SprinklerLayoutResults
    {
        public IList<Point> SprinklerPositions { get; set; } = new List<Point>();
        public IDictionary<Point, Point> ConnectionPoints { get; set; } = new Dictionary<Point, Point>();
        public SprinklerLayouts StrategyUsed { get; set; }
/*        public LayoutQualityMetrics QualityMetrics { get; set; } = new LayoutQualityMetrics();*/
/*        public double CalculationTimeMs { get; set; }*/
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; } = string.Empty;
    }
}
