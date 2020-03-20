// GeneralProductDataDisplay.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/20 9:49 PM


using BloodBank.ModSettingsData;
using Verse;

namespace BloodBank.ModSettingsDisplay
{
    public class GeneralProductDataDisplay : ModSettingsDataDisplay<GeneralProductDataBlock>
    {
        public GeneralProductDataDisplay(GeneralProductDataBlock dataBlock, float sectionHeight) : base(dataBlock, sectionHeight) { }
    }
}
