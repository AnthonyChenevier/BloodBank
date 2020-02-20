// HediffComp_HealFactor.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/02/10 4:40 PM


using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BloodBank {

    public class HediffCompProperties_HealFactor : HediffCompProperties
    {
        public float healFactor;
    }
    public class HediffComp_HealFactor : HediffComp
    {
        private List<Hediff_Injury> injuries;
        private Pawn pawn;
        public HediffCompProperties_HealFactor Props => (HediffCompProperties_HealFactor)this.props;

        public override void CompPostMake()
        {
            pawn = this.parent.pawn;
            
            base.CompPostMake();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            injuries = new List<Hediff_Injury>();
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs.Where(h => h is Hediff_Injury))
            {
                var injury = (Hediff_Injury)hediff;
                injury.Heal(Props.healFactor * pawn.HealthScale * 0.01f);
                pawn.health.Notify_HediffChanged(injury);
            }
            base.CompPostTick(ref severityAdjustment);
        }
    }
}
