using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RAHI.Def.Hediff
{
    public class HediffComp_StatModifier : HediffComp
    {
        public HediffCompProperties_StatModifier Props => (HediffCompProperties_StatModifier)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Pawn != null)
            {
                Pawn.GetStatValue(StatDefOf.ComfyTemperatureMax, true);
            }
        }
    }

    public class HediffCompProperties_StatModifier : HediffCompProperties
    {
        public List<StatModifier> statOffsets = new List<StatModifier>();

        public HediffCompProperties_StatModifier()
        {
            compClass = typeof(HediffComp_StatModifier);
        }
    }
}
