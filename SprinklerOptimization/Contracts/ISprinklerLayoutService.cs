using SprinklerOptimization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SprinklerOptimization.Enums.Enums;

namespace SprinklerOptimization.Contracts
{
    public interface ISprinklerLayoutService
    {
        /// <summary>
        /// Use grid as default strategy for layout calculation
        /// </summary>
        /// <param name="roomCorners"></param>
        /// <param name="pipes"></param>
        /// <param name="layout"></param>
        /// <returns></returns>
        SprinklerLayoutResults CalculateLayoutResults(
            List<Point> roomCorners,
            List<Pipe> pipes,
            SprinklerLayouts layout = SprinklerLayouts.Grid);

        bool ValidateLayout(SprinklerLayoutResults result, out string validationMessage);
    }
}
