// HediffComp_HealFactor.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/02/10 4:40 PM


using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BloodBank {

    public class HediffCompProperties_HealFactor : HediffCompProperties
    {
        public float healFactor = 2f;
        public HediffCompProperties_HealFactor()
        {
            this.compClass = typeof(HediffComp_HealFactor);
        }
    }

    public class HediffComp_HealFactor : HediffComp
    {
        public HediffCompProperties_HealFactor Props => (HediffCompProperties_HealFactor)this.props;
        
        public override string CompTipStringExtra => $"{"HealFactor".Translate($"{Props.healFactor:F1}")}";
    }
}
