// BloodBankUIExtensions.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/18 3:15 AM


using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BloodBank {
    public static class UIExtensions {
        public static float LabeledSliderWithOverride(this Listing_Standard listing, float val, TaggedString label, float min, float max, string tooltip = null)
        {
            Rect inRect = listing.GetRect(Text.LineHeight);

            Rect leftRect = inRect.LeftPart(.85f);

            Rect labelRect = leftRect.LeftHalf();
            Rect sliderRect = leftRect.RightHalf();

            Rect numericInputRect = inRect.RightPart(0.1f);

            Rect sliderOffsetRect = sliderRect;
            float yOffset = Text.LineHeight / 2;
            sliderOffsetRect.position += new Vector2(0, yOffset);


            Widgets.Label(labelRect, label.Resolve());
            float num = Widgets.HorizontalSlider(sliderOffsetRect, val, min, max, false, null, min.ToString(), max.ToString());
            num = float.TryParse(Widgets.TextField(numericInputRect, num.ToString("##0.0#")), out float t) ? t : num;
            listing.Gap(yOffset);

            if (tooltip != null)
                SetTooltip(labelRect, tooltip);

            if (!num.Equals(val))
                SoundDefOf.DragSlider.PlayOneShotOnCamera();

            listing.Gap(listing.verticalSpacing);
            return num;
        }

        public static string TextFieldLabeled(this Listing_Standard listing, TaggedString label, string value, string tooltip=null)
        {
            Rect rect = listing.GetRect(Text.LineHeight);
            Rect labelRect = rect.LeftHalf();
            Widgets.Label(labelRect, label);
            string output = Widgets.TextField(rect.RightHalf(), value);
            if (tooltip != null)
                SetTooltip(labelRect, tooltip);
            listing.Gap(listing.verticalSpacing);
            return output;
        }

        public static void SetTooltip(Rect rect, string tooltip)
        {
            if (Mouse.IsOver(rect))
                Widgets.DrawHighlight(rect);
            TooltipHandler.TipRegion(rect, tooltip);
        }

        public static void BeginScrollViewBB(this Listing listing, Rect outRect, ref Vector2 scrollPosition, ref Rect viewRect)
        {
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            listing.Begin(viewRect);
        }

        public static void EndScrollViewBB(this Listing listing, ref Rect viewRect)
        {
            listing.End();
            Widgets.EndScrollView();
            viewRect.height = listing.CurHeight;
        }
    }
}
