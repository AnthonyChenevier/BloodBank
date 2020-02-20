using RimWorld;
using Verse;

namespace BloodBank
{
    public class CompUseEffect_GiveBlood : CompUseEffect
    {
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {

            BloodPackComp bloodPackComp = parent.GetComp<BloodPackComp>();
            if (bloodPackComp == null)
            {
                Log.Error("Blood Bank - Give blood use effect failed (parent has no BloodPackComp)");
                failReason = "ERROR: Not a blood pack";
                return false;
            }
            if (p.skills.GetSkill(SkillDefOf.Medicine).TotallyDisabled)
            {
                failReason = "cannot do medical";
                return false;
            }
            
            if (p.skills.GetSkill(SkillDefOf.Medicine).Level < 4)
            {
                failReason = "low skill (requires Medical 4)";
                return false;
            }

            if (!p.health.hediffSet.HasHediff(HediffDefOf.BloodLoss) ||
                p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss).Severity <= bloodPackComp.Props.minSeverityForGive)
            {
                failReason = p.Name.ToStringShort + " does not need blood";
                return false;
            }

            failReason = null;
            return true;
        }

        public override void DoEffect(Pawn pawn)
        {
            if (parent.def.HasComp(typeof(BloodPackComp)))
            {
                BloodPackUtilities.ApplyBloodPack(pawn, parent.def);
                base.DoEffect(pawn);
            }
            else
            {
                Log.Error("Blood Bank - Give blood use effect failed (parent has no BloodPackComp)");
            }

        }
    }
}
