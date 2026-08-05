using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RAHI.Def
{
    [DefOf]
    public static class RAHIDefOf
    {
        //Biomes
        public static BiomeDef TropicalRainforest;
        public static BiomeDef TropicalSwamp;
        public static BiomeDef ExtremeDesert;
        public static BiomeDef AridShrubland;
        public static BiomeDef TemperateSwamp;

        //Weathers
        public static WeatherDef Rain;
        public static WeatherDef Fog;
        public static WeatherDef RainyThunderstorm;
        public static WeatherDef FoggyRain;

        //Genes (Biotech DLC)
        [MayRequireBiotech]
        public static GeneDef FireResistant;
        [MayRequireBiotech]
        public static GeneDef MaxTemp_SmallIncrease;
        [MayRequireBiotech]
        public static GeneDef MaxTemp_LargeIncrease;

        //Hediff
        public static HediffDef RAHI_AdjustedMaxCT;
        public static HediffDef RAHI_ExposedBodyPartsEfficiencyBoost;
    }
}
