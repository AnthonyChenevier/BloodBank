// CompUsableByRace.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/03 4:52 PM


using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BloodBank
{

    public class CompProperties_UsableWithSkill : CompProperties_UseEffect
    {
        public List<SkillRequirement> skillRequirements;
        public CompProperties_UsableWithSkill() { compClass = typeof(CompUsableWithSkill); }
    }
    public class CompUsableWithSkill : CompUseEffect
    {
        public CompProperties_UsableWithSkill Props => (CompProperties_UsableWithSkill)this.props;

        public override float OrderPriority => 1000;
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            foreach (SkillRequirement requirement in Props.skillRequirements)
            {
                SkillDef skillDef = requirement.skill;
                int minLevel = requirement.minLevel;

                if (p.skills.GetSkill(skillDef).TotallyDisabled)
                {
                    failReason = "SkillDisabled".Translate();
                    return false;
                }

                if (p.skills.GetSkill(skillDef).Level >= minLevel)
                    continue;

                failReason = "UnderRequiredSkill".Translate(requirement.Summary);
                return false;
            }
            
            return base.CanBeUsedBy(p, out failReason);
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
        }
    }
}
