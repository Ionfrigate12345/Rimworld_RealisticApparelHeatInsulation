using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RealisticApparelHeatInsulation.Global;
using RimWorld;
using RimWorld.Planet;
using RAHI.Entity;
using RAHI.View;
using UnityEngine;
using Verse;
using static UnityEngine.Scripting.GarbageCollector;
using Verse.AI;
using RAHI.Def;
using RAHI.Def.Hediff;

namespace RAHI.Model
{
    public class PawnsHeatInsulationAdjuster : WorldComponent
    {
        public PawnsHeatInsulationAdjuster(World world) : base(world)
        {
        }

        public override void WorldComponentTick()
        {
            var tickCount = Find.TickManager.TicksGame;
            var checkInteval = GenDate.TicksPerHour / 4;
            if (tickCount % checkInteval != 123)
                return;

            var efficiencyBonusExposedShoulders = RAHIModWindow.Instance.settings.efficiencyBonusExposedShoulders;
            var efficiencyBonusExposedArms = RAHIModWindow.Instance.settings.efficiencyBonusExposedArms;
            var efficiencyBonusExposedLegs = RAHIModWindow.Instance.settings.efficiencyBonusExposedLegs;

            var playerPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction_NoCryptosleep
                .Where(p => p.RaceProps.Humanlike && p.Faction == Faction.OfPlayer);

            var buf = new StringBuilder(1024);

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
                    biome = caravan.Biome;
                    temperature = Find.WorldGrid[caravan.Tile].temperature;
                }
                else
                {
                    continue;
                }

                float maxCTBonusFromExposedBodyPart = float.MaxValue; //Uninitialized value
                string exposedBodyPartDesc = String.Empty;
                bool coveredShoulders;
                bool coveredArms;
                bool coveredTorso;
                bool coveredLegs;
                bool coveredNeck;
                bool wearingShort;
                bool wearingSkirt;

                //If any of them > 0, calculate exposed body parts efficiency bonus regardless of temperature.
                if (efficiencyBonusExposedShoulders > 0 || efficiencyBonusExposedArms > 0 || efficiencyBonusExposedLegs > 0) 
                {
                    maxCTBonusFromExposedBodyPart = CalculateMaxCTPenaltyExposedBodyPartReduction(pawn,
                        out exposedBodyPartDesc,
                        out coveredShoulders,
                        out coveredArms,
                        out coveredTorso,
                        out coveredLegs,
                        out coveredNeck,
                        out wearingShort,
                        out wearingSkirt
                    );
                    UpdateExposedBodyPartEfficiencyBoost(pawn, BodyPartDefOf.Shoulder, coveredShoulders ? 0 : efficiencyBonusExposedShoulders);
                    UpdateExposedBodyPartEfficiencyBoost(pawn, BodyPartDefOf.Arm, coveredArms ? 0 : efficiencyBonusExposedArms);
                    UpdateExposedBodyPartEfficiencyBoost(pawn, BodyPartDefOf.Leg, 
                        (coveredLegs && !wearingShort && !wearingSkirt) ? 0 : efficiencyBonusExposedLegs);
                }

                if (temperature <= 30)
                {
                    //No script needed if ambient temperature isn't high enough.
                    //Pawn RAHI hediff removed
                    var pawnHediffAdjustedMaxCT = pawn.health.hediffSet.hediffs.Where(x =>
                        x.def.defName == RAHIDefOf.RAHI_AdjustedMaxCT.defName
                    ).FirstOrDefault();
                    if(pawnHediffAdjustedMaxCT != null)
                    {
                        pawn.health.RemoveHediff(pawnHediffAdjustedMaxCT);
                    }
                    continue;
                }

                //Apparel_Duster, Apparel_CowboyHat, Apparel_Shadecone, Apparel_HatHood are considerred as Heat Insulation apparels (HIA)
                var apparelsHI = UtilsApparel.GetAllHeatInsulationClothingsOnPawn(pawn);
                var apparelsNonHI = UtilsApparel.GetAllEligibleNonHeatInsulationClothingsOnPawn(pawn);

