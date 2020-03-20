// BloodBankSettings.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/12 7:03 PM


using System.Collections.Generic;
using BloodBank.ModSettingsData;
using BloodBank.ModSettingsDisplay;
using UnityEngine;
using Verse;

namespace BloodBank
{
    public class BloodBankModSettings : ModSettings
    {
        private const float ScrollViewHeightSentinel = 100000f; //should always be bigger than the contents of the scroll view. if not and bugs happens, make this bigger.
        
        private float _scrollViewHeight = ScrollViewHeightSentinel;
        private Vector2 _scrollPos;

        private List<string> _bloodProductDefNames;
        
        private BloodDefDataBlock _bloodDefDataBlock;
        private BloodProductDataBlock _bloodProductDataBlock;

        private float _bloodDefPreviousHeight = 0;
        private float _bloodProductPreviousHeight = 0;

        public void InitializeWithDefaultData(List<string> bloodProductDefNames)
        {
            _bloodProductDefNames = bloodProductDefNames;

            if (_bloodDefDataBlock == null)
            {
                _bloodDefDataBlock = new BloodDefDataBlock();
                _bloodDefDataBlock.SetDefault();
            }

            if (_bloodProductDataBlock == null)
            {
                _bloodProductDataBlock = new BloodProductDataBlock(_bloodProductDefNames);
                _bloodProductDataBlock.SetDefault();
            }
        }

        public bool Empty => _bloodDefDataBlock == null || _bloodProductDataBlock == null;

        public override void ExposeData()
        {
            Debug.Log($"Exposing Setting Data");
            Scribe_Deep.Look(ref _bloodDefDataBlock, "BloodDefProperties");
            Scribe_Deep.Look(ref _bloodProductDataBlock, "BloodProductProperties");

            base.ExposeData();
        }


        //BUG: FARK THIS METHOD AND ALL WHO DWELL WITHIN HER. HERE BE FUCKIN' DRAGONS, YO.
        //NOTE: it actually works fine. I've commented heavily in case someone finds it helpful
        public void DoSettingWindowContents(Rect inRect)
        {
            if (Empty)
            {
                Debug.Error("DoSettingWindowContents called before settings were initialized");
                return;
            }

            Listing_Standard listing = new Listing_Standard(GameFont.Small);

            Rect viewRect = inRect;
            viewRect.width -= 16f;
            viewRect.height = _scrollViewHeight;
            //Listing_Standard.BeginScrollView is broken and moves the mouse target
            //rect used for capturing mouse scroll events with the viewRect - the
            //immediately noticeable effect of which is a 'dead spot' in scrolling
            //once you reach a certain point in the listing. Because this is due to
            //the wandering target Rect is was hard to diagnose if the mouse was still
            //inside the moving, invisible rect.
            //Anyway, This is what that method should have done.
            Widgets.BeginScrollView(inRect, ref _scrollPos, viewRect);
            listing.Begin(viewRect);

            ModSettingsDataDisplay<BloodDefDataBlock> bloodDefDisplay = new ModSettingsDataDisplay<BloodDefDataBlock>(_bloodDefDataBlock, _bloodDefPreviousHeight);
            bool bloodDefChanged = bloodDefDisplay.DoSettingsUI(listing, "BloodDefProperties".Translate());
            _bloodDefPreviousHeight = bloodDefDisplay.SectionHeight;

            BloodProductDataDisplay productDataDisplay = new BloodProductDataDisplay(_bloodProductDataBlock, _bloodProductPreviousHeight);
            bool bloodProductChanged = productDataDisplay.DoSettingsUI(listing, "BloodProductProperties".Translate());
            _bloodProductPreviousHeight = productDataDisplay.SectionHeight;

            //Listing_Standard.EndScrollView seems to work fine, but this is basically
            //what it does (far as I can tell the only important bit is height, but I
            //haven't used columns in this listing framework so 🤷‍♂)
            listing.End();
            Widgets.EndScrollView();
            viewRect.height = listing.CurHeight;

            float offset = bloodDefDisplay.SectionDisplayHeightOffset +
                           productDataDisplay.SectionDisplayHeightOffset;

            if (_scrollViewHeight.Equals(ScrollViewHeightSentinel))
                _scrollViewHeight = viewRect.height + offset;
            else if (bloodDefChanged || bloodProductChanged)
                _scrollViewHeight = ScrollViewHeightSentinel;
        }

        public BloodDefDataBlock GetBloodDefProperties()
        {
            if (!Empty)
            {
                Debug.Log("GetBloodDefProperties() - Found existing properties");
                return _bloodDefDataBlock;
            }

            //no properties exist. create temporary ones
            BloodDefDataBlock tempDataBlock = new BloodDefDataBlock();
            tempDataBlock.SetDefault();
            Debug.Log("GetBloodDefProperties() - Temporary default properties created");
            return tempDataBlock;

        }
    }
}
