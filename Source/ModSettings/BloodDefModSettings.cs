// BloodDefModSettingsDisplay.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/19 2:54 PM


using Verse;

namespace BloodBank.ModSettings
{
    public class BloodDefDataBlock : ModSettingsDataBlock<BloodDefDataBlock>
    {
        public float Amount;
        public float HarvestEfficiencyFactor;
        public float BloodLossReqdForBadThought;

        public int StackLimit;
        public float DaysToRot;

        public int SkillRequirement;

        public float HumanBloodCostMeatMultiplier;
        public float AnimalBloodCostMeatMultiplier;

        public float HitPoints;
        public float DeteriorationRate;

        public float Nutrition;
        public float FoodPoisonChanceFixedHuman;

        public override void SetDefault()
        {
            Amount = 0.1f;
            HarvestEfficiencyFactor = 2f;
            BloodLossReqdForBadThought = 0.4f;

            DaysToRot = 0.5f;

            SkillRequirement = 4;

            StackLimit = 25;

            HitPoints = 60;
            DeteriorationRate = 10;
            Nutrition = 0.01f;
            FoodPoisonChanceFixedHuman = 0.02f;

            HumanBloodCostMeatMultiplier = 4f;
            AnimalBloodCostMeatMultiplier = 0.5f;
        }

        public override void CopyFrom(BloodDefDataBlock other)
        {
            Amount = other.Amount;
            BloodLossReqdForBadThought = other.BloodLossReqdForBadThought;
            HarvestEfficiencyFactor = other.HarvestEfficiencyFactor;
            DaysToRot = other.DaysToRot;
            SkillRequirement = other.SkillRequirement;
            StackLimit = other.StackLimit;
            HitPoints = other.HitPoints;
            DeteriorationRate = other.DeteriorationRate;
            Nutrition = other.Nutrition;
            FoodPoisonChanceFixedHuman = other.FoodPoisonChanceFixedHuman;
            AnimalBloodCostMeatMultiplier = other.AnimalBloodCostMeatMultiplier;
            HumanBloodCostMeatMultiplier = other.HumanBloodCostMeatMultiplier;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Amount, "Amount", 0.1f);
            Scribe_Values.Look(ref HarvestEfficiencyFactor, "HarvestEfficiencyFactor", 2f);
            Scribe_Values.Look(ref BloodLossReqdForBadThought, "BloodLossReqdForBadThought", 0.4f);
            Scribe_Values.Look(ref DaysToRot, "DaysToRot", 0.5f);

            Scribe_Values.Look(ref SkillRequirement, "SkillRequirement", 4);

            Scribe_Values.Look(ref StackLimit, "StackLimit", 25);

            Scribe_Values.Look(ref HitPoints, "HitPoints", 60);
            Scribe_Values.Look(ref DeteriorationRate, "DeteriorationRate", 10);
            Scribe_Values.Look(ref Nutrition, "Nutrition", 0.01f);
            Scribe_Values.Look(ref FoodPoisonChanceFixedHuman, "FoodPoisonChanceFixedHuman", 0.02f);

            Scribe_Values.Look(ref HumanBloodCostMeatMultiplier, "HumanBloodCostMeatMultiplier", 4f);
            Scribe_Values.Look(ref AnimalBloodCostMeatMultiplier, "AnimalBloodCostMeatMultiplier", 0.5f);
        }

