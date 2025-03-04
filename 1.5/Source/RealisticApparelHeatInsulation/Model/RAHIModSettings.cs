using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RAHI.Model
{
    public class RAHIModSettings : ModSettings
    {
        public float maxCTReductionPerVanillaBonus = 0.2f;
        public float maxCTReductionPerVanillaBonusHIA = 0.1f; //Heat Insulation Apparel like duster will suffer this penalty with humidity penalty.
        public float maxCTReductionPerMassKgBetween5And10 = 0.2f;
        public float maxCTReductionPerMassKgAbove10 = 0.5f;

        public float humidityPenaltyPerApparelBiomeTropicalRainforest = 0.5f;
        public float humidityPenaltyPerApparelBiomeTemperateSwamp = 1.0f;
        public float humidityPenaltyPerApparelBiomeTropicalSwamp = 1.5f;
        public float humidityPenaltyPerApparelBiomeWetWeather = 1.0f;

        public float maxCTBonusGeneHeatToleranceSmallValue = 5.0f;
        public float maxCTBonusGeneHeatToleranceSmallPercentage = 0.25f;
        public float maxCTBonusGeneHeatToleranceLargeValue = 10.0f;
        public float maxCTBonusGeneHeatToleranceLargePercentage = 0.5f;

        public float maxCTBonusExposedShoulders = 4.0f;
        public float maxCTBonusExposedArms = 4.0f;
        public float maxCTBonusExposedLegs = 4.0f;
        public float maxCTBonusExposedTorso = 6.0f;
        public float maxCTBonusExposedNeck = 2.0f;
        public float maxCTBonusExposedMaxTotal = 15.0f;

        public bool ignoreSOS2EVAApparelsWithoutSOS2Installed = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(value: ref maxCTReductionPerVanillaBonus, label: "RAHI_Setting_MaxCTReductionPerVanillaBonus", defaultValue: 0.2f);
            Scribe_Values.Look(value: ref maxCTReductionPerVanillaBonusHIA, label: "RAHI_Setting_MaxCTReductionPerVanillaBonusHIA", defaultValue: 0.1f);
            Scribe_Values.Look(value: ref maxCTReductionPerMassKgBetween5And10, label: "RAHI_Setting_MaxCTReductionPerMassKgBetween5And10", defaultValue: 0.2f);
            Scribe_Values.Look(value: ref maxCTReductionPerMassKgAbove10, label: "RAHI_Setting_MaxCTReductionPerMassKgAbove10", defaultValue: 0.5f);

            Scribe_Values.Look(value: ref humidityPenaltyPerApparelBiomeTropicalRainforest, label: "RAHI_Setting_HumidityPenaltyPerApparelBiomeTropicalRainforest", defaultValue: 1.0f);
            Scribe_Values.Look(value: ref humidityPenaltyPerApparelBiomeTemperateSwamp, label: "RAHI_Setting_HumidityPenaltyPerApparelBiomeTemperateSwamp", defaultValue: 1.5f);
            Scribe_Values.Look(value: ref humidityPenaltyPerApparelBiomeTropicalSwamp, label: "RAHI_Setting_HumidityPenaltyPerApparelBiomeTropicalSwamp", defaultValue: 2.0f);
            Scribe_Values.Look(value: ref humidityPenaltyPerApparelBiomeWetWeather, label: "RAHI_Setting_HumidityPenaltyPerApparelBiomeWetWeather", defaultValue: 1.5f);

            Scribe_Values.Look(value: ref maxCTBonusGeneHeatToleranceSmallValue, label: "RAHI_Setting_MaxCTBonusGeneHeatToleranceSmallValue", defaultValue: 5.0f);
            Scribe_Values.Look(value: ref maxCTBonusGeneHeatToleranceSmallPercentage, label: "RAHI_Setting_MaxCTBonusGeneHeatToleranceSmallPercentage", defaultValue: 0.25f);
            Scribe_Values.Look(value: ref maxCTBonusGeneHeatToleranceLargeValue, label: "RAHI_Setting_MaxCTBonusGeneHeatToleranceLargeValue", defaultValue: 10.0f);
            Scribe_Values.Look(value: ref maxCTBonusGeneHeatToleranceLargePercentage, label: "RAHI_Setting_MaxCTBonusGeneHeatToleranceLargePercentage", defaultValue: 0.5f);
            
            Scribe_Values.Look(value: ref maxCTBonusExposedShoulders, label: "RAHI_Setting_MaxCTBonusExposedShoulders", defaultValue: 4.0f);
            Scribe_Values.Look(value: ref maxCTBonusExposedArms, label: "RAHI_Setting_MaxCTBonusExposedArms", defaultValue: 4.0f);
            Scribe_Values.Look(value: ref maxCTBonusExposedLegs, label: "RAHI_Setting_MaxCTBonusExposedLegs", defaultValue: 4.0f);
            Scribe_Values.Look(value: ref maxCTBonusExposedTorso, label: "RAHI_Setting_MaxCTBonusExposedTorso", defaultValue: 6.0f);
            Scribe_Values.Look(value: ref maxCTBonusExposedNeck, label: "RAHI_Setting_MaxCTBonusExposedNeck", defaultValue: 2.0f);
            Scribe_Values.Look(value: ref maxCTBonusExposedMaxTotal, label: "RAHI_Setting_MaxCTBonusExposedMaxTotal", defaultValue: 15.0f);

            Scribe_Values.Look(value: ref ignoreSOS2EVAApparelsWithoutSOS2Installed, label: "RAHI_Setting_IgnoreSOS2DecompressionApparelWithoutModInstalled", defaultValue: true);
        }
    }
}
