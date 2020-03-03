using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BloodBank
{

    public class CompProperties_UseEffectAddHediff : CompProperties_UseEffect
    {
        public HediffDef hediffDef;
        public float severity;

        public CompProperties_UseEffectAddHediff()
        {
            compClass = typeof(CompUseEffect_AddHediff);
        }
    }
    public class CompUseEffect_AddHediff : CompUseEffect
    {
        private CompProperties_UseEffectAddHediff Props => (CompProperties_UseEffectAddHediff)props;
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (p.health.hediffSet.HasHediff(Props.hediffDef))
            {
                failReason = $"{Props.hediffDef.label} effect already active";
                return false;
            }

            return base.CanBeUsedBy(p, out failReason);
        }
        public override void DoEffect(Pawn usedBy)
        {
            Hediff hediff = usedBy.health.AddHediff(Props.hediffDef);
            hediff.Severity = Props.severity;

            base.DoEffect(usedBy);
        }
    }
}
