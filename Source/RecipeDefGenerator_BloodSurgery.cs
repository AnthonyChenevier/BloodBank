using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BloodBank
{
    public static class RecipeDefGenerator_BloodSurgery
    {
        public static IEnumerable<RecipeDef> ImpliedOperationDefs()
        {
            foreach (ThingDef sourceDef in from sourceDef in DefDatabase<ThingDef>.AllDefs
                                           where sourceDef.category == ThingCategory.Pawn
                                           where sourceDef.race.IsFlesh
                                           select sourceDef)
            {
                yield return GenerateRacialSurgery(sourceDef, DefDatabase<RecipeDef>.GetNamed("TakeBlood"));
                yield return GenerateRacialSurgery(sourceDef, DefDatabase<RecipeDef>.GetNamed("GiveBlood"));
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
            newDef.researchPrerequisite = original.researchPrerequisite;
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
            
            //newDef.recipeUsers.Add(pawn);
            pawn.recipes.Add(newDef);

            return newDef;
        }
    }
}
