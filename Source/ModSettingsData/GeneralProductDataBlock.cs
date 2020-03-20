// GeneralProductProperties.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/16 12:30 PM


using Verse;

namespace BloodBank.ModSettingsData
{
    public class GeneralProductDataBlock : ModSettingsDataBlock<GeneralProductDataBlock>
    {
        public int StackLimit;
        public float DaysToRot;

        public bool RequireMedicalSkill;
        public int SkillRequirement;

        public float HitPoints;
        public float DeteriorationRate;


        public override void SetDefault()
        {
            DaysToRot = 0.5f;
            RequireMedicalSkill = true;
            SkillRequirement = 4;
            StackLimit = 25;
            HitPoints = 60f;
            DeteriorationRate = 6;
        }

        public override void CopyFrom(GeneralProductDataBlock other)
        {
            DaysToRot = other.DaysToRot;
            RequireMedicalSkill = other.RequireMedicalSkill;
            SkillRequirement = other.SkillRequirement;
            StackLimit = other.StackLimit;
            HitPoints = other.HitPoints;
            DeteriorationRate = other.DeteriorationRate;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref DaysToRot, "DaysToRot", 0.5f);

            Scribe_Values.Look(ref RequireMedicalSkill, "RequireMedicalSkill", true);
            Scribe_Values.Look(ref SkillRequirement, "SkillRequirement", 4);

            Scribe_Values.Look(ref StackLimit, "StackLimit", 25);

            Scribe_Values.Look(ref HitPoints, "HitPoints", 60);
            Scribe_Values.Look(ref DeteriorationRate, "DeteriorationRate", 10);
        }

        public override bool Equals(GeneralProductDataBlock other)
        {
            return DaysToRot.Equals(other.DaysToRot) &&
                   RequireMedicalSkill.Equals(other.RequireMedicalSkill) &&
                   SkillRequirement.Equals(other.SkillRequirement) &&
                   StackLimit.Equals(other.StackLimit) &&
                   HitPoints.Equals(other.HitPoints) &&
                   DeteriorationRate.Equals(other.DeteriorationRate);
        }

        public override GeneralProductDataBlock DisplayControls(Listing_Standard listing)
        {
            float daysToRot = listing.LabeledSliderWithOverride(DaysToRot, "DaysToRot_BBS".Translate(), 0, 10, "DaysToRot_BBS_Tag".Translate());

            bool requireMedicalSkill = RequireMedicalSkill;
            listing.CheckboxLabeled("RequireMedicalSkill_BBS".Translate(), ref requireMedicalSkill, "RequireMedicalSkill_BBS_Tag".Translate());

            int skillRequirement = SkillRequirement;

            if (requireMedicalSkill)
                skillRequirement = (int)listing.LabeledSliderWithOverride(skillRequirement, "SkillRequirement_BBS".Translate(), 0, 10, "SkillRequirement_BBS_Tag".Translate());

            int stackLimit = (int)listing.LabeledSliderWithOverride(StackLimit, "StackLimit_BBS".Translate(), 25, 75, "StackLimit_BBS_Tag".Translate());
            float hitPoints = listing.LabeledSliderWithOverride(HitPoints, "HitPoints_BBS".Translate(), 0.1f, 10f, "HitPoints_BBS_Tag".Translate());
            float deteriorationRate = listing.LabeledSliderWithOverride(DeteriorationRate, "DeteriorationRate_BBS".Translate(), 0.1f, 100f, "DeteriorationRate_BBS_Tag".Translate());

            return new GeneralProductDataBlock
            {
                DaysToRot = daysToRot,
                RequireMedicalSkill = requireMedicalSkill,
                SkillRequirement = skillRequirement,
                StackLimit = stackLimit,
                HitPoints = hitPoints,
                DeteriorationRate = deteriorationRate,
            };
        }
    }
}