        public override bool Equals(BloodDefDataBlock other)
        {
            return Amount.Equals(other.Amount) &&
                   HarvestEfficiencyFactor.Equals(other.HarvestEfficiencyFactor) &&
                   BloodLossReqdForBadThought.Equals(other.BloodLossReqdForBadThought) &&
                   StackLimit.Equals(other.StackLimit) &&
                   DaysToRot.Equals(other.DaysToRot) &&
                   SkillRequirement.Equals(other.SkillRequirement) &&
                   HumanBloodCostMeatMultiplier.Equals(other.HumanBloodCostMeatMultiplier) &&
                   AnimalBloodCostMeatMultiplier.Equals(other.AnimalBloodCostMeatMultiplier) &&
                   HitPoints.Equals(other.HitPoints) &&
                   DeteriorationRate.Equals(other.DeteriorationRate) &&
                   Nutrition.Equals(other.Nutrition) &&
                   FoodPoisonChanceFixedHuman.Equals(other.FoodPoisonChanceFixedHuman);
        }

    }
    public class BloodDefDataDisplay: ModSettingsDataDisplay<BloodDefDataBlock>
    {
        public BloodDefDataDisplay(BloodDefDataBlock dataBlock) : base(dataBlock) { }

        public override bool DoSettingsUI(Listing_Standard mainListing)
        {
            if (SettingHeader(mainListing, "BloodDefProperties".Translate()))
            {
                DataBlock.SetDefault();
                return true;
            }

            Listing_Standard sectionListing = mainListing.BeginSection(SectionHeight);

            float amount =
                sectionListing.LabeledSliderWithOverride(DataBlock.Amount, "Amount_BBS".Translate(), 
                                                         0.05f, 0.5f, "Amount_BBS_Tag".Translate());
            float badThought = 
                sectionListing.LabeledSliderWithOverride(DataBlock.BloodLossReqdForBadThought, "BloodLossReqdForBadThought_BBS".Translate(), 
                                                         0f, 1f, "BloodLossReqdForBadThought_BBS_Tag".Translate());
            float harvestEfficiencyFactor =
                sectionListing.LabeledSliderWithOverride(DataBlock.HarvestEfficiencyFactor, "HarvestEfficiencyFactor_BBS".Translate(), 
                                                         0.5f, 2f, "HarvestEfficiencyFactor_BBS_Tag".Translate());
            float daysToRot =
                sectionListing.LabeledSliderWithOverride(DataBlock.DaysToRot, "DaysToRot_BBS".Translate(), 
                                                         0, 10, "DaysToRot_BBS_Tag".Translate());
            int skillRequirement = 
                (int)sectionListing.LabeledSliderWithOverride(DataBlock.SkillRequirement, "SkillRequirement_BBS".Translate(),
                                                              0, 10, "SkillRequirement_BBS_Tag".Translate());
            int stackLimit =
                (int)sectionListing.LabeledSliderWithOverride(DataBlock.StackLimit, "StackLimit_BBS".Translate(), 
                                                              25, 75, "StackLimit_BBS_Tag".Translate());

            float hitPoints = 
                sectionListing.LabeledSliderWithOverride(DataBlock.HitPoints, "HitPoints_BBS".Translate(),
                                                         0.1f, 10f, "HitPoints_BBS_Tag".Translate());

            float deteriorationRate = 
                sectionListing.LabeledSliderWithOverride(DataBlock.DeteriorationRate, "DeteriorationRate_BBS".Translate(),
                                                         0.1f, 100f, "DeteriorationRate_BBS_Tag".Translate());

            float nutrition = 
                sectionListing.LabeledSliderWithOverride(DataBlock.Nutrition, "Nutrition_BBS".Translate(),
                                                         0.01f, 0.1f, "Nutrition_BBS_Tag".Translate());
            float foodPoisonChanceFixedHuman =
                sectionListing.LabeledSliderWithOverride(DataBlock.FoodPoisonChanceFixedHuman, "FoodPoisonChanceFixedHuman_BBS".Translate(), 
                                                         0f, 1f, "FoodPoisonChanceFixedHuman_BBS_Tag".Translate());
            float animalBloodCostMeatMultiplier =
                sectionListing.LabeledSliderWithOverride(DataBlock.AnimalBloodCostMeatMultiplier, "AnimalBloodCostMeatMultiplier_BBS".Translate(), 
                                                         0.1f, 10f, "AnimalBloodCostMeatMultiplier_BBS_Tag".Translate());
            float humanBloodCostMeatMultiplier =
                sectionListing.LabeledSliderWithOverride(DataBlock.HumanBloodCostMeatMultiplier, "HumanBloodCostMeatMultiplier_BBS".Translate(), 
                                                         0.1f, 10f, "HumanBloodCostMeatMultiplier_BBS_Tag".Translate());

            mainListing.EndSection(sectionListing);

            bool contentsChanged = CopyDataIfChanged(new BloodDefDataBlock 
            {
                Amount = amount,
                BloodLossReqdForBadThought = badThought,
                HarvestEfficiencyFactor = harvestEfficiencyFactor,
                DaysToRot = daysToRot,
                SkillRequirement = skillRequirement,
                StackLimit = stackLimit,
                HitPoints = hitPoints,
                DeteriorationRate = deteriorationRate,
                Nutrition = nutrition,
                FoodPoisonChanceFixedHuman = foodPoisonChanceFixedHuman,
                AnimalBloodCostMeatMultiplier = animalBloodCostMeatMultiplier,
                HumanBloodCostMeatMultiplier = humanBloodCostMeatMultiplier,
            });

            return CheckAndUpdateSectionHeight(sectionListing.CurHeight, contentsChanged);
        }
    }
}
