using SprinklerOptimization.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Services
{
    public class SprinklerConfiguration : ISprinklerConfiguration
    {
        public double WallClearance { get; set; } = 2500;

        public double SprinklerSpacing { get; set; } = 2500;

        public double CeilingHeight { get; set; } = 2500;

        public double MinimumCoverageRadius { get; set; } = 1800;

        public double MaximumConnectionDistance { get; set; } = 5000;
    }
}
