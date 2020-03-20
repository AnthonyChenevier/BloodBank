// CompBloodProduct.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/04 7:24 PM


using RimWorld;
using Verse;

namespace BloodBank {

    public class CompProperties_BloodProduct : CompProperties
    {
        public HediffDef hediffDef;
        public float severity;
        public CompProperties_BloodProduct() { compClass = typeof(CompBloodProduct); }
    }

    public class CompBloodProduct : ThingComp
    {
        public CompProperties_BloodProduct Props =>(CompProperties_BloodProduct)props;
    }
}
