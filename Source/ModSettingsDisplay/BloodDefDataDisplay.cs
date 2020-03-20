// BloodDefModSettingsDisplay.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/19 2:54 PM


using BloodBank.ModSettingsData;
using Verse;

namespace BloodBank.ModSettingsDisplay
{
    public class BloodDefDataDisplay : ModSettingsDataDisplay<BloodDefDataBlock>
    {
        public BloodDefDataDisplay(BloodDefDataBlock dataBlock, float sectionHeight) : base(dataBlock, sectionHeight) { }
    }
}
