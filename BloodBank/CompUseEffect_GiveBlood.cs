using RimWorld;
using Verse;

namespace BloodBank
{
    public class CompProperties_Blood : CompProperties_UseEffect
    {
        public float bloodAmount;
        public float minSeverityForGive;
        public float harvestEfficiencyFactor;
        public float minSeverityForBadThought;

        public CompProperties_Blood() { compClass = typeof(CompUseEffect_GiveBlood); }
    }

    public class CompUseEffect_GiveBlood : CompUseEffect
    {
        private CompProperties_Blood Props => (CompProperties_Blood)props;

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (!p.health.hediffSet.HasHediff(HediffDefOf.BloodLoss) ||
                p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss).Severity <= Props.minSeverityForGive)
            {
                failReason = p.Name.ToStringShort + " does not need blood";
                return false;
            }

            return base.CanBeUsedBy(p, out failReason);
        }

        public override void DoEffect(Pawn pawn)
        {
            BloodBankUtilities.ApplyBloodPack(pawn, parent.def);
            base.DoEffect(pawn);
        }
    }
}
