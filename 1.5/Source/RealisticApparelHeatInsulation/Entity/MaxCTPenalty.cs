using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;

namespace RAHI.Entity
{
    //The class for each record of max comfortable temperature (MaxCT) reduction. 
    //If Apparel is null, then the MaxCTReduction will be directly applied as overall modifier to the pawn.
    internal class MaxCTPenalty
    {
        public Apparel Apparel { get; set; }
        public float MaxCTDefault { get; set; }
        public float MaxCTReduction { get; set; }
        public MaxCTPenalty(Apparel apparel, float maxCTDefault, float maxCTReduction) 
        {
            this.Apparel = apparel;
            this.MaxCTDefault = maxCTDefault;
            this.MaxCTReduction = maxCTReduction;
        }
    }
}
