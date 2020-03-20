// BloodProductProperties.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/14 2:16 PM


using System.Collections.Generic;
using BloodBank.ModSettingsData;
using Verse;

namespace BloodBank.ModSettingsDisplay
{
    public class BloodProductDataDisplay : ModSettingsDataDisplay<BloodProductDataBlock>
    {
        private readonly Dictionary<SpecificProductDataBlock, float> _specificPropsSectionHeights = new Dictionary<SpecificProductDataBlock, float>();
        private float _generalPropsSectionHeight = 0;

        public BloodProductDataDisplay(BloodProductDataBlock dataBlock, float sectionHeight) : base(dataBlock, sectionHeight) { }

        public override bool DoSettingsUI(Listing_Standard mainListing, TaggedString sectionLabel, string extra = null)
        {
            if (SectionHeader(mainListing, sectionLabel))
            {
                DataBlock.SetDefault();
                return true;
            }

            Listing_Standard sectionListing = mainListing.BeginSection(SectionHeight);


            ModSettingsDataDisplay<GeneralProductDataBlock> dataDisplay = new ModSettingsDataDisplay<GeneralProductDataBlock>(DataBlock.GeneralProperties, _generalPropsSectionHeight);
            bool generalPropsChanged = dataDisplay.DoSettingsUI(sectionListing, "GeneralBloodProductProperties".Translate());
            _generalPropsSectionHeight = dataDisplay.SectionHeight;
            float sectionHeightCumulative = dataDisplay.FullHeight + dataDisplay.SectionDisplayHeightOffset;


            bool specificPropsChanged = false;
            foreach (SpecificProductDataBlock specificProperties in DataBlock.SpecificProperties)
            {
                if (!_specificPropsSectionHeights.ContainsKey(specificProperties))
                    _specificPropsSectionHeights.Add(specificProperties, 0);
                SpecificProductDataDisplay display = new SpecificProductDataDisplay(specificProperties, _specificPropsSectionHeights[specificProperties]);
                if (display.DoSettingsUI(sectionListing, "SpecificBloodProductProperties".Translate(), specificProperties.ThingDefName))
                    specificPropsChanged = true;

                _specificPropsSectionHeights[specificProperties] = display.SectionHeight;
                sectionHeightCumulative += display.FullHeight + display.SectionDisplayHeightOffset;
            }


            mainListing.EndSection(sectionListing);

            bool changed = generalPropsChanged || specificPropsChanged;
            FinalizeSection(sectionHeightCumulative, changed);
            return changed;
        }
    }
}