                /** Humidity penalties (stackable) on pawn: 
                    * In wet biomes and weathers, pawns will suffer more maxHC penalty for each clothing piece worn.
                    * Will apply when temperature is above 30 and one of the following condition applies:*/
                CalculateMaxCTPenaltyHumidity(pawn, biome, 
                    out float maxCTHumidityPenaltyPerApparelBiome, 
                    out float maxCTHumidityPenaltyPerApparelWeather, 
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

                var maxCTPenaltiesNonHIA = CalculateMaxCTPenaltyNonHIA(apparelsNonHI, humidityPenaltyPerApparelTotal);
                var maxCTPenaltiesHIA = CalculateMaxCTPenaltyHIA(apparelsHI, biome, humidityPenaltyPerApparelTotal);
                var maxCTPenalties = maxCTPenaltiesNonHIA.Concat(maxCTPenaltiesHIA);

                //Get race base MaxCT value.
                float maxCTRace = pawn.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax);

                /**
                - If the sum of the mass of all apparels (including non eligible ones) reach a certain percentage of total carry weight, MaxCT will reduce
                     -20 when >= 80 %,  -15 when >= 70 %, -10 when >= 60 %, -5 when >= 40 %, -2 when >= 20 %
                - TODO: "Robust" gene from Biotech can reduce this penalty by -25%
                */
                float maxCTPenaltiesTotalApparelsMassKg = CalculateMaxCTPenaltyTotalApparelsWeight(pawn);

