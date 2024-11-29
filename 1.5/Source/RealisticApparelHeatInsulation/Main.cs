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
            UpdateAllApparelHeatInsulation();
            Log.Message("RealisticApparelHeatInsulation started. Most apparels have their heat insulation values changed!");
        }

        //Alter heat insulation for all apparels.
        public static void UpdateAllApparelHeatInsulation()
        {
            var thingDefApparels = new List<ThingDef>();
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading
                .Where(thingDef => thingDef.defName.ToLower().StartsWith("apparel"))
                .OrderBy(thingDef => thingDef.label).ToList())
            {
                foreach(var statModifier in thingDef.statBases)
                {
                    if (//Do not change the heat insulations of following apparels
                        //TODO: Some of them shall only have heat insulation in certain conditions like biome or weather.
                        thingDef.defName == "Apparel_ArmorCataphractPhoenix"
                        /*|| thingDef.defName == "Apparel_Duster"
                        || thingDef.defName == "Apparel_CowboyHat"
                        || thingDef.defName == "Apparel_Shadecone"
                        || thingDef.defName == "Apparel_HatHood"*/
                    )
                    {
                        continue;
                    }
                    //Do not change the heat insulations of apparels having Decompression Resistance (from SOS2)
                    if (ToolsApparel.HasSOS2DecompressionResistanceStat(thingDef))
                    {
                        continue;
                    }

                    //Base heat insulation depending on material (Usually clothing)
                    /*if (statModifier.stat.defName == StatDefOf.StuffEffectMultiplierInsulation_Heat.defName)
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
                    }*/

                    //Fixed heat insulation (Usually armor)
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
                }
            }
        }
    }
}
