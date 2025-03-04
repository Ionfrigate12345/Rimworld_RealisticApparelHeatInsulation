using System;
using System.Collections.Generic;
using System.Linq;
using RAHI.View;
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
            if(ModsConfig.IsActive("kentington.saveourship2") || RAHIModWindow.Instance.settings.ignoreSOS2EVAApparelsWithoutSOS2Installed)
            {
                return apparel.def.comps.Any(comp => comp.compClass?.FullName == "SaveOurShip2.CompEVA");
            }
            return false;
        }

        public static bool IsBaseClothingWithoutPenalty(Apparel apparel)
        {
            return apparel.def.defName == "Apparel_BasicShirt"
                || apparel.def.defName == "Apparel_Pants"
                || apparel.def.defName == "Apparel_Corset"
                || apparel.def.defName == "Apparel_Tribal"
                || apparel.def.defName == "Apparel_KidTribal"
                //Vanilla Apparels Expanded 
                || apparel.def.defName == "VAE_Apparel_CasualTShirt"
                || apparel.def.defName == "VAE_Apparel_Shorts"
                || apparel.def.defName == "VAE_Apparel_Skirt"
                || apparel.def.defName == "VAE_Apparel_TankTop"
                ;
        }

        public static bool IsVEApparelFootwear(Apparel apparel)
        {
            return apparel.def.defName.StartsWith("VAE_Footwear_");
        }

        public static bool IsVEApparelHandwear(Apparel apparel)
        {
            return apparel.def.defName.StartsWith("VAE_Handwear_");
        }

        public static float GetApparelDefaultMaxComfortableTemperatureBonus(Apparel apparel)
        {
            var valueIH = apparel.GetStatValue(StatDefOf.Insulation_Heat);
            return valueIH;
        }

        public static float GetApparelDefaultMinComfortableTemperatureBonus(Apparel apparel)
        {
            var valueIC = apparel.GetStatValue(StatDefOf.Insulation_Cold);
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
