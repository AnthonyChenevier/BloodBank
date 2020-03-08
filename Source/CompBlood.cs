// CompBlood.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/04 12:52 AM


using Verse;

namespace BloodBank
{
    public class CompProperties_Blood : CompProperties
    {
        public float bloodAmount;
        public float harvestEfficiencyFactor;
        public float minSeverityForBadThought;

        public CompProperties_Blood() { compClass = typeof(CompBlood); }
    }

    public class CompBlood:ThingComp
    {
        public CompProperties_Blood Props => (CompProperties_Blood)props;
    }
}
