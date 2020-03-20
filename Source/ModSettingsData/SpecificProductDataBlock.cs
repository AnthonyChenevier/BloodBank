// SpecificProductDataBlock.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/20 9:50 PM


using System.Linq;
using Verse;

namespace BloodBank.ModSettingsData
{
    public class SpecificProductDataBlock : ModSettingsDataBlock<SpecificProductDataBlock>
    {
        public string ThingDefName;

        public string HediffDefName;
        public IntRange EffectTime;


        public override void SetDefault()
        {
            ThingDef thingDef = BloodDefCache.BloodProducts.FirstOrDefault(o => o.defName == ThingDefName);
            if (thingDef == null)
            {
                Debug.Error($"No Blood product found for {ThingDefName}");
                return;
            }

            CompProperties_BloodProduct bloodProduct = thingDef.GetCompProperties<CompProperties_BloodProduct>();

            HediffDefName = bloodProduct.hediffDef.defName;
            EffectTime = bloodProduct.hediffDef.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks;
        }

        public override void CopyFrom(SpecificProductDataBlock other)
        {
            if (ThingDefName != other.ThingDefName)
            {
                Debug.Error("SpecificProductProperties.CopyProperties() requires matching Def fields before copy");
                return;
            }

            HediffDefName = other.HediffDefName;
            EffectTime = other.EffectTime;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref ThingDefName, "ThingDefName");
            Scribe_Values.Look(ref HediffDefName, "HediffDefName");
            Scribe_Values.Look(ref EffectTime, "EffectTime", new IntRange(60000, 60000));
        }

        public override bool Equals(SpecificProductDataBlock other)
        {
            return ThingDefName.Equals(other.ThingDefName) &&
                   HediffDefName.Equals(other.HediffDefName) &&
                   EffectTime.Equals(other.EffectTime);
        }

        public override SpecificProductDataBlock DisplayControls(Listing_Standard listing)
        {
            string hediff = listing.TextFieldLabeled("HediffDefName_BBS".Translate(), HediffDefName, "HediffDefName_BBS_Tag".Translate());

            //this is an InRange, but treat it like a simple int.
            //Additionally, hediffDuration is stored as ticks, but
            //that is unintuitive so express as a fraction of 24hrs
            //on the slider.
            IntRange hediffDuration = new IntRange();
            hediffDuration.max = hediffDuration.min =
                                     (int)(listing.LabeledSliderWithOverride(EffectTime.max / 60000f,
                                                                                    "EffectTime_BBS".Translate(), 0.25f, 60f,
                                                                                    "EffectTime_BBS_Tag".Translate()) * 60000f);
            return new SpecificProductDataBlock
            {
                ThingDefName = ThingDefName,
                HediffDefName = hediff,
                EffectTime = hediffDuration,
            };
        }
    }
}
