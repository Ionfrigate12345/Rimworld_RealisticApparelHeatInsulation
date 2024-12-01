using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RAHI;
using Verse;
using RAHI.Def;

namespace RealisticApparelHeatInsulation.Global
{
    internal class UtilsPawn
    {
        public static float GetPawnCarryingCapacity(Pawn pawn)
        {
            var statModifierCC = pawn.def.statBases.Find(statModifier => statModifier.stat.defName == StatDefOf.CarryingCapacity.defName);
            return statModifierCC.value;
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
