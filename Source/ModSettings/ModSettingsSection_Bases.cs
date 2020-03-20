// BBSettingProperties.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/19 2:53 PM


using UnityEngine;
using Verse;

namespace BloodBank.ModSettings
{
    public abstract class ModSettingsDataBlock<T> : IExposable where T : ModSettingsDataBlock<T>
    {
        public virtual void SetDefault() { }
        public virtual void CopyFrom(T other) { }
        public virtual void ExposeData() { }
        public abstract bool Equals(T other);
    }

    public abstract class ModSettingsDataDisplay<T> where T : ModSettingsDataBlock<T>
    {
        protected const float SectionHeightSentinel = 10000f;

        protected readonly T DataBlock;

        public float SectionHeight { get; protected set; } = SectionHeightSentinel;
        public float FullHeight { get; protected set; }
        public float SectionDisplayHeightOffset { get; protected set; }

        public ModSettingsDataDisplay(T dataBlock)
        {
            DataBlock = dataBlock;
        }

        public abstract bool DoSettingsUI(Listing_Standard mainListing);

        protected virtual bool SettingHeader(Listing_Standard listing, TaggedString headingLabel, string extra = null)
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

        protected bool CopyDataIfChanged(T newData)
        {
            bool changed = !newData.Equals(DataBlock);
            if (changed)
                DataBlock.CopyFrom(newData);
            return changed;
        }

        /// <summary>
        /// Another check to fix listing heights, this time for sections. hooray. WOULD YOU LIKE TO KNOW MORE? >>>> <see cref="BloodBankModSettings.CheckHeight"/>
        /// </summary>
        /// <param name="sectionHeight"></param>
        /// <param name="contentsChanged"></param>
        protected virtual bool CheckAndUpdateSectionHeight(float sectionHeight, bool contentsChanged = false)
        {
            SectionDisplayHeightOffset = 0;
            if (SectionHeight.Equals(SectionHeightSentinel))
            {
                SectionDisplayHeightOffset = sectionHeight - SectionHeightSentinel;
                SectionHeight = sectionHeight;
                Debug.Log($"{GetType()} - SectionHeight was sentinel value. Updated to {SectionHeight} for next frame. Height offset for this frame is {SectionDisplayHeightOffset}");
            }
            else if (contentsChanged)
            {
                Debug.Log($"{GetType()} - Section contents changed. Updating section height to sentinel value to force recalculation. Height offset for this frame is {SectionDisplayHeightOffset}");
                SectionHeight = SectionHeightSentinel;
            }
            return contentsChanged;
        }
    }
}
