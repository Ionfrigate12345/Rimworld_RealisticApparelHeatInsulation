using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SOS2VEEPatch.Model
{
    public class RAHIModSettings : ModSettings
    {
        public float maxCTReductionPerVanillaBonus = 0.2f;
        public float maxCTReductionPerMassKgBetween5And10 = 0.2f;
        public float maxCTReductionPerMassKgAbove10 = 0.5f;

        public override void ExposeData()
        {
            Scribe_Values.Look(value: ref maxCTReductionPerVanillaBonus, label: "RAHI_MaxCTReductionPerVanillaBonus", defaultValue: 0.2f);
            Scribe_Values.Look(value: ref maxCTReductionPerMassKgBetween5And10, label: "RAHI_MaxCTReductionPerMassKgBetween5And10", defaultValue: 0.2f);
            Scribe_Values.Look(value: ref maxCTReductionPerMassKgAbove10, label: "RAHI_MaxCTReductionPerMassKgAbove10", defaultValue: 0.5f);
        }
    }
}
