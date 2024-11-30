﻿using System;
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

            var playerPawns = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_OfPlayerFaction_NoCryptosleep
                .Where(p => p.RaceProps.Humanlike).ToList();

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

                /** Humidity penalties (stackable) on pawn: 
                    * In wet biomes and weathers, pawns will suffer more maxHC penalty for each clothing piece worn.
                    * Will apply when temperature is above 30 and one of the following condition applies:*/
                CalculateMaxCTPenaltyHumidity(pawn, biome, 
                    out float humidityPenaltyPerApparelBiome, 
                    out float humidityPenaltyPerApparelWeather, 
                    out float humidityPenaltyPerApparelTotal
                    );

                /** For all non-HIA:
                - Instead of increasing max comfortable temperature(MaxCT) by most clothings, it will mostly reduce it instead.
                    The reduction value x is:
                        Max(MaxCT, MinCT) * 0.2
                    Where MacCT is the vanilla heat insulation MaxCT bonus, MinCT is the vanilla cold insulation bonus.
                - Apparels have 2x extra MaxCT reduction: Apparel_Parka, Apparel_KidParka
                - For heavier apparels between 5-10kg, each extra kg above 5 will reduce MaxCT by 0.2.
                - For heavier apparels more than 10kg, each extra kg above 10 will reduce MaxCT by 0.5.
                - Some basic apparels have MaxCT reduction permanently 0: Apparel_BasicShirt, Apparel_Pants, Apparel_Corset, Apparel_Tribal, Apparel_KidTribal.
                    However basic apparels may still bring humidity penalties on wet biomes or weathers.
                - Phoenix Armor permanently keeps its original heat insulation bonus regardless of calculations above.
                - Same for all clothings with EVA decompression resistance from SOS2. 
                    The apparel doesn't have to come from SOS2 itself, this rule is applied to all modded apparels with EVA compatibility depending on your modlist..
                    From real world thermodynamics, EVA apparels can isolate heat transfer from all 3 ways (Conduction, Convection, Radiation), making it almost perfectly heat insulated until extremely high temperature breaks its material.
                    Also, SOS2 doesn't have to be installed to get this feature applied. RAHI will simply check if "SaveOurShip2.CompEVA" compClass exists in ThingDef XML.
                    Example of modded apparels that keep their original MaxCT bonus (list not exhaustive): 
                        Eccentric Tech - Angel Apparel Ancient Mech Armor (only those with EVA compatibility)
                        Ancient Mech Armors
                */
                maxCTPenalties = CalculateMaxCTPenaltyNonHIA(apparelsHI, maxCTPenalties, humidityPenaltyPerApparelTotal);

                /** For all HIA:
                        The HI bonus is applied when there aren't humidity penalties.
                        */
                maxCTPenalties = CalculateMaxCTPenaltyHIA(apparelsHI, maxCTPenalties, biome, humidityPenaltyPerApparelTotal);

                //Get race base MaxCT value.
                float maxCTRace = pawn.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax);

                /**
                - If the sum of the mass of all apparels (including non eligible ones) reach a certain percentage of total carry weight, MaxCT will reduce
                     -20 when >= 80 %,  -15 when >= 70 %, -10 when >= 60 %, -5 when >= 40 %, -2 when >= 20 %
                - "Robust" gene from Biotech can reduce this penalty by -25%
                */
                float maxCTPenaltiesTotalApparelsMassKg = CalculateMaxCTPenaltyTotalApparelsWeight(pawn);

                /**
                - Heat tolerance/super-tolerance gene from Biotech can get +5/+10 extra base maxCT besides vanilla bonus
                - Heat tolerance/super-tolerance gene from Biotech can reduce all MaxCT reductions from this mod by 25%/50%
                */
                CalculateMaxCTPenaltyGenesReduction(pawn, 
                    out float maxCTPenaltiesGenesReductionValue, 
                    out float maxCTPenaltiesGenesReductionPercentage
                    );

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
                float maxCTBonusFromExposedBodyPart = CalculateMaxCTPenaltyExposedBodyPartReduction(pawn);

                /**
                TODO In later versions
                If temperature > 40 and a pawn is outdoor, exposed body part under luminosity > 51% may randonly get that body part wounded by burnt.
                Fire tolerance gene can reduce this chance by 90%
                */

                /**
                 Human pawns cannot have MaxCT < 21
                */
                float maxCTPenaltiesApparelsTotal = CalculateMaxCTPenaltiesApparelTotal(maxCTPenalties);
                float maxCTPenaltiesTotal = (maxCTPenaltiesApparelsTotal + maxCTPenaltiesTotalApparelsMassKg) * (1 - maxCTPenaltiesGenesReductionPercentage);
                float finalMaxCT = maxCTRace - maxCTPenaltiesTotal + maxCTPenaltiesGenesReductionValue + maxCTBonusFromExposedBodyPart;

                /**Apply new maxCT to pawn hediff */

            }
        }

        private void CalculateMaxCTPenaltyHumidity(Pawn pawn, BiomeDef biome,
            out float humidityPenaltyPerApparelBiome,
            out float humidityPenaltyPerApparelWeather,
            out float humidityPenaltyPerApparelTotal
            )
        {
            humidityPenaltyPerApparelBiome = 0;
            humidityPenaltyPerApparelWeather = 0;
            //Humidity penalty 1: Some biomes have fixed humidity penalty
            if (biome == RAHIDefOf.TropicalRainforest)
            {
                humidityPenaltyPerApparelBiome = RAHIModWindow.Instance.settings.humidityPenaltyPerApparelBiomeTropicalRainforest;
            }

            if (biome == RAHIDefOf.TemperateSwamp)
            {
                humidityPenaltyPerApparelBiome = RAHIModWindow.Instance.settings.humidityPenaltyPerApparelBiomeTemperateSwamp;
            }

            if (biome == RAHIDefOf.TropicalSwamp)
            {
                humidityPenaltyPerApparelBiome = RAHIModWindow.Instance.settings.humidityPenaltyPerApparelBiomeTropicalSwamp;
            }

            //Humidity penalty 2: Rainy or foggy weathers can cause temporary heat insulation penalty. MaxCT -1 for each piece of NonHI clothing.
            if (pawn.Map != null
                && (
                    pawn.Map.weatherManager.curWeather == RAHIDefOf.Rain
                    || pawn.Map.weatherManager.curWeather == RAHIDefOf.RainyThunderstorm
                    || pawn.Map.weatherManager.curWeather == RAHIDefOf.Fog
                    || pawn.Map.weatherManager.curWeather == RAHIDefOf.FoggyRain
                )
            )
            {
                humidityPenaltyPerApparelWeather = RAHIModWindow.Instance.settings.humidityPenaltyPerApparelBiomeWetWeather;
            }
            humidityPenaltyPerApparelTotal = humidityPenaltyPerApparelBiome + humidityPenaltyPerApparelWeather;
        }

        private List<MaxCTPenalty> CalculateMaxCTPenaltyNonHIA(List<Apparel> apparelsNonHI,
            List<MaxCTPenalty> maxCTPenalties,
            float humidityPenaltyPerApparelTotal
            )
        {
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

                maxCTPenalties.Add(new MaxCTPenalty(apparel, maxCTReduction + humidityPenaltyPerApparelTotal));
            }
            return maxCTPenalties;
        }

        private List<MaxCTPenalty> CalculateMaxCTPenaltyHIA(List<Apparel> apparelsHI,
            List<MaxCTPenalty> maxCTPenalties,
            BiomeDef biome, 
            float humidityPenaltyPerApparelTotal
            )
        {
            foreach (var apparel in apparelsHI)
            {
                float maxCTReduction = 0;
                float defaultMaxCTBonus = UtilsApparel.GetApparelDefaultMaxComfortableTemperatureBonus(apparel);

                //Any of these humidity penalty will void HIA MaxCT bonus, instead will reduce MaxCT by 10% of its vanilla MaxCT.
                //Humidity penalty wont apply on heat insulation type apparels.
                if (humidityPenaltyPerApparelTotal > 0)
                {
                    maxCTReduction = defaultMaxCTBonus //For voiding default maxCT bonus before applying reduction
                        + defaultMaxCTBonus * RAHIModWindow.Instance.settings.maxCTReductionPerVanillaBonusHIA;

                    maxCTPenalties.Add(new MaxCTPenalty(apparel, maxCTReduction));
                    continue;
                }

                //Desert, ExtremeDesert and AridShrubland can benefit from increased heat insulation compared with vanilla
                if (biome == BiomeDefOf.Desert
                        || biome == RAHIDefOf.ExtremeDesert
                        || biome == RAHIDefOf.AridShrubland)
                {
                    maxCTPenalties.Add(new MaxCTPenalty(apparel,
                        -defaultMaxCTBonus * 0.25f
                        ));
                }
                //Temperate Forest can benefit from reduced heat insulation bonus compared with vanilla
                else if (biome == BiomeDefOf.TemperateForest)
                {
                    maxCTPenalties.Add(new MaxCTPenalty(apparel,
                        defaultMaxCTBonus * 0.5f
                        ));
                }
                //For all other non-humidity biomes (cold biomes), HIA wont get heat insulation nor penalty
                else
                {
                    maxCTPenalties.Add(new MaxCTPenalty(apparel, defaultMaxCTBonus));
                }
            }
            return maxCTPenalties;
        }

        private float CalculateMaxCTPenaltyTotalApparelsWeight(Pawn pawn)
        {
            var allApparels = pawn.apparel.WornApparel;
            float totalApparelsMassKg = 0;
            float pawnCarryingCapacity = UtilsPawn.GetPawnCarryingCapacity(pawn);
            float maxCTPenaltiesTotalApparelsMassKg = 0;
            foreach (var apparel in allApparels)
            {
                totalApparelsMassKg += UtilsApparel.GetApparelMassKg(apparel);
            }
            if (totalApparelsMassKg / pawnCarryingCapacity > 0.8f)
            {
                maxCTPenaltiesTotalApparelsMassKg = 20;
            }
            else if (totalApparelsMassKg / pawnCarryingCapacity > 0.7f)
            {
                maxCTPenaltiesTotalApparelsMassKg = 15;
            }
            else if (totalApparelsMassKg / pawnCarryingCapacity > 0.6f)
            {
                maxCTPenaltiesTotalApparelsMassKg = 10;
            }
            else if (totalApparelsMassKg / pawnCarryingCapacity > 0.4f)
            {
                maxCTPenaltiesTotalApparelsMassKg = 5;
            }
            else if (totalApparelsMassKg / pawnCarryingCapacity > 0.2f)
            {
                maxCTPenaltiesTotalApparelsMassKg = 2;
            }
            return maxCTPenaltiesTotalApparelsMassKg;
        }

        private void CalculateMaxCTPenaltyGenesReduction(Pawn pawn,
                out float maxCTPenaltiesGenesReductionValue,
                out float maxCTPenaltiesGenesReductionPercentage
            )
        {
            if(UtilsPawn.HasGeneHeatToleranceLarge(pawn))
            {
                maxCTPenaltiesGenesReductionValue = RAHIModWindow.Instance.settings.maxCTBonusGeneHeatToleranceLargeValue;
                maxCTPenaltiesGenesReductionPercentage = RAHIModWindow.Instance.settings.maxCTBonusGeneHeatToleranceLargePercentage;
            }
            if (UtilsPawn.HasGeneHeatToleranceSmall(pawn))
            {
                maxCTPenaltiesGenesReductionValue = RAHIModWindow.Instance.settings.maxCTBonusGeneHeatToleranceSmallValue;
                maxCTPenaltiesGenesReductionPercentage = RAHIModWindow.Instance.settings.maxCTBonusGeneHeatToleranceSmallPercentage;
            }
            maxCTPenaltiesGenesReductionValue = 0;
            maxCTPenaltiesGenesReductionPercentage = 0;
        }

        private float CalculateMaxCTPenaltyExposedBodyPartReduction(Pawn pawn)
        {
            var allApparels = pawn.apparel.WornApparel;
            bool coveredShoulders = false;
            bool coveredArms = false;
            bool coveredLegs = false;
            bool coveredTorso = false;
            bool coveredNeck = false;
            foreach (var apparel in allApparels)
            {
                var bpGroups = apparel.def.apparel.bodyPartGroups;
                coveredShoulders = (coveredShoulders ? coveredShoulders : bpGroups.Any(bpg => bpg.defName.ToLower() == "shoulders"));
                coveredArms = (coveredArms ? coveredArms : bpGroups.Any(bpg => bpg.defName.ToLower() == "arms"));
                coveredLegs = (coveredLegs ? coveredLegs : bpGroups.Any(bpg => bpg.defName.ToLower() == "legs"));
                coveredTorso = (coveredTorso ? coveredTorso : bpGroups.Any(bpg => bpg.defName.ToLower() == "torso"));
                coveredNeck = (coveredNeck ? coveredNeck : bpGroups.Any(bpg => bpg.defName.ToLower() == "neck"));
            }
            float bonus = (!coveredShoulders ? RAHIModWindow.Instance.settings.maxCTBonusExposedShoulders : 0)
                + (!coveredArms ? RAHIModWindow.Instance.settings.maxCTBonusExposedArms : 0)
                + (!coveredLegs ? RAHIModWindow.Instance.settings.maxCTBonusExposedLegs : 0)
                + (!coveredTorso ? RAHIModWindow.Instance.settings.maxCTBonusExposedTorso : 0)
                + (!coveredNeck ? RAHIModWindow.Instance.settings.maxCTBonusExposedNeck : 0)
                ;
            return Math.Min(bonus, RAHIModWindow.Instance.settings.maxCTBonusExposedMaxTotal);
        }

        private float CalculateMaxCTPenaltiesApparelTotal(List<MaxCTPenalty> maxCTPenalties)
        {
            float sum = 0;
            foreach(var maxCTPenalty in maxCTPenalties)
            {
                sum += maxCTPenalty.MaxCTReduction;
            }
            return sum;
        }
    }
}