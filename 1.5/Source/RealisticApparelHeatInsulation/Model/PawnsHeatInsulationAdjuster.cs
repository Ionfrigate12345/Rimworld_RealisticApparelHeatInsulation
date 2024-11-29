using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

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
                if (pawn.Map != null)
                {
                    //Pawns on player map: Get map tile
                    biome = pawn.Map.Biome;
                }
                else if (pawn.GetCaravan() is Caravan caravan)
                {
                    //Pawns on world map: Get caravan biome
                    biome = pawn.GetCaravan().Biome;
                }

                //Desert, ExtremeDesert and AridShrubland can benefit from major heat insulation bonus of:
                //Apparel_Duster, Apparel_CowboyHat, Apparel_Shadecone, Apparel_HatHood (Heat Insulation Clothings)
                if (biome == BiomeDefOf.Desert
                    || biome == RAHIDefOf.ExtremeDesert
                    || biome == RAHIDefOf.AridShrubland)
                {

                }

                //Temperate Forest can benefit from minor heat insulation bonus of Heat Insulation Clothings
                if (biome == BiomeDefOf.TemperateForest)
                {

                }

                //Humidity penalty 1: Tropical Rainforest has moderate heat insulation penalty. 

                //Humidity penalty 2: Temperate Swamp and Tripical Swamp have major heat insulation penalty

                //Humidity penalty 3: Rainy and foggy weathers on local maps can cause temporary heat insulation penalty

                //Calculate apparel heat insulation modifier and apply to pawn hediff

                //1. Instead of increasing max comfortable temperature(MaxCT) like vanilla, it will mostly reduce it instead.
                //   Each layer of each body part (except head) will reduce max confortable temperature by x, where x is 20% of its original MaxCT bonus value.
                //2. Some apparels are exempted from this reduction: Apparel_BasicShirt, Apparel_Corset, Apparel_Tribal, Apparel_KidTribal
                //3. Some apparels uses its MinCT (cold insulation bonus) as MaxCT reduction: Apparel_Parka, Apparel_KidParka
                //4. For Heat Insulation Clothings like duster, the bonus is applied only when there aren't humidity penalties, otherwise they will have penalty like others above.
                //5. For armor type clothing (those with fixed value of Insulation_Heat), the reduction value is fixed and loaded on game startup.
                //6. If the sum of all apparels reach a certain percentage of total carry weight, MaxCT will reduce even more
                //   -20 when >=80%, -10 when >=50%, -5 when >=30%, -2 when >=10%  
                //7. Human pawns cannot have MaxCT < 21
            }
        }
    }
}
