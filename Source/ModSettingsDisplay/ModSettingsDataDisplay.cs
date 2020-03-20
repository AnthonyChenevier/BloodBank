// BBSettingProperties.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/19 2:53 PM


using BloodBank.ModSettingsData;
using UnityEngine;
using Verse;

namespace BloodBank.ModSettingsDisplay
{
    public class ModSettingsDataDisplay<T> where T : ModSettingsDataBlock<T>
    {
        private const float SectionHeightSentinel = 10000f;

        protected readonly T DataBlock;

        public float SectionHeight { get; private set; }
        public float FullHeight { get; private set; }
        public float SectionDisplayHeightOffset { get; private set; }

        public ModSettingsDataDisplay(T dataBlock, float sectionHeight)
        {
            DataBlock = dataBlock;
            SectionHeight = sectionHeight.Equals(0f) ? SectionHeightSentinel : sectionHeight;
        }
        
        protected bool SectionHeader(Listing_Standard listing, TaggedString headingLabel, string extra = null)
        {
            Rect headerRect = listing.GetRect(Text.LineHeight);

            TaggedString taggedString = $"<b>{headingLabel}</b> {(extra != null ? $" ({extra})" : "")}";
            Widgets.Label(headerRect.LeftHalf(), taggedString);
            
            Rect buttonRect = headerRect.RightHalf().RightPart(.5f);
            UIExtensions.SetTooltip(buttonRect, "ResetToDefault_Tip".Translate(headingLabel));
            bool doReset = Widgets.ButtonText(buttonRect, "ResetToDefault".Translate());

            listing.Gap(listing.verticalSpacing);
            FullHeight = headerRect.height + listing.verticalSpacing + SectionHeight + 8f;
            return doReset;
        }

        public virtual bool DoSettingsUI(Listing_Standard mainListing, TaggedString sectionLabel, string extra = null)
        {
            if (SectionHeader(mainListing, sectionLabel, extra))
            {
                DataBlock.SetDefault();
                return true;
            }

            Listing_Standard sectionListing = mainListing.BeginSection(SectionHeight);

            bool changed = ProcessNewData(DataBlock.DisplayControls(sectionListing));

            mainListing.EndSection(sectionListing);

            FinalizeSection(sectionListing.CurHeight, changed);

            return changed;
        }

        protected bool ProcessNewData(T newData)
        {
            bool changed = !newData.Equals(DataBlock);
            if (changed)
                DataBlock.CopyFrom(newData);
            return changed;
        }

        protected void FinalizeSection(float sectionHeight, bool contentsChanged = false)
        {
            SectionDisplayHeightOffset = 0;
            if (SectionHeight.Equals(SectionHeightSentinel))
            {
                SectionDisplayHeightOffset = sectionHeight - SectionHeightSentinel;
                SectionHeight = sectionHeight;
            }
            else if (contentsChanged)
                SectionHeight = SectionHeightSentinel;
        }
    }
}
