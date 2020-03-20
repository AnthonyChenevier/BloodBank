// Recipe_GiveBlood.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/02/04 12:24 AM


using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BloodBank
{
    //use Recipe_AdministerIngestible as superclass because it
    //is the only vanilla method to force an icon (the fixed
    //ingredient for the recipe) to be used in operations bill menu
    public class Recipe_AdministerBloodTransfusion : Recipe_AdministerIngestible
    {
        public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
            return false;
        }
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            CompBlood compBlood = ingredients[0].TryGetComp<CompBlood>();
            if (compBlood != null)
                BloodBankUtilities.AdministerTransfusion(pawn, compBlood);
            else
                Debug.Error($"Blood Bank - Give blood operation failed (ingredients[0] ({ingredients[0].def.defName}) has no CompBlood)");
        }

        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
            ingredient.Destroy(DestroyMode.Vanish);
        }

        public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part) { return recipe.label; }
    }
}
