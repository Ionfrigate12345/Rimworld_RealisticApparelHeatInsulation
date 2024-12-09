using System.Linq;
using RimWorld;
using Verse;
using RAHI.Def;

namespace RealisticApparelHeatInsulation.Global
{
    internal class UtilsPawn
    {
        public static float GetPawnCarryingCapacity(Pawn pawn)
        {
            var valueCC = pawn.GetStatValue(StatDefOf.CarryingCapacity);
            return valueCC;
        }

        public static bool HasGeneHeatToleranceSmall(Pawn pawn)
        {
            if(!ModLister.BiotechInstalled)
            {
                return false;
            }
            return pawn.genes.Xenogenes.Any(gene => gene.def.defName == RAHIDefOf.MaxTemp_SmallIncrease.defName);
        }
        public static bool HasGeneHeatToleranceLarge(Pawn pawn)
        {
            if (!ModLister.BiotechInstalled)
            {
                return false;
            }
            return pawn.genes.Xenogenes.Any(gene => gene.def.defName == RAHIDefOf.MaxTemp_LargeIncrease.defName);
        }
        public static bool HasGeneFireTolerance(Pawn pawn)
        {
            if (!ModLister.BiotechInstalled)
            {
                return false;
            }
            return pawn.genes.Xenogenes.Any(gene => gene.def.defName == RAHIDefOf.FireResistant.defName);
        }
    }
}
