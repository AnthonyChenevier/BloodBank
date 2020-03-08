using System;
using RimWorld;
using Verse;

namespace BloodBank
{
    public class CompUseEffect_AdministerBloodTransfusion : CompUseEffect
    {
        public override void DoEffect(Pawn pawn)
        {
            BloodBankUtilities.AdministerTransfusion(pawn, parent.GetComp<CompBlood>());
            base.DoEffect(pawn);
        }
    }
}
