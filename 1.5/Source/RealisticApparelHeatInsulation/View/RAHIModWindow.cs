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
            options.Label($"{"RAHI_Setting_MaxCTReductionPerVanillaBonus".Translate()}: {settings.maxCTReductionPerVanillaBonus.ToStringByStyle(style: ToStringStyle.FloatThree)}");
            settings.maxCTReductionPerVanillaBonus = options.Slider(settings.maxCTReductionPerVanillaBonus, 0f, 1f);
            options.Label($"{"RAHI_Setting_MaxCTReductionPerMassKgBetween5And10".Translate()}: {settings.maxCTReductionPerMassKgBetween5And10.ToStringByStyle(style: ToStringStyle.FloatThree)}");
            settings.maxCTReductionPerMassKgBetween5And10 = options.Slider(settings.maxCTReductionPerMassKgBetween5And10, 0f, 2f);
            options.Label($"{"RAHI_Setting_MaxCTReductionPerMassKgAbove10".Translate()}: {settings.maxCTReductionPerMassKgAbove10.ToStringByStyle(style: ToStringStyle.FloatThree)}");
            settings.maxCTReductionPerMassKgAbove10 = options.Slider(settings.maxCTReductionPerMassKgAbove10, 0f, 5f);
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
