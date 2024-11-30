using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOS2VEEPatch.Model;
using UnityEngine;
using Verse;

namespace SOS2VEEPatch.View
{
    public class RAHIModWindow : Mod
    {
        public readonly RAHIModSettings settings;
        public static RAHIModWindow Instance { get; private set; }

        public RAHIModWindow(ModContentPack content) : base(content)
        {
            settings = GetSettings<RAHIModSettings>();
            Instance = this;
        }

        private static Vector2 scrollPosition;

        public override void DoSettingsWindowContents(Rect rect)
        {
            Rect viewRect = new Rect(0f, 0f, rect.width - 16f, rect.height + 250f);
            Listing_Standard options = new Listing_Standard();
            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);
            options.Begin(viewRect);
            Text.Font = GameFont.Small;

            options.Label($"{"RAHI_Setting_MaxCTReductionPerVanillaBonus".Translate()}: {settings.maxCTReductionPerVanillaBonus.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTReductionPerVanillaBonus = options.Slider(settings.maxCTReductionPerVanillaBonus, 0f, 1f);
            options.Label($"{"RAHI_Setting_MaxCTReductionPerVanillaBonusHIA".Translate()}: {settings.maxCTReductionPerVanillaBonusHIA.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTReductionPerVanillaBonusHIA = options.Slider(settings.maxCTReductionPerVanillaBonusHIA, 0f, 1f);
            options.Label($"{"RAHI_Setting_MaxCTReductionPerMassKgBetween5And10".Translate()}: {settings.maxCTReductionPerMassKgBetween5And10.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTReductionPerMassKgBetween5And10 = options.Slider(settings.maxCTReductionPerMassKgBetween5And10, 0f, 2f);
            options.Label($"{"RAHI_Setting_MaxCTReductionPerMassKgAbove10".Translate()}: {settings.maxCTReductionPerMassKgAbove10.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTReductionPerMassKgAbove10 = options.Slider(settings.maxCTReductionPerMassKgAbove10, 0f, 5f);
            
            options.Label($"{"RAHI_Setting_HumidityPenaltyPerApparelBiomeTropicalRainforest".Translate()}: {settings.humidityPenaltyPerApparelBiomeTropicalRainforest.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.humidityPenaltyPerApparelBiomeTropicalRainforest = options.Slider(settings.humidityPenaltyPerApparelBiomeTropicalRainforest, 0f, 5f);
            options.Label($"{"RAHI_Setting_HumidityPenaltyPerApparelBiomeTemperateSwamp".Translate()}: {settings.humidityPenaltyPerApparelBiomeTemperateSwamp.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.humidityPenaltyPerApparelBiomeTemperateSwamp = options.Slider(settings.humidityPenaltyPerApparelBiomeTemperateSwamp, 0f, 5f);
            options.Label($"{"RAHI_Setting_HumidityPenaltyPerApparelBiomeTropicalSwamp".Translate()}: {settings.humidityPenaltyPerApparelBiomeTropicalSwamp.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.humidityPenaltyPerApparelBiomeTropicalSwamp = options.Slider(settings.humidityPenaltyPerApparelBiomeTropicalSwamp, 0f, 5f);
            options.Label($"{"RAHI_Setting_HumidityPenaltyPerApparelBiomeWetWeather".Translate()}: {settings.humidityPenaltyPerApparelBiomeWetWeather.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.humidityPenaltyPerApparelBiomeWetWeather = options.Slider(settings.humidityPenaltyPerApparelBiomeWetWeather, 0f, 5f);
            
            options.Label($"{"RAHI_Setting_MaxCTBonusGeneHeatToleranceSmallValue".Translate()}: {settings.maxCTBonusGeneHeatToleranceSmallValue.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTBonusGeneHeatToleranceSmallValue = options.Slider(settings.maxCTBonusGeneHeatToleranceSmallValue, 0f, 20f);
            options.Label($"{"RAHI_Setting_MaxCTBonusGeneHeatToleranceSmallPercentage".Translate()}: {settings.maxCTBonusGeneHeatToleranceSmallPercentage.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTBonusGeneHeatToleranceSmallPercentage = options.Slider(settings.maxCTBonusGeneHeatToleranceSmallPercentage, 0f, 1.0f);
            options.Label($"{"RAHI_Setting_MaxCTBonusGeneHeatToleranceLargeValue".Translate()}: {settings.maxCTBonusGeneHeatToleranceLargeValue.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTBonusGeneHeatToleranceLargeValue = options.Slider(settings.maxCTBonusGeneHeatToleranceLargeValue, 0f, 40f);
            options.Label($"{"RAHI_Setting_MaxCTBonusGeneHeatToleranceLargePercentage".Translate()}: {settings.maxCTBonusGeneHeatToleranceLargePercentage.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTBonusGeneHeatToleranceLargePercentage = options.Slider(settings.maxCTBonusGeneHeatToleranceLargePercentage, 0f, 1.0f);
            
            options.Label($"{"RAHI_Setting_MaxCTBonusExposedShoulders".Translate()}: {settings.maxCTBonusExposedShoulders.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTBonusExposedShoulders = options.Slider(settings.maxCTBonusExposedShoulders, 0f, 10.0f);
            options.Label($"{"RAHI_Setting_MaxCTBonusExposedArms".Translate()}: {settings.maxCTBonusExposedArms.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTBonusExposedArms = options.Slider(settings.maxCTBonusExposedArms, 0f, 10.0f);
            options.Label($"{"RAHI_Setting_MaxCTBonusExposedLegs".Translate()}: {settings.maxCTBonusExposedLegs.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTBonusExposedLegs = options.Slider(settings.maxCTBonusExposedLegs, 0f, 10.0f);
            options.Label($"{"RAHI_Setting_MaxCTBonusExposedTorso".Translate()}: {settings.maxCTBonusExposedTorso.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTBonusExposedTorso = options.Slider(settings.maxCTBonusExposedTorso, 0f, 10.0f);
            options.Label($"{"RAHI_Setting_MaxCTBonusExposedNeck".Translate()}: {settings.maxCTBonusExposedNeck.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTBonusExposedNeck = options.Slider(settings.maxCTBonusExposedNeck, 0f, 10.0f);
            options.Label($"{"RAHI_Setting_MaxCTBonusExposedMaxTotal".Translate()}: {settings.maxCTBonusExposedMaxTotal.ToStringByStyle(style: ToStringStyle.FloatOne)}");
            settings.maxCTBonusExposedMaxTotal = options.Slider(settings.maxCTBonusExposedMaxTotal, 0f, 30.0f);

            options.End();
            Widgets.EndScrollView();
            base.DoSettingsWindowContents(rect);
        }

        public override string SettingsCategory()
        {
            return "_Setting_ModName".Translate();
        }
    }
}
