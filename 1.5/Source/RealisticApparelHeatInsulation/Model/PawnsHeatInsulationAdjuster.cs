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

                /** 
                - Instead of increasing max comfortable temperature(MaxCT) by most clothings, it will mostly reduce it instead.
                    The reduction value x is:
                        Max(MaxCT, MinCT) * 0.2
                    Where MacCT is the vanilla heat insulation MaxCT bonus, MinCT is the vanilla cold insulation bonus.
                - Apparels have 2x extra MaxCT reduction: Apparel_Parka, Apparel_KidParka
                - For heavier apparels between 5-10kg, each extra kg above 5 will reduce MaxCT by 0.2.
                - For heavier apparels more than 10kg, each extra kg above 10 will reduce MaxCT by 0.5.
                - For Heat Insulation Clothings like duster, the bonus is applied only when there aren't humidity penalties.
                    In case of humidity penalties, Heat Insulation Clothings will reduce MaxCT by 10% of its vanilla MaxCT increasing value.
                - Some basic apparels have MaxCT reduction permanently 0: Apparel_BasicShirt, Apparel_Pants, Apparel_Corset, Apparel_Tribal, Apparel_KidTribal
                - Phoenix Armor permanently keeps its original heat insulation bonus regardless of calculations above.
                - Same for all clothings with EVA decompression resistance from SOS2. 
                    The apparel doesn't have to come from SOS2 itself, this rule is applied to all modded apparels with EVA compatibility depending on your modlist..
                    This is for immersion because an apparel that can survive the space vacuum is obviously much more heat resilient. 
                    Also, SOS2 doesn't have to be installed to get this feature applied. RAHI will simply check if "SaveOurShip2.CompEVA" compClass exists in ThingDef XML.
                    Example of modded apparels that keep their original MaxCT bonus (list not exhaustive): 
                        Eccentric Tech - Angel Apparel Ancient Mech Armor (only those with EVA compatibility)
                        Ancient Mech Armors
                - If the sum of all apparels reach a certain percentage of total carry weight, MaxCT will reduce even more
                    -20 when >=80%, -10 when >=50%, -5 when >=30%, -2 when >=10%  
                - Heat resistance gene can half the MaxCT reduction from this mod.
                - Human pawns cannot have MaxCT < 21
                */
            }
        }
    }
}
