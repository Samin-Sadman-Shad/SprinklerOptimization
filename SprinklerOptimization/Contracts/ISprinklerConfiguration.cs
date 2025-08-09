using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerOptimization.Contracts
{
    public interface ISprinklerConfiguration
    {
        double WallClearance { get; }
        double SprinklerSpacing { get; }
        double CeilingHeight { get; }
        double MinimumCoverageRadius { get; }
        double MaximumConnectionDistance { get; }
    }
}
