using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using RealisticApparelHeatInsulation.Global;
using RimWorld;
using RimWorld.Planet;
using SOS2VEEPatch.Entity;
using SOS2VEEPatch.View;
using UnityEngine;
using Verse;
using static UnityEngine.Scripting.GarbageCollector;
using Verse.AI;

namespace SOS2VEEPatch.Model
{
    public class PawnsHeatInsulationAdjuster : WorldComponent
    {
        public PawnsHeatInsulationAdjuster(World world) : base(world)
        {
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();

            var tickCount = Find.TickManager.TicksGame;
            var checkInteval = GenDate.TicksPerHour / 2;
            if (tickCount % checkInteval != 321)
                return;

            var playerPawns = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_OfPlayerFaction_NoCryptosleep;

            foreach (var pawn in playerPawns)
            {
                //Check biome
                BiomeDef biome = null;
                float temperature = 0;
                if (pawn.Map != null)
                {
                    //Pawns on player map: Get map tile and ambient temperature
                    biome = pawn.Map.Biome;
                    temperature = pawn.AmbientTemperature;
                }
                else if (pawn.GetCaravan() is Caravan caravan)
                {
                    //Pawns on world map: Get caravan biome and temperature
                    biome = pawn.GetCaravan().Biome;
                    temperature = Find.WorldGrid[pawn.GetCaravan().Tile].temperature;
                }
                else
                {
                    continue;
                }

                if (temperature <= 30)
                {
                    //No script needed if ambient temperature isn't high enough.
                    //TODO: Pawn RAHI hediff removed
                    continue;
                }

                List<MaxCTPenalty> maxCTPenalties = new List<MaxCTPenalty>();

                //Apparel_Duster, Apparel_CowboyHat, Apparel_Shadecone, Apparel_HatHood are considerred as Heat Insulation apparels (HIA)
                List<Apparel> apparelsHI = UtilsApparel.GetAllHeatInsulationClothingsOnPawn(pawn);
                List<Apparel> apparelsNonHI = UtilsApparel.GetAllEligibleNonHeatInsulationClothingsOnPawn(pawn);

                /** For all non-HIA:
                - Instead of increasing max comfortable temperature(MaxCT) by most clothings, it will mostly reduce it instead.
                    The reduction value x is:
                        Max(MaxCT, MinCT) * 0.2
                    Where MacCT is the vanilla heat insulation MaxCT bonus, MinCT is the vanilla cold insulation bonus.
                - Apparels have 2x extra MaxCT reduction: Apparel_Parka, Apparel_KidParka
                - For heavier apparels between 5-10kg, each extra kg above 5 will reduce MaxCT by 0.2.
                - For heavier apparels more than 10kg, each extra kg above 10 will reduce MaxCT by 0.5.
                - Some basic apparels have MaxCT reduction permanently 0: Apparel_BasicShirt, Apparel_Pants, Apparel_Corset, Apparel_Tribal, Apparel_KidTribal
                - Phoenix Armor permanently keeps its original heat insulation bonus regardless of calculations above.
                - Same for all clothings with EVA decompression resistance from SOS2. 
                    The apparel doesn't have to come from SOS2 itself, this rule is applied to all modded apparels with EVA compatibility depending on your modlist..
                    From real world thermodynamics, EVA apparels can isolate heat transfer from all 3 ways (Conduction, Convection, Radiation), making it almost perfectly heat insulated until extremely high temperature breaks its material.
                    Also, SOS2 doesn't have to be installed to get this feature applied. RAHI will simply check if "SaveOurShip2.CompEVA" compClass exists in ThingDef XML.
                    Example of modded apparels that keep their original MaxCT bonus (list not exhaustive): 
                        Eccentric Tech - Angel Apparel Ancient Mech Armor (only those with EVA compatibility)
                        Ancient Mech Armors
                */

                foreach (var apparel in apparelsNonHI)
                {
                    float maxCTReduction = 0;

                    if (!UtilsApparel.IsBaseClothingWithoutPenalty(apparel))
                    {
                        float defaultMaxCTBonus = UtilsApparel.GetApparelDefaultMaxComfortableTemperatureBonus(apparel);
                        float defaultMinCTBonus = UtilsApparel.GetApparelDefaultMinComfortableTemperatureBonus(apparel);
                        float massKg = UtilsApparel.GetApparelMassKg(apparel);

                        maxCTReduction = defaultMaxCTBonus //For voiding default maxCT bonus before applying reduction
                            + (Math.Max(defaultMinCTBonus, defaultMaxCTBonus) * RAHIModWindow.Instance.settings.maxCTReductionPerVanillaBonus);

                        if (massKg > 5 && massKg <= 10)
                        {
                            maxCTReduction += (massKg - 5) * RAHIModWindow.Instance.settings.maxCTReductionPerMassKgBetween5And10;
                        }
                        else if (massKg > 10)
                        {
                            maxCTReduction += (massKg - 10) * RAHIModWindow.Instance.settings.maxCTReductionPerMassKgAbove10
                                + 5 * RAHIModWindow.Instance.settings.maxCTReductionPerMassKgBetween5And10;
                        }
                    }

                    maxCTPenalties.Add(new MaxCTPenalty(apparel, maxCTReduction));
                }

                /** Humidity penalties (stackable) on pawn: 
                    * In wet biomes and weathers, pawns will suffer more maxHC penalty for each clothing piece worn.
                    * Will apply when temperature is above 30 and one of the following condition applies:*/

                //Humidity penalty 1: Tropical Rainforest has moderate heat insulation penalty. MaxCT -1 for each piece of NonHI clothing.

                //Humidity penalty 2: Temperate Swamp and Tripical Swamp have major heat insulation penalty. MaxCT -1.5 for each piece of NonHI clothing.

                //Humidity penalty 3: Rainy and foggy weathers on local maps can cause temporary heat insulation penalty. MaxCT -1 for each piece of NonHI clothing.

                //Any of these 3 humidity penalty will void HIA MaxCT bonus, instead will reduce MaxCT by 10% of its vanilla MaxCT.


                /** For all HIA:
                    The HI bonus is applied when there aren't humidity penalties.
                    */

                //Desert, ExtremeDesert and AridShrubland can benefit from major heat insulation bonus of HIC
                //The bonus MaxCT value = vanilla value * 1.5
                foreach (var apparel in apparelsHI)
                {
                    if (biome == BiomeDefOf.Desert
                        || biome == RAHIDefOf.ExtremeDesert
                        || biome == RAHIDefOf.AridShrubland)
                    {

                    }

                    //Temperate Forest can benefit from minor heat insulation bonus of Heat Insulation Clothings
                    //The bonus MaxCT value = vanilla value * 0.5
                    if (biome == BiomeDefOf.TemperateForest)
                    {

                    }
                }

                //Get race base MaxCT value.
                //raceDef.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax)

                /**
                - If the sum of the mass of all apparels reach a certain percentage of total carry weight, MaxCT will reduce
                     -20 when >= 80 %,  -15 when >= 70 %, -10 when >= 60 %, -5 when >= 40 %, -2 when >= 20 %
                - "Robust" gene from Biotech can reduce this penalty by -25%
                */

                /**
                - Heat tolerance/super-tolerance gene from Biotech can get +5/+10 extra base maxCT besides vanilla bonus
                - Heat tolerance/super-tolerance gene from Biotech can reduce all MaxCT reductions from this mod by -25%/-50%
                */

                /** 
                MaxCT will increase for pawns with some body parts not covered by any apparel:
                -  +2 bonus for each non covered shoulder,
                -  +2 bonus for each non covered arm,
                -  +2 bonus for each non covered leg.
                -  +5 bonus for non covered torso.
                -  +5 bonus for non covered waist.
                -  +1 bonus for non covered neck.
                However the maximum MaxCT bonus for this part can't go beyond 15.
                */

                /**
                TODO: If temperature > 40 and a pawn is outdoor, exposed body part under luminosity > 51% may randonly get that body part wounded by burnt.
                */

                /**
                 Lastly: Human pawns cannot have MaxCT < 21
                */

                /**Apply to pawn hediff */
            }
        }
    }
}
