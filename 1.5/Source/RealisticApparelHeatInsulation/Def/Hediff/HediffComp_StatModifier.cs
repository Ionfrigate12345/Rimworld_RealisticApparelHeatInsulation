using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        }
    }

    public class HediffCompProperties_StatModifier : HediffCompProperties
    {
        public HediffCompProperties_StatModifier()
        {
            compClass = typeof(HediffComp_StatModifier);
        }
    }
}
