using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace BloodBank
{
    class HarmonyPatches
	{
        class BloodBankHarmonyInjector: Mod
        {
            public BloodBankHarmonyInjector(ModContentPack content) : base(content)
            {
                var harmony = HarmonyInstance.Create("com.github.makeitso.rimworld.mod.bloodbank");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
        }

        [HarmonyPatch(typeof(DefGenerator))]
        [HarmonyPatch("GenerateImpliedDefs_PreResolve")]
        class DefGenerator_GenerateImpliedDefs_PreResolve_HP
        {
            static void Postfix()
            {
                foreach (ThingDef def in ThingDefGenerator_Blood.ImpliedBloodDefs())
                    DefGenerator.AddImpliedDef(def);
                foreach (RecipeDef op in BloodRecipeDefGenerator.ImpliedOperationDefs())
                    DefGenerator.AddImpliedDef(op);
                DirectXmlCrossRefLoader.ResolveAllWantedCrossReferences(FailMode.Silent);
            }
        }

        [HarmonyPatch(typeof(FoodUtility))]
        [HarmonyPatch("IsHumanlikeMeat")]
        class FoodUtility_IsHumanlikeMeat_HP
        {
            static void Postfix(ThingDef def, ref bool __result)
            {
                //meat and blood are indistinguishable to the original method. Use my new comp to flag as not meat
                __result = __result && !def.HasComp(typeof(BloodPackComp));
            }
        }


        [HarmonyPatch(typeof(FoodUtility))]
        [HarmonyPatch("ThoughtsFromIngesting")]
        class FoodUtility_ThoughtsFromIngesting_HP
        {
            static void Postfix(Pawn ingester, Thing foodSource, ThingDef foodDef, ref List<ThoughtDef> __result)
            {
                CompIngredients comp = foodSource.TryGetComp<CompIngredients>();
                if (IsHumanlikeBlood(foodDef) && ingester.RaceProps.Humanlike)
                {
                    if (ingester.story.traits.HasTrait(TraitDefOf.Bloodlust))
                        __result.Add(ThoughtDef.Named("DrankHumanlikeBloodBloodlust"));
                    else if (ingester.story.traits.HasTrait(TraitDefOf.Cannibal))
                        __result.Add(ThoughtDef.Named("DrankHumanlikeBloodCannibal"));
                    else
                        __result.Add(ThoughtDef.Named("DrankHumanlikeBlood"));
                }
                else if (comp != null)
                {
                    foreach (ThingDef ingredient in comp.ingredients)
                    {
                        if (ingredient.ingestible == null)
                            return;
                        if (ingester.RaceProps.Humanlike && IsHumanlikeBlood(ingredient))
                        {
                            if (ingester.story.traits.HasTrait(TraitDefOf.Cannibal))
                                __result.Add(ThoughtDef.Named("ConsumedHumanlikeBloodAsIngredientCannibal"));
                            else
                                __result.Add(ThoughtDef.Named("ConsumedHumanlikeBloodAsIngredient"));
                        }
                    }
                }
            }

            private static bool IsHumanlikeBlood(ThingDef foodDef)
            {
                return foodDef.ingestible.sourceDef?.race != null && foodDef.ingestible.sourceDef.race.Humanlike && foodDef.HasComp(typeof(BloodPackComp));
            }
        }
        //from alien races framework github. TO BE INTERGRATED AT A LATER DATE
        //public static void ThoughtsFromIngestingPostfix(Pawn ingester, Thing foodSource, ref List<ThoughtDef> __result)
        //{
            //if (ingester.story.traits.HasTrait(tDef: AlienDefOf.Xenophobia) && ingester.story.traits.DegreeOfTrait(tDef: AlienDefOf.Xenophobia) == 1)
            //    if (__result.Contains(item: ThoughtDefOf.AteHumanlikeMeatDirect) && foodSource.def.ingestible.sourceDef != ingester.def)
            //        __result.Remove(item: ThoughtDefOf.AteHumanlikeMeatDirect);
            //    else if (__result.Contains(item: ThoughtDefOf.AteHumanlikeMeatAsIngredient) &&
            //             (foodSource.TryGetComp<CompIngredients>()?.ingredients.Any(predicate: td => FoodUtility.IsHumanlikeMeat(def: td) && td.ingestible.sourceDef != ingester.def) ?? false))
            //        __result.Remove(item: ThoughtDefOf.AteHumanlikeMeatAsIngredient);

            //if (!(ingester.def is ThingDef_AlienRace alienProps)) return;

            //bool cannibal = ingester.story.traits.HasTrait(tDef: TraitDefOf.Cannibal);

            //for (int i = 0; i < __result.Count; i++)
            //{
            //    ThoughtDef thoughtDef = __result[index: i];
            //    ThoughtSettings settings = alienProps.alienRace.thoughtSettings;

            //    thoughtDef = settings.ReplaceIfApplicable(def: thoughtDef);

            //    if (thoughtDef == ThoughtDefOf.AteHumanlikeMeatDirect || thoughtDef == ThoughtDefOf.AteHumanlikeMeatDirectCannibal)
            //        thoughtDef = settings.GetAteThought(race: foodSource.def.ingestible.sourceDef, cannibal: cannibal, ingredient: false);

            //    if (thoughtDef == ThoughtDefOf.AteHumanlikeMeatAsIngredient || thoughtDef == ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal)
            //    {
            //        ThingDef race = foodSource.TryGetComp<CompIngredients>()?.ingredients.FirstOrDefault(td => td.ingestible?.sourceDef?.race?.Humanlike ?? false);
            //        if (race != null)
            //            thoughtDef = settings.GetAteThought(race: race, cannibal: cannibal, ingredient: true);
            //    }

            //    __result[index: i] = thoughtDef;
            //}
        //}
    }
}
