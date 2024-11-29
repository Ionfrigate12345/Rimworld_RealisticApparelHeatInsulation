using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RealisticApparelHeatInsulation.Global
{
    internal class UtilsApparel
    {
        public static bool HasSOS2DecompressionResistanceStat(ThingDef thingDef)
        {
             return thingDef.comps.Any(comp => comp.compClass?.FullName == "SaveOurShip2.CompEVA");
        }
    }
}
