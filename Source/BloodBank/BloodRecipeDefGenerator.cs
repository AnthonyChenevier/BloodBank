using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace BloodBank
{
    public static class BloodRecipeDefGenerator
    {
        public static IEnumerable<RecipeDef> ImpliedOperationDefs()
        {
            foreach (ThingDef sourceDef in DefDatabase<ThingDef>.AllDefs.ToList())
            {
                if (sourceDef.category == ThingCategory.Pawn)
                {
                    if (sourceDef.race.useMeatFrom == null)
                    {
                        if (sourceDef.race.IsFlesh)
                        {
                            RecipeDef takeBaseDef = DefDatabase<RecipeDef>.GetNamed("TakeBlood");
                            RecipeDef giveBaseDef = DefDatabase<RecipeDef>.GetNamed("GiveBlood");

                            yield return GenerateRacialSurgery(sourceDef, takeBaseDef);
                            yield return GenerateRacialSurgery(sourceDef, giveBaseDef);
                        }
                    }
                }
            }
        }

        private static RecipeDef GenerateRacialSurgery(ThingDef pawn, RecipeDef original)
        {
            RecipeDef newDef = new RecipeDef();
            newDef.label = original.label;
            newDef.description = original.description;
            newDef.workerClass = original.workerClass;
            newDef.jobString = original.jobString;
            newDef.anesthetize = original.anesthetize;
            newDef.workAmount = original.workAmount;
            newDef.isViolation = original.isViolation;
            newDef.targetsBodyPart = original.targetsBodyPart;

            newDef.skillRequirements = original.skillRequirements;

            newDef.surgerySuccessChanceFactor = original.surgerySuccessChanceFactor;

            newDef.defName = original.defName + "_" + pawn.defName;
            newDef.modContentPack = original.modContentPack; //does this matter?

            //set up ingredients and products
            ThingDef bloodDef = BloodPackUtilities.GetBloodDefForPawn(pawn);

            if (original.defName == "TakeBlood")
            {
                newDef.products = new List<ThingDefCountClass>
                {
                    new ThingDefCountClass(bloodDef, 1)
                };
            }
            else if (original.defName == "GiveBlood")
            {
                IngredientCount ingredientCount = new IngredientCount();
                ingredientCount.filter.SetAllow(bloodDef, true);
                ingredientCount.SetBaseCount(1);

                newDef.ingredients.Add(ingredientCount);

                newDef.fixedIngredientFilter.SetAllow(bloodDef, true);
            }

            pawn.recipes.Add(newDef);

            return newDef;
        }
    }
}
