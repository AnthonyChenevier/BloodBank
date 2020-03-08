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

    public class CompProperties_RestrictUsableByRace : CompProperties_UseEffect
    {
        public List<ThingDef> allowedRaces;
        public CompProperties_RestrictUsableByRace() { compClass = typeof(CompRestrictUsableByRace); }
    }
    public class CompRestrictUsableByRace : CompUsableRestriction
    {
        public CompProperties_RestrictUsableByRace Props => (CompProperties_RestrictUsableByRace)props;
        
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (!Props.allowedRaces.Contains(p.def))
            {
                failReason = "RaceCantUse".Translate();
                return false;
            }

            return base.CanBeUsedBy(p, out failReason);
        }
    }
}
