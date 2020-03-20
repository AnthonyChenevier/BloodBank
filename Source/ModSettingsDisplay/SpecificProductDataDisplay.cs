// SpecificBPProperties.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/16 12:30 PM


using BloodBank.ModSettingsData;
using Verse;

namespace BloodBank.ModSettingsDisplay
{
    public class SpecificProductDataDisplay : ModSettingsDataDisplay<SpecificProductDataBlock>
    {
        public SpecificProductDataDisplay(SpecificProductDataBlock dataBlock, float sectionHeight) : base(dataBlock, sectionHeight) { }
    }
}
