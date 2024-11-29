using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using SaveOurShip2;
using VEE.RegularEvents;
using Verse;

namespace RealisticApparelHeatInsulation
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        private static Harmony _harmonyInstance;
        public static Harmony HarmonyInstance { get { return _harmonyInstance; } }

        static HarmonyPatches()
        {
            _harmonyInstance = new Harmony("com.ionfrigate12345.realisticapparelheatinsulation");
            _harmonyInstance.PatchAll();
        }
    }
}