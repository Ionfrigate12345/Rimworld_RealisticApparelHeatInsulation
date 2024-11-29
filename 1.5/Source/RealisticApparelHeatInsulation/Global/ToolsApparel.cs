using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RealisticApparelHeatInsulation.Global
{
    internal class ToolsApparel
    {

        public static bool HasSOS2DecompressionResistanceStat(ThingDef thingDef)
        {
            if (!ModsConfig.IsActive("kentington.saveourship2"))
            {
                return false;
            }
            return thingDef.equippedStatOffsets.Where(offset => offset.stat.defName == "DecompressionResistance").ToList().Count > 0;
        }
    }
}
