using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RAHI.Def.Hediff
{
    public class HediffComp_DescriptionModifier : HediffComp
    {
        public string CustomDescription { get; set; }

        public override string CompTipStringExtra
        {
            get
            {
                if (!string.IsNullOrEmpty(CustomDescription))
                {
                    return CustomDescription;
                }
                return base.CompTipStringExtra;
            }
        }
    }

    public class HediffCompProperties_DescriptionModifier : HediffCompProperties
    {
        public HediffCompProperties_DescriptionModifier()
        {
            compClass = typeof(HediffComp_DescriptionModifier);
        }
    }
}
