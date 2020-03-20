// ModSettingsDataBlock.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/20 9:51 PM


using Verse;

namespace BloodBank.ModSettingsData
{
    public abstract class ModSettingsDataBlock<T> : IExposable where T : ModSettingsDataBlock<T>
    {
        public abstract void SetDefault();
        public abstract void CopyFrom(T other);
        public abstract void ExposeData();
        public abstract bool Equals(T other);
        public abstract T DisplayControls(Listing_Standard listing);
    }
}
