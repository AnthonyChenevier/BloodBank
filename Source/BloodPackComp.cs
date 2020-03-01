using System;
using Verse;

namespace BloodBank
{

    public class BloodPackComp : ThingComp
    {
        public CompProperties_BloodPack Props => (CompProperties_BloodPack)this.props;
    }

    public class CompProperties_BloodPack: CompProperties
    {
        public float bloodAmount;
        public float minSeverityForGive;
        public float harvestEfficiencyFactor;
        public float minSeverityForBadThought;

        public CompProperties_BloodPack() { this.compClass = typeof(BloodPackComp); }
    }
}