                /**
                - Heat tolerance/super-tolerance gene from Biotech can get +5/+10 extra base maxCT besides vanilla bonus
                - Heat tolerance/super-tolerance gene from Biotech can reduce all MaxCT reductions from this mod by 25%/50%
                */
                CalculateMaxCTPenaltyGenesReduction(pawn, 
                    out float maxCTBonusFromGenesValue, 
                    out float maxCTBonusFromGenePercentage
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
                if(maxCTBonusFromExposedBodyPart == float.MaxValue)
                { 
                    //If hasn't previously initialized (To avoid running the function again for better optimization)
                    maxCTBonusFromExposedBodyPart = CalculateMaxCTPenaltyExposedBodyPartReduction(pawn,
                        out exposedBodyPartDesc,
                        out coveredShoulders,
                        out coveredArms,
                        out coveredTorso,
                        out coveredLegs,
                        out coveredNeck,
                        out wearingShort,
                        out wearingSkirt
                    );
                }

                /**
                TODO In later versions
                If temperature > 40 and a pawn is outdoor, exposed body part under luminosity > 51% may randonly get that body part wounded by burnt.
                Fire tolerance gene can reduce this chance by 90%
                */

                /**
                 Calculate final maxCT. Human pawns cannot have MaxCT < 21
                */
                float maxCTPenaltiesApparelsTotal = CalculateMaxCTPenaltiesApparelTotal(maxCTPenalties);
                float maxCTPenaltiesTotal = (maxCTPenaltiesApparelsTotal + maxCTPenaltiesTotalApparelsMassKg) * (1 - maxCTBonusFromGenePercentage);

                /**Apply new maxCT to pawn hediff */
                var hediffAdjustedMaxCT = pawn.health.hediffSet.hediffs.Where(x =>
                    x.def.defName == RAHIDefOf.RAHI_AdjustedMaxCT.defName
                ).FirstOrDefault();
                if(hediffAdjustedMaxCT == null)
                {
                    hediffAdjustedMaxCT = HediffMaker.MakeHediff(RAHIDefOf.RAHI_AdjustedMaxCT, pawn);
                    pawn.health.AddHediff(hediffAdjustedMaxCT);
                }

                hediffAdjustedMaxCT.Severity = 0;
                float vanillaMaxCTRaceActual = pawn.GetStatValue(StatDefOf.ComfyTemperatureMax);
                float finalMaxCT = vanillaMaxCTRaceActual - maxCTPenaltiesTotal + maxCTBonusFromGenesValue + maxCTBonusFromExposedBodyPart;
                finalMaxCT = Math.Max(finalMaxCT, 21.0f);
                int finalMaxCTModifier = (int)Math.Round(finalMaxCT - vanillaMaxCTRaceActual);
                //Use minSeverity tag as stage selector
                //Positive modifiers use severity 0.xx, while negative 1.xx
                //The modifier is between -30C and 30C
                finalMaxCTModifier = Math.Max(finalMaxCTModifier, -30);
                finalMaxCTModifier = Math.Min(finalMaxCTModifier, 30);
                hediffAdjustedMaxCT.Severity = Math.Abs(finalMaxCTModifier) * 0.01f + (finalMaxCTModifier < 0 ? 1.0f : 0);

                //Dynamic description
                buf.Clear();
                foreach (var maxCTPenalty in maxCTPenalties)
                {
                    if (maxCTPenalty.Apparel != null)
                    {
                        buf.Append("\n");
                        buf.Append(maxCTPenalty.Apparel.Label);
                        buf.Append(" : ");
                        buf.Append(maxCTPenalty.MaxCTDefault.ToString("0.0"));
                        buf.Append(" / ");
                        buf.Append((maxCTPenalty.MaxCTDefault - maxCTPenalty.MaxCTReduction).ToString("0.0"));
                    }
                }
                string descDetailApparels = buf.ToString();

                buf.Clear();
                buf.Append(
                    new TaggedString("RAHI_Hediff_Description".Translate(
                        (int)Math.Round(vanillaMaxCTRaceActual), 
                        (int)Math.Round(finalMaxCT),
                        maxCTRace,
                        (maxCTBonusFromGenePercentage * 100),
                        maxCTBonusFromGenesValue,
                        maxCTBonusFromExposedBodyPart,
                        maxCTHumidityPenaltyPerApparelBiome,
                        maxCTHumidityPenaltyPerApparelWeather,
                        maxCTPenaltiesTotalApparelsMassKg,
                        descDetailApparels
                    ))
                );
                if (!string.IsNullOrEmpty(exposedBodyPartDesc))
                {
                    buf.Append("\n");
                    buf.Append(Strings.RAHI_Hediff_Description_Exposed_Bodyparts.Value);
                    buf.Append("\n");
                    buf.Append(exposedBodyPartDesc);
                }
                var comp = hediffAdjustedMaxCT.TryGetComp<HediffComp_DescriptionModifier>();
                comp.CustomDescription = buf.ToString();
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
            else if (biome == RAHIDefOf.TemperateSwamp)
            {
                humidityPenaltyPerApparelBiome = RAHIModWindow.Instance.settings.humidityPenaltyPerApparelBiomeTemperateSwamp;
            }
            else if (biome == RAHIDefOf.TropicalSwamp)
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

        private IEnumerable<MaxCTPenalty> CalculateMaxCTPenaltyNonHIA(IEnumerable<Apparel> apparelsNonHI,
            float humidityPenaltyPerApparelTotal
            )
        {
            foreach (var apparel in apparelsNonHI)
            {
                float defaultMaxCTBonus = UtilsApparel.GetApparelDefaultMaxComfortableTemperatureBonus(apparel);
                float maxCTReduction = defaultMaxCTBonus;

                if (!UtilsApparel.IsBaseClothingWithoutPenalty(apparel))
                {
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

                    //If VE Apparel footwear or handwear, half penalty
                    if(UtilsApparel.IsVEApparelFootwear(apparel) || UtilsApparel.IsVEApparelHandwear(apparel))
                    {
                        maxCTReduction = maxCTReduction / 2.0f;
                    }
                }

                yield return new MaxCTPenalty(apparel, defaultMaxCTBonus, maxCTReduction + humidityPenaltyPerApparelTotal);
            }
        }

        private IEnumerable<MaxCTPenalty> CalculateMaxCTPenaltyHIA(IEnumerable<Apparel> apparelsHI,
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

                    yield return new MaxCTPenalty(apparel, defaultMaxCTBonus, maxCTReduction);
                    continue;
                }

                //Desert, ExtremeDesert and AridShrubland can benefit from increased heat insulation compared with vanilla
                if (biome == BiomeDefOf.Desert
                        || biome == RAHIDefOf.ExtremeDesert
                        || biome == RAHIDefOf.AridShrubland)
                {
                    yield return new MaxCTPenalty(apparel, defaultMaxCTBonus, -defaultMaxCTBonus * 0.25f);
                }
                //Temperate Forest can benefit from reduced heat insulation bonus compared with vanilla
                else if (biome == BiomeDefOf.TemperateForest)
                {
                    yield return new MaxCTPenalty(apparel,defaultMaxCTBonus, defaultMaxCTBonus * 0.5f);
                }
                //For all other non-humidity biomes (cold biomes), HIA wont get heat insulation nor penalty
                else
                {
                    yield return new MaxCTPenalty(apparel, defaultMaxCTBonus, defaultMaxCTBonus);
                }
            }
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
                out float maxCTBonusFromGenesValue,
                out float maxCTBonusFromGenesPercentage
            )
        {
            if(UtilsPawn.HasGeneHeatToleranceLarge(pawn))
            {
                maxCTBonusFromGenesValue = RAHIModWindow.Instance.settings.maxCTBonusGeneHeatToleranceLargeValue;
                maxCTBonusFromGenesPercentage = RAHIModWindow.Instance.settings.maxCTBonusGeneHeatToleranceLargePercentage;
            }
            else if (UtilsPawn.HasGeneHeatToleranceSmall(pawn))
            {
                maxCTBonusFromGenesValue = RAHIModWindow.Instance.settings.maxCTBonusGeneHeatToleranceSmallValue;
                maxCTBonusFromGenesPercentage = RAHIModWindow.Instance.settings.maxCTBonusGeneHeatToleranceSmallPercentage;
            }
            else
            {
                maxCTBonusFromGenesValue = 0;
                maxCTBonusFromGenesPercentage = 0;
            }
        }

