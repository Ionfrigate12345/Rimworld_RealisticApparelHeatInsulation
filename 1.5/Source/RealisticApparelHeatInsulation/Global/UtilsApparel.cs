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
        private static HashSet<string> validIHApparelDefNames = new HashSet<string>
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
            var valueIH = apparel.GetStatValue(StatDefOf.Insulation_Heat);
            Log.Message("Apparel " + apparel.def.defName + " Insulation_Heat stat:" + valueIH);
            return valueIH;
        }

        public static float GetApparelDefaultMinComfortableTemperatureBonus(Apparel apparel)
        {
            var valueIC = apparel.GetStatValue(StatDefOf.Insulation_Cold);
            Log.Message("Apparel " + apparel.def.defName + " Insulation_Cold stat:" + valueIC);
            return valueIC;
        }

        public static float GetApparelMassKg(Apparel apparel)
        {
            return apparel.GetStatValue(StatDefOf.Mass);
        }

        public static List<Apparel> GetAllHeatInsulationClothingsOnPawn(Pawn pawn)
        {
            var apparels = pawn.apparel.WornApparel.Where(apparel => validIHApparelDefNames.Contains(apparel.def.defName)).ToList();
            return apparels;
        }

        public static List<Apparel> GetAllEligibleNonHeatInsulationClothingsOnPawn(Pawn pawn)
        {
            var apparels = pawn.apparel.WornApparel.Where(apparel => !validIHApparelDefNames.Contains(apparel.def.defName)
                && apparel.def.defName != "Apparel_ArmorCataphractPhoenix" //Phoenix Armor excluded
                && !HasSOS2DecompressionResistanceStat(apparel) //Apparels with SOS2 EVA property excluded
                ).ToList();
            return apparels;
        }
    }
}
