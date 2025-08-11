using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SprinklerOptimization.Enums.Enums;

namespace SprinklerOptimization.LayoutStrategy
{
    /// <summary>
    /// Create implementation for layouts with Factory Pattern for Open -close principle
    /// </summary>
    public static class LayoutStrategyFactory
    {
        private static readonly Dictionary<SprinklerLayouts, Func<ILayoutStrategy>> _registry = new();

        public static void Register(SprinklerLayouts layout, Func<ILayoutStrategy> creator)
        {
            _registry[layout] = creator;
        }

        public static ILayoutStrategy Create(SprinklerLayouts layout)
        {
            return _registry.TryGetValue(layout, out Func<ILayoutStrategy>? value) ? value() : null;
        }
    }
}