        private float CalculateMaxCTPenaltyExposedBodyPartReduction(
            Pawn pawn, 
            out string exposedBodyPartDesc,
            out bool coveredShoulders,
            out bool coveredArms,
            out bool coveredTorso,
            out bool coveredLegs,
            out bool coveredNeck,
            out bool wearingShort,
            out bool wearingSkirt
            )
        {
            List<string> buf = new(5);
            var allApparels = pawn.apparel.WornApparel;
            coveredShoulders = false;
            coveredArms = false;
            coveredLegs = false;
            coveredTorso = false;
            coveredNeck = false;
            wearingShort = false;
            wearingSkirt = false;
            foreach (var apparel in allApparels)
            {
                var bpGroups = apparel.def.apparel.bodyPartGroups;

                //Wearing shorts or skirts from VE Apparel don't count as leg covering.
                if (apparel.def.defName == "VAE_Apparel_Shorts") { wearingShort = true; continue; }
                else if (apparel.def.defName == "VAE_Apparel_Skirt") { wearingSkirt = true; continue; }
                else
                {
                    coveredLegs = (coveredLegs ? coveredLegs : bpGroups.Any(bpg => bpg.defName.ToLower() == "legs"));
                }

                coveredShoulders = (coveredShoulders ? coveredShoulders : bpGroups.Any(bpg => bpg.defName.ToLower() == "shoulders"));
                coveredArms = (coveredArms ? coveredArms : bpGroups.Any(bpg => bpg.defName.ToLower() == "arms"));
                coveredTorso = (coveredTorso ? coveredTorso : bpGroups.Any(bpg => bpg.defName.ToLower() == "torso"));
                coveredNeck = (coveredNeck ? coveredNeck : bpGroups.Any(bpg => bpg.defName.ToLower() == "neck"));
            }
            float bonus = (!coveredShoulders ? RAHIModWindow.Instance.settings.maxCTBonusExposedShoulders : 0)
                + (!coveredArms ? RAHIModWindow.Instance.settings.maxCTBonusExposedArms : 0)
                + (!coveredLegs ? RAHIModWindow.Instance.settings.maxCTBonusExposedLegs : 0)
                + (!coveredTorso ? RAHIModWindow.Instance.settings.maxCTBonusExposedTorso : 0)
                + (!coveredNeck ? RAHIModWindow.Instance.settings.maxCTBonusExposedNeck : 0)
                ;
            if (!coveredShoulders)
            {
                buf.Append(Strings.RAHI_Hediff_Description_Exposed_Bodyparts_Shoulders.Value);
            }
            if (!coveredArms)
            {
                buf.Append(Strings.RAHI_Hediff_Description_Exposed_Bodyparts_Arms.Value);
            }
            if (!coveredLegs)
            {
                if (wearingShort)
                {
                    buf.Append(Strings.RAHI_Hediff_Description_Exposed_Bodyparts_Legs_Short.Value);
                }
                else if (wearingSkirt)
                {
                    buf.Append(Strings.RAHI_Hediff_Description_Exposed_Bodyparts_Legs_Skirt.Value);
                }
                else
                {
                    buf.Append(Strings.RAHI_Hediff_Description_Exposed_Bodyparts_Legs.Value);
                }
            }
            if (!coveredTorso)
            {
                buf.Append(Strings.RAHI_Hediff_Description_Exposed_Bodyparts_Torso.Value);
            }
            if (!coveredNeck)
            {
                buf.Append(Strings.RAHI_Hediff_Description_Exposed_Bodyparts_Neck.Value);
            }
            exposedBodyPartDesc = string.Join("\n", buf);
            return Math.Min(bonus, RAHIModWindow.Instance.settings.maxCTBonusExposedMaxTotal);
        }

