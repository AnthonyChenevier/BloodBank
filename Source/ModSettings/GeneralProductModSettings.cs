// GeneralProductProperties.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/16 12:30 PM


using Verse;

namespace BloodBank.ModSettings {
    public class GeneralProductDataDisplay: ModSettingsDataDisplay<GeneralProductDataBlock>
    {
        public GeneralProductDataDisplay(GeneralProductDataBlock dataBlock) : base(dataBlock) { }

        /// <summary>
        /// 'ERE BE DRAGONS. See
        /// <see cref="BloodBankModSettings.CheckHeight"/> for more info.
        /// </summary>
        /// <returns></returns>
        public override bool DoSettingsUI(Listing_Standard mainListing)
        {
            if (SettingHeader(mainListing, "GeneralBloodProductProperties".Translate()))
            {
                DataBlock.SetDefault();
                return true;
            }

            Listing_Standard sectionListing = mainListing.BeginSection(SectionHeight);

            float daysToRot = sectionListing.LabeledSliderWithOverride(DataBlock.DaysToRot, "DaysToRot_BBS".Translate(), 0, 10, "DaysToRot_BBS_Tag".Translate());

            //WHY DOES THIS MAKE THINGS SO HARD
            bool requireMedicalSkill = DataBlock.RequireMedicalSkill; 
            sectionListing.CheckboxLabeled("RequireMedicalSkill_BBS".Translate(), ref requireMedicalSkill, "RequireMedicalSkill_BBS_Tag".Translate()); 

            int skillRequirement = DataBlock.SkillRequirement;
            //THIS CONDITIONAL FUCKER RIGHT HERE, OFFICER. IT SMACKED ME ON THE BUM-BUM. IF WE GO FROM true TO false, EVERYTHING IS FINE BUT IF IT'S false TO true THEN WOE BETIDES YOU TRAVELLER
            if (requireMedicalSkill)
                skillRequirement = (int)sectionListing.LabeledSliderWithOverride(skillRequirement, "SkillRequirement_BBS".Translate(), 0, 10, "SkillRequirement_BBS_Tag".Translate());

            int stackLimit = (int)sectionListing.LabeledSliderWithOverride(DataBlock.StackLimit, "StackLimit_BBS".Translate(), 25, 75, "StackLimit_BBS_Tag".Translate());
            float hitPoints = sectionListing.LabeledSliderWithOverride(DataBlock.HitPoints, "HitPoints_BBS".Translate(), 0.1f, 10f, "HitPoints_BBS_Tag".Translate());
            float deteriorationRate = sectionListing.LabeledSliderWithOverride(DataBlock.DeteriorationRate, "DeteriorationRate_BBS".Translate(), 0.1f, 100f, "DeteriorationRate_BBS_Tag".Translate());

            mainListing.EndSection(sectionListing);

            bool contentsChanged = CopyDataIfChanged(new GeneralProductDataBlock
            {
                DaysToRot = daysToRot,
                RequireMedicalSkill = requireMedicalSkill,
                SkillRequirement = skillRequirement,
                StackLimit = stackLimit,
                HitPoints = hitPoints,
                DeteriorationRate = deteriorationRate,
            });
            return CheckAndUpdateSectionHeight(sectionListing.CurHeight, contentsChanged);
        }
    }

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
            base.ExposeData();
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
    }
}
