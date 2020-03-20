// HarmonyPatches.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/07 11:27 PM


using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BloodBank
{
    [HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve")]
    internal class DefGenerator_Patcher
    {
        [HarmonyPostfix]
        private static void GenerateImpliedDefs_PreResolve_Postfix()
        {
            foreach (ThingDef def in ThingDefGenerator_Blood.ImpliedBloodDefs())
                DefGenerator.AddImpliedDef(def);
            DirectXmlCrossRefLoader.ResolveAllWantedCrossReferences(FailMode.Silent);
            foreach (RecipeDef op in RecipeDefGenerator_BloodSurgeries.ImpliedOperationDefs())
                DefGenerator.AddImpliedDef(op);

        }
    }


    [HarmonyPatch(typeof(Pawn))]
    internal class Pawn_Patcher
    {
        //NOTE: I hate this patch, but I can't think of a better place to influence
        //natural healing factor after looking into Pawn_HeathTracker.HeathTick()
        //ADDENDUM: On second thought, there could be worse places for this. just don't
        //go crazy on the overhead because it's called by everything with a pulse
        [HarmonyPostfix]
        [HarmonyPatch("HealthScale", MethodType.Getter)]
        private static void HealthScale_Postfix(Pawn __instance, ref float __result)
        {
            //check for existing healbooster hediff and add effect if it exists
            __result *= BloodBankUtilities.GetHealingFactor(__instance);
        }
    }


    [HarmonyPatch(typeof(FoodUtility), "IsHumanlikeMeat")]
    internal class FoodUtility_Patcher
    {
        [HarmonyPostfix]
        private static void IsHumanlikeMeat_Postfix(ThingDef def, ref bool __result)
        {
            //meat and blood are indistinguishable to the original method. Use CompBlood to flag as not meat
            bool newResult = __result && !def.HasComp(typeof(CompBlood));
            //Debug.Log($"{def.defName}.IsHumanlikeMeat postfix. Original result: {__result}. New result: {newResult}");
            __result = newResult;
        }
    }

    //these next 2 are conditional based on the existence of the Alien Race Framework
    //this one supports Xenophobic aliens. Hooray...?
    [HarmonyPatch]
    [HarmonyAfter("rimworld.erdelf.alien_race.main")]
    internal class FoodUtility_ARF_Patcher
    {
        [HarmonyPrepare]
        private static bool Prepare()
        {
            bool frameworkExists = BloodBankMod.Instance.AlienFrameworkExists;
            if (frameworkExists) Debug.Log("Running ARF specific FoodUtility.ThoughtsFromIngesting patch");
            return frameworkExists;
        }

        private static MethodBase TargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(FoodUtility), nameof(FoodUtility.ThoughtsFromIngesting));
        }

        [HarmonyPostfix]
        private static void ThoughtsFromIngesting_Postfix(Pawn ingester, Thing foodSource, ThingDef foodDef, ref List<ThoughtDef> __result)
        {
            if (!ingester.RaceProps.Humanlike)
                return;

            //Debug.Log("Processing blood thoughts for humanlikes");
            ////direct consumption
            if (foodDef.IsHumanlikeBlood()) 
            {
                //bloodlust beats xenophobia
                if (ingester.story.traits.HasTrait(TraitDefOf.Bloodlust))
                    __result.Add(BloodThoughtDefOf.ConsumedHumanlikeBloodDirectBloodlust);
                else if (ingester.def == foodDef.ingestible.sourceDef || !ingester.IsXenophobic()) //alien-specific food check
                    __result.Add(ingester.story.traits.HasTrait(TraitDefOf.Cannibal)
                                     ? BloodThoughtDefOf.ConsumedHumanlikeBloodDirectCannibal
                                     : BloodThoughtDefOf.ConsumedHumanlikeBloodDirect);
                return;
            }

            //else it's consumption as ingredient
            CompIngredients ingredientComp = foodSource.TryGetComp<CompIngredients>();
            if (ingredientComp == null)
                return;

            __result.AddRange(from ingredient in ingredientComp.ingredients
                              where ingredient.ingestible != null && ingredient.IsHumanlikeBlood() &&
                                    (ingester.def == ingredient.ingestible.sourceDef || !ingester.IsXenophobic()) //and this
                              select ingester.story.traits.HasTrait(TraitDefOf.Cannibal)
                                         ? BloodThoughtDefOf.ConsumedHumanlikeBloodAsIngredientCannibal
                                         : BloodThoughtDefOf.ConsumedHumanlikeBloodAsIngredient);

        }

    }

    //this one leaves out the check. Not much of an optimization, but I learned things!
    [HarmonyPatch]
    internal class FoodUtility_Vanilla_Patcher
    {
        [HarmonyPrepare]
        private static bool Prepare()
        {
            bool frameworkExists = BloodBankMod.Instance.AlienFrameworkExists;
            if (!frameworkExists) Debug.Log("Running vanilla FoodUtility.ThoughtsFromIngesting patch");
            return !frameworkExists;
        }

        private static MethodBase TargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(FoodUtility), nameof(FoodUtility.ThoughtsFromIngesting));
        }

        [HarmonyPostfix]
        private static void ThoughtsFromIngesting_Postfix(Pawn ingester, Thing foodSource, ThingDef foodDef, ref List<ThoughtDef> __result)
        {
            if (!ingester.RaceProps.Humanlike)
                return;

            //Debug.Log("Processing blood thoughts for humanlikes");
            //direct consumption
            if (foodDef.IsHumanlikeBlood()) //much saving, so optimize. wow.🐶
            {
                if (ingester.story.traits.HasTrait(TraitDefOf.Bloodlust))
                    __result.Add(BloodThoughtDefOf.ConsumedHumanlikeBloodDirectBloodlust);
                else if (ingester.story.traits.HasTrait(TraitDefOf.Cannibal))
                    __result.Add(BloodThoughtDefOf.ConsumedHumanlikeBloodAsIngredientCannibal);
                else
                    __result.Add(BloodThoughtDefOf.ConsumedHumanlikeBloodDirect);
                return;
            }

            //consumption as ingredient
            CompIngredients ingredientComp = foodSource.TryGetComp<CompIngredients>();
            if (ingredientComp == null)
                return;

            __result.AddRange(from ingredient in ingredientComp.ingredients
                              where ingredient.ingestible != null &&
                                    ingredient.IsHumanlikeBlood() //The mad bastard did it again! How can we deal with such optimized code?!
                              select ingester.story.traits.HasTrait(TraitDefOf.Cannibal)
                                             ? BloodThoughtDefOf.ConsumedHumanlikeBloodAsIngredientCannibal
                                             : BloodThoughtDefOf.ConsumedHumanlikeBloodAsIngredient);
        }
    }
}