        private float CalculateMaxCTPenaltiesApparelTotal(IEnumerable<MaxCTPenalty> maxCTPenalties)
        {
            float sum = 0;
            foreach(var maxCTPenalty in maxCTPenalties)
            {
                sum += maxCTPenalty.MaxCTReduction;
            }
            return sum;
        }

        private void UpdateExposedBodyPartEfficiencyBoost(Pawn pawn, BodyPartDef bodyPartDef, float efficiencyBonus)
        {
            if (pawn == null || bodyPartDef == null)
                return;

            List<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts()
                .Where(bp => bp.def == bodyPartDef).ToList();

            if (bodyParts.NullOrEmpty())
            {
                Log.Error($"[RAHI] No valid body parts found for {bodyPartDef.defName} on {pawn.Name}!");
                return;
            }
            if (RAHIDefOf.RAHI_ExposedBodyPartsEfficiencyBoost == null)
            {
                Log.Error("[RAHI] RAHI_ExposedBodyPartsEfficiencyBoost HediffDef is null!");
                return;
            }
            foreach (var bodyPart in bodyParts)
            {
                var hediff = pawn.health.hediffSet.hediffs
                    .FirstOrDefault(x => x.def == RAHIDefOf.RAHI_ExposedBodyPartsEfficiencyBoost && x.Part == bodyPart);
                if (efficiencyBonus <= 0)
                {
                    if (hediff != null) pawn.health.RemoveHediff(hediff);
                }
                else
                {
                    if (hediff is null)
                    {
                        hediff = HediffMaker.MakeHediff(RAHIDefOf.RAHI_ExposedBodyPartsEfficiencyBoost, pawn, bodyPart);
                        if (hediff is null)
                        {
                            Log.Error("[RAHI] HediffMaker.MakeHediff returned null!");
                            continue;
                        }
                        pawn.health.AddHediff(hediff);
                    }
                    if (hediff.Severity != efficiencyBonus)
                    {
                        hediff.Severity = efficiencyBonus;
                    }
                }
            }
        }

        private static class Strings
        {
            internal static readonly Lazy<string> RAHI_Hediff_Description_Exposed_Bodyparts = new(delegate
            {
                return new TaggedString("RAHI_Hediff_Description_Exposed_Bodyparts".Translate());
            });

            internal static readonly Lazy<string> RAHI_Hediff_Description_Exposed_Bodyparts_Shoulders = new(delegate
            {
                return new TaggedString("RAHI_Hediff_Description_Exposed_Bodyparts_Shoulders".Translate());
            });

            internal static readonly Lazy<string> RAHI_Hediff_Description_Exposed_Bodyparts_Arms = new(delegate
            {        
                return new TaggedString("RAHI_Hediff_Description_Exposed_Bodyparts_Arms".Translate());
            });

            internal static readonly Lazy<string> RAHI_Hediff_Description_Exposed_Bodyparts_Legs_Short = new(delegate
            {
                return new TaggedString("RAHI_Hediff_Description_Exposed_Bodyparts_Legs_Short".Translate());
            });

            internal static readonly Lazy<string> RAHI_Hediff_Description_Exposed_Bodyparts_Legs_Skirt = new(delegate
            {
                return new TaggedString("RAHI_Hediff_Description_Exposed_Bodyparts_Legs_Skirt".Translate());
            });

            internal static readonly Lazy<string> RAHI_Hediff_Description_Exposed_Bodyparts_Legs = new(delegate
            {
                return new TaggedString("RAHI_Hediff_Description_Exposed_Bodyparts_Legs".Translate());
            });

            internal static readonly Lazy<string> RAHI_Hediff_Description_Exposed_Bodyparts_Torso = new(delegate
            {
                return new TaggedString("RAHI_Hediff_Description_Exposed_Bodyparts_Torso".Translate());
            });

            internal static readonly Lazy<string> RAHI_Hediff_Description_Exposed_Bodyparts_Neck = new(delegate
            {
                return new TaggedString("RAHI_Hediff_Description_Exposed_Bodyparts_Neck".Translate());
            });
        }

    }
}
