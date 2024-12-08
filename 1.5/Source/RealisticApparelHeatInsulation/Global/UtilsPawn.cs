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
            if (pawn.def.statBases == null)
            {
                Log.Warning("RAHI: Pawn " + pawn.Name + " doesnt have valid statBases! Using default value 35.");
                return 35;
            }
            var statModifierCC = pawn.def.statBases.Find(statModifier => statModifier.stat.defName == StatDefOf.CarryingCapacity.defName);
            if (statModifierCC == null)
            {
                Log.Warning("RAHI: Pawn " + pawn.Name + " does not have CarryingCapacity stat!  Using default value 35.");
                return 35;
            }

            Log.Message("RAHI: Pawn " + pawn.Name + " carrying capacity:" + statModifierCC.value);
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
