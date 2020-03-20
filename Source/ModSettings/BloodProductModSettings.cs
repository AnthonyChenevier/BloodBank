// BloodProductProperties.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/14 2:16 PM
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BloodBank.ModSettings
{
    public class BloodProductDataBlock : ModSettingsDataBlock<BloodProductDataBlock>
    {
        public GeneralProductDataBlock GeneralProperties;

        public List<SpecificProductDataBlock> SpecificProperties;

        private List<string> _bloodProducts;

        //used by expose data
        public BloodProductDataBlock() { }

        public BloodProductDataBlock(List<string> bloodProducts)
        {
            _bloodProducts = bloodProducts;
        }

        public override void SetDefault()
        {
            if (_bloodProducts == null)
            {
                Debug.Error("BloodProductModSettings_Data.SetDefault() called without initialization. ");
                return;
            }

            if (GeneralProperties == null)
                GeneralProperties = new GeneralProductDataBlock();

            GeneralProperties.SetDefault();

            if (SpecificProperties == null || SpecificProperties.Any(o => o == null))
            {
                SpecificProperties = _bloodProducts.Select(o => new SpecificProductDataBlock { ThingDefName = o }).ToList();
            }

            SpecificProperties.ForEach(sp => sp.SetDefault());
        }

        public override void CopyFrom(BloodProductDataBlock other)
        {
            GeneralProperties = other.GeneralProperties;
            SpecificProperties = other.SpecificProperties;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref GeneralProperties, nameof(GeneralProperties));
            Scribe_Collections.Look(ref SpecificProperties, nameof(SpecificProperties), LookMode.Deep);
            _bloodProducts = SpecificProperties.Select(o => o.ThingDefName).ToList();
        }


        public override bool Equals(BloodProductDataBlock other)
        {
            return GeneralProperties.Equals(other.GeneralProperties) &&
                   SpecificProperties.Equals(other.SpecificProperties);
        }

    }


    public class BloodProductDataDisplay: ModSettingsDataDisplay<BloodProductDataBlock>
    {
        private GeneralProductDataDisplay _generalSettings_Display;
        private Dictionary<SpecificProductDataBlock, SpecificProductDataDisplay> _specificSettingsDisplays = new Dictionary<SpecificProductDataBlock, SpecificProductDataDisplay>();
        public BloodProductDataDisplay(BloodProductDataBlock dataBlock) : base(dataBlock) { }

        public override bool DoSettingsUI(Listing_Standard mainListing)
        {
            if (SettingHeader(mainListing, "BloodProductProperties".Translate()))
            {
                DataBlock.SetDefault();
                return true;
            }

            Listing_Standard sectionListing = mainListing.BeginSection(SectionHeight);

            if (_generalSettings_Display == null)
            {
                _generalSettings_Display = new GeneralProductDataDisplay(DataBlock.GeneralProperties);
            }

            bool generalPropsChanged = _generalSettings_Display.DoSettingsUI(sectionListing);
            float sectionHeightCumulative = _generalSettings_Display.FullHeight + _generalSettings_Display.SectionDisplayHeightOffset;


            bool specificPropsChanged = false;
            foreach (SpecificProductDataBlock specificProperties in DataBlock.SpecificProperties)
            {
                SpecificProductDataDisplay display = GetDisplayFor(specificProperties);
                if (display.DoSettingsUI(sectionListing))
                    specificPropsChanged = true;
                sectionHeightCumulative += display.FullHeight + display.SectionDisplayHeightOffset;
            }
            
            mainListing.EndSection(sectionListing);

            return CheckAndUpdateSectionHeight(sectionHeightCumulative, generalPropsChanged || specificPropsChanged);
        }

        private SpecificProductDataDisplay GetDisplayFor(SpecificProductDataBlock specificProperties)
        {
            SpecificProductDataDisplay display;
            if (!_specificSettingsDisplays.ContainsKey(specificProperties) || _specificSettingsDisplays[specificProperties] == null)
            {
                display = new SpecificProductDataDisplay(specificProperties);
                _specificSettingsDisplays.Add(specificProperties, display);
            }
            else
                display = _specificSettingsDisplays[specificProperties];
            return display;
        }
    }
}
