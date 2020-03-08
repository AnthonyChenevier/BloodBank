using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BloodBank
{
    public static class RecipeDefGenerator_BloodSurgeries
    {
        public static IEnumerable<RecipeDef> ImpliedOperationDefs()
        {
            foreach (ThingDef sourceDef in from def in DefDatabase<ThingDef>.AllDefs
                                           where def.category == ThingCategory.Pawn
                                           where def.race.IsFlesh
                                           select def)
            {
                yield return GenerateRacialSurgery(sourceDef, DefDatabase<RecipeDef>.GetNamed("TakeBlood"));
                yield return GenerateRacialSurgery(sourceDef, DefDatabase<RecipeDef>.GetNamed("GiveBlood"));
            }
        }

        private static RecipeDef GenerateRacialSurgery(ThingDef pawnDef, RecipeDef original)
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
            newDef.dontShowIfAnyIngredientMissing = original.dontShowIfAnyIngredientMissing;
            newDef.defName = original.defName + "_" + pawnDef.defName;
            newDef.modContentPack = original.modContentPack; //does this matter?
            newDef.researchPrerequisite = original.researchPrerequisite;
            //set up ingredients and products
            ThingDef bloodDef = pawnDef.GetBloodDef();

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
                ingredientCount.SetBaseCount(1);
                ingredientCount.filter.SetAllow(bloodDef, true);
                newDef.ingredients.Add(ingredientCount);
                newDef.fixedIngredientFilter.SetAllow(bloodDef, true);
            }
            
            //newDef.recipeUsers.Add(pawn);
            pawnDef.recipes.Add(newDef);

            return newDef;
        }
    }
}
