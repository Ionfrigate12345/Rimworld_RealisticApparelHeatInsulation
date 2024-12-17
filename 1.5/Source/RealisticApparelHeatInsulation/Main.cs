using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RealisticApparelHeatInsulation.Global;
using Verse;

namespace RealisticApparelHeatInsulation
{
    [StaticConstructorOnStartup]
    public static class Main
    {
        static Main() //our constructor
        {
            //UpdateAllApparelHeatInsulation();
            Log.Message("RealisticApparelHeatInsulation started. Most apparels may have their heat insulation values changed!");
        }

        //Alter heat insulation for all apparels.
        /*public static void UpdateAllApparelHeatInsulation()
        {
            var thingDefApparels = new List<ThingDef>();
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading
                .Where(thingDef => thingDef.defName.ToLower().StartsWith("apparel"))
                .OrderBy(thingDef => thingDef.label).ToList())
            {
                foreach (var statModifier in thingDef.statBases)
                {
                    if (//Do not change the heat insulations of following apparels
                        thingDef.defName == "Apparel_ArmorCataphractPhoenix"

                        //Exclude these heat insulation clothings so that they still work like vanilla
                        || thingDef.defName == "Apparel_Duster"
                        || thingDef.defName == "Apparel_CowboyHat"
                        || thingDef.defName == "Apparel_Shadecone"
                        || thingDef.defName == "Apparel_HatHood"
                    )
                    {
                        continue;
                    }

                    //Do not change the heat insulations of apparels having Decompression Resistance (from SOS2)
                    if (UtilsApparel.HasSOS2DecompressionResistanceStat(thingDef))
                    {
                        continue;
                    }

                    //Fixed heat insulation without material differences (Usually armor)
                    if (statModifier.stat.defName == StatDefOf.Insulation_Heat.defName)
                    {
                        if (thingDef.defName.ToLower().StartsWith("Apparel_Flak"))
                        {
                            statModifier.value = -1;
                        }
                        else
                        {
                            statModifier.value = -10;
                        }
                    }

                    //Base heat insulation depending on material set to 0 for all other clothings except excluded ones.
                    if (statModifier.stat.defName == StatDefOf.StuffEffectMultiplierInsulation_Heat.defName)
                    {
                        statModifier.value = 0;
                    }
                    if (statModifier.stat.defName == StatDefOf.StuffEffectMultiplierInsulation_Heat.defName)
                    {
                        if (thingDef.defName == "Apparel_Parka"
                            || thingDef.defName.ToLower().StartsWith("Apparel_KidParka"))
                        {
                            statModifier.value = -0.6f;
                        }
                        else if (thingDef.defName == "Apparel_Corset")
                        {
                            statModifier.value = 0;
                        }
                        else if(thingDef.defName.ToLower().StartsWith("Apparel_Tribal") 
                            || thingDef.defName.ToLower().StartsWith("Apparel_KidTribal"))
                        {
                            statModifier.value = 0;
                        }
                        else
                        {
                            statModifier.value = -0.1f;
                        }
                    }
                }
            }
        }*/
    }
}
