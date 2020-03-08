// CompUsableByRace.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/03 5:50 PM


using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BloodBank {
    public class CompProperties_UsableByRace : CompProperties_UseEffect
    {
        public List<ThingDef> allowedRaces;
        public CompProperties_UsableByRace() { compClass = typeof(CompUsableByRace); }
    }
    public class CompUsableByRace : CompUseEffect
    {

        public CompProperties_UsableByRace Props => (CompProperties_UsableByRace)props;

        public override float OrderPriority => 1000;

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (Props.allowedRaces.Contains(p.def))
                return base.CanBeUsedBy(p, out failReason);

            failReason = "Can't be used by this race";
            return false;
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
        }
    }
}
