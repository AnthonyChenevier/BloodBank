// CompUseEffect_AdministerBloodProduct.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/04 7:41 PM


using RimWorld;
using Verse;

namespace BloodBank {
    public class CompUseEffect_AdministerBloodProduct : CompUseEffect
    {
        public override void DoEffect(Pawn pawn)
        {
            BloodBankUtilities.AdministerBooster(pawn, parent.GetComp<CompBloodProduct>());
            base.DoEffect(pawn);
        }
    }
}
