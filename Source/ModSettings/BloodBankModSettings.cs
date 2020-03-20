// BloodBankSettings.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/12 7:03 PM


using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BloodBank.ModSettings
{
    public class BloodBankModSettings : Verse.ModSettings
    {
        private const float ScrollViewHeightSentinel = 100000f; //should always be bigger than the contents of the scroll view. if not and bugs happens, make this bigger.
        private float _scrollViewHeight = ScrollViewHeightSentinel;
        private Vector2 _scrollPos;

        private List<string> _bloodProductDefNames;


        private BloodDefDataBlock _bloodDefDataBlock;
        private BloodDefDataDisplay _bloodDefDisplay;


        private BloodProductDataBlock _bloodProductDataBlock;
        private BloodProductDataDisplay _bloodProductDisplay;

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
            //Simples! heh. hehehe. ah.

            //these might change the listing's overall height, depending on internal
            //variables. 
            if (_bloodDefDisplay == null)
                _bloodDefDisplay = new BloodDefDataDisplay(_bloodDefDataBlock);
            bool bloodDefChanged = _bloodDefDisplay.DoSettingsUI(listing);
            if (_bloodProductDisplay == null)
                _bloodProductDisplay = new BloodProductDataDisplay(_bloodProductDataBlock);
            bool bloodProductChanged = _bloodProductDisplay.DoSettingsUI(listing);

            //Listing_Standard.EndScrollView seems to work fine, but this is basically
            //what it does (far as I can tell the only important bit is height, but I
            //haven't used columns in this listing framework so 🤷‍♂)
            //Addendum: I added in the one-frame height offsets, but that can be done the
            //normal way too
            listing.End();
            Widgets.EndScrollView();
            viewRect.height = listing.CurHeight;

            float offset = _bloodDefDisplay.SectionDisplayHeightOffset +
                           _bloodProductDisplay.SectionDisplayHeightOffset;

            CheckHeight(viewRect.height, offset,bloodDefChanged || bloodProductChanged);
        }

        /// <summary>
        /// Ugh, ok here we go. By using a very large sentinel value InitialScrollViewHeight
        /// and resetting to that if any properties change, I was able to get past an annoying issue
        /// where removing a property from view would somehow break the listing's accumulation of height.
        /// Best guess for the cause is that something something procedural UI per frame,
        /// with a mismatch on redraw and a negative number thrown in. Bugger me if I can
        /// come up with a better answer atm.
        /// See <see cref="GeneralProductDataDisplay.DoSettingsUI"/> to dive further into
        /// the rabbit hole. Just look for the screaming comments.
        /// </summary>
        private void CheckHeight(float newHeight, float sfOffset, bool contentsChanged = false)
        {
            if (_scrollViewHeight.Equals(ScrollViewHeightSentinel))
            {
                Debug.Log($"{GetType()} - _scrollViewHeight is sentinel value. Updating to current content height ({newHeight}) + Total SingleFrameHeightOffset ({sfOffset}) = {newHeight+sfOffset}");
                _scrollViewHeight = newHeight + sfOffset;
            }
            else if (contentsChanged)
            {
                Debug.Log($"{GetType()} - content changed. Updating _scrollViewHeight to sentinel value ({ScrollViewHeightSentinel}) for reset. Total SingleFrameHeightOffset = { sfOffset }");
                _scrollViewHeight = ScrollViewHeightSentinel;
            }
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
