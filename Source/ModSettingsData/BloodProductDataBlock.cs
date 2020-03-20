// BloodProductDataBlock.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/20 9:46 PM


using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BloodBank.ModSettingsData
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
            Scribe_Deep.Look(ref GeneralProperties, nameof(GeneralProperties));
            Scribe_Collections.Look(ref SpecificProperties, nameof(SpecificProperties), LookMode.Deep);
            _bloodProducts = SpecificProperties.Select(o => o.ThingDefName).ToList();
        }


        public override bool Equals(BloodProductDataBlock other)
        {
            return GeneralProperties.Equals(other.GeneralProperties) &&
                   SpecificProperties.Equals(other.SpecificProperties);
        }

        public override BloodProductDataBlock DisplayControls(Listing_Standard listing) { return this; }
    }
}
