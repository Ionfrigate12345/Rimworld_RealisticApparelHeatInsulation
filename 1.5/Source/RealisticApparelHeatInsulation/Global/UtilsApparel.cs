using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RealisticApparelHeatInsulation.Global
{
    internal class UtilsApparel
    {
        private static HashSet<string> validApparelDefNames = new HashSet<string>
        {
            "Apparel_Duster",
            "Apparel_CowboyHat",
            "Apparel_Shadecone",
            "Apparel_HatHood"
        };

        public static bool HasSOS2DecompressionResistanceStat(Apparel apparel)
        {
            return apparel.def.comps.Any(comp => comp.compClass?.FullName == "SaveOurShip2.CompEVA");
        }
        public static bool IsBaseClothingWithoutPenalty(Apparel apparel)
        {
            return apparel.def.defName == "Apparel_BasicShirt"
                || apparel.def.defName == "Apparel_Pants"
                || apparel.def.defName == "Apparel_Corset"
                || apparel.def.defName == "Apparel_Tribal"
                || apparel.def.defName == "Apparel_KidTribal"
                ;
        }
        public static float GetApparelDefaultMaxComfortableTemperatureBonus(Apparel apparel)
        {
            var statModifierIH = apparel.def.statBases.Find(statModifier => statModifier.stat.defName == StatDefOf.Insulation_Heat.defName);

            if(statModifierIH == null)
            {
                Log.Warning("Apparel " + apparel.def.defName + " Doesnt have stat Insulation_Heat.");
                return 0;
            }
            else
            {
                Log.Message("Apparel " + apparel.def.defName + "Insulation_Heat stat:" + statModifierIH.value);
            }
            return statModifierIH.value;
        }

        public static float GetApparelDefaultMinComfortableTemperatureBonus(Apparel apparel)
        {
            var statModifierIC = apparel.def.statBases.Find(statModifier => statModifier.stat.defName == StatDefOf.Insulation_Cold.defName);

            if (statModifierIC == null)
            {
                Log.Warning("Apparel " + apparel.def.defName + " Doesnt have stat Insulation_Cold.");
                return 0;
            }
            else
            {
                Log.Message("Apparel " + apparel.def.defName + "Insulation_Cold stat:" + statModifierIC.value);
            }
            return statModifierIC.value;
        }

        public static float GetApparelMassKg(Apparel apparel)
        {
            var statModifierIH = apparel.def.statBases.Find(statModifier => statModifier.stat.defName == StatDefOf.Mass.defName);
            return statModifierIH.value;
        }

        public static List<Apparel> GetAllHeatInsulationClothingsOnPawn(Pawn pawn)
        {
            var apparels = pawn.apparel.WornApparel;
            return apparels.Where(apparel => validApparelDefNames.Contains(apparel.def.defName)).ToList();
        }

        public static List<Apparel> GetAllEligibleNonHeatInsulationClothingsOnPawn(Pawn pawn)
        {
            var apparels = pawn.apparel.WornApparel;
            return apparels.Where(apparel => !validApparelDefNames.Contains(apparel.def.defName) 
                && apparel.def.defName != "Apparel_ArmorCataphractPhoenix" //Phoenix Armor excluded
                && !HasSOS2DecompressionResistanceStat(apparel) //Apparels with SOS2 EVA property excluded
                ).ToList();
        }
    }
}
