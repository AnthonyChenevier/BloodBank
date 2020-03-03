// Recipe_GiveBlood.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/02/04 12:24 AM


using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BloodBank {
    public class Recipe_GiveBlood : Recipe_AdministerIngestible
    {
        public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
            return false;
        }
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (!(ingredients[0] is ThingWithComps) || !ingredients[0].def.HasComp(typeof(CompUseEffect_GiveBlood)))
            {
                Log.Error("Blood Bank - Give blood operation failed (ingredients[0] has no BloodPackComp)");
                return;
            }

            BloodBankUtilities.ApplyBloodPack(pawn, ingredients[0].def);
        }

        public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part) { return recipe.label; }
    }
}
