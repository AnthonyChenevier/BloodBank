// CompUsableRestriction.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/04 3:20 PM


using RimWorld;
using Verse;

namespace BloodBank {
    
    public class CompUsableRestriction:CompUseEffect {
        public override float OrderPriority => 1000;
        public override void DoEffect(Pawn usedBy) { } //no effect, only restrict!
    }
}
