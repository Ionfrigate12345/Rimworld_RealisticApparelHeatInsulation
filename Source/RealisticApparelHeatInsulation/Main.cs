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
            //UpdateAllApparelHeatInsulation();
        }

        //Alter certain apparels.
        /*public static void UpdateAllApparelHeatInsulation()
        {
            var thingDefApparels = new List<ThingDef>();
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading
                .Where(thingDef => thingDef.defName.ToLower().StartsWith("apparel"))
                .OrderBy(thingDef => thingDef.label).ToList())
            {
                if (thingDef.defName == "VAE_Apparel_Shorts" || thingDef.defName == "VAE_Apparel_Skirt") 
                {
                }
            }
        }*/
    }
}
