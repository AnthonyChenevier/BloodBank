// ThingDefGenerator_Blood.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/02/06 3:34 PM


using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BloodBank {
    public static class ThingDefGenerator_Blood {
        public static IEnumerable<ThingDef> ImpliedBloodDefs()
        {
            foreach (ThingDef sourceDef in DefDatabase<ThingDef>.AllDefs.ToList())
            {
                if (sourceDef.category != ThingCategory.Pawn || sourceDef.race.useMeatFrom != null || !sourceDef.race.IsFlesh)
                    continue;


                ThingDef bloodDef = new ThingDef();

                bloodDef.defName = "Blood_" + sourceDef.defName;
                bloodDef.label = GetBloodLabel(sourceDef);

                bloodDef.resourceReadoutPriority = ResourceCountPriority.Middle;
                bloodDef.category = ThingCategory.Item;
                bloodDef.thingClass = typeof(ThingWithComps);
                bloodDef.useHitPoints = true;
                bloodDef.selectable = true;
                bloodDef.altitudeLayer = AltitudeLayer.Item;
                bloodDef.stackLimit = 25;

                bloodDef.SetStatBaseValue(StatDefOf.Beauty, -4f);
                bloodDef.SetStatBaseValue(StatDefOf.MaxHitPoints, 60f);
                bloodDef.SetStatBaseValue(StatDefOf.DeteriorationRate, 10f);
                bloodDef.SetStatBaseValue(StatDefOf.Mass, 0.5f);
                bloodDef.SetStatBaseValue(StatDefOf.Flammability, 2f);
                bloodDef.SetStatBaseValue(StatDefOf.Nutrition, 0.01f);
                bloodDef.SetStatBaseValue(StatDefOf.FoodPoisonChanceFixedHuman, 0.02f);

                bloodDef.tickerType = TickerType.Rare;

                bloodDef.comps.Add(new CompProperties_Forbiddable());
                bloodDef.comps.Add(new CompProperties_Rottable
                {
                    daysToRotStart = 0.5f,
                    rotDestroys = true
                });
                bloodDef.comps.Add(new CompProperties_Blood
                {
                    bloodAmount = 0.1f,
                    minSeverityForBadThought = 0.4f,
                    harvestEfficiencyFactor = 2f
                });

                //humanlike blood can be self-administered (if same race)
                if (sourceDef.race.Humanlike)
                {
                    bloodDef.comps.Add(new CompProperties_Usable
                    {
                        useJob = DefDatabase<JobDef>.GetNamed("UseItem"),
                        useLabel = "GiveSelfTransfusion".Translate(bloodDef.label)
                    });
                    bloodDef.comps.Add(new CompProperties_RestrictUsableWithSkill
                    {
                        skillRequirements = new List<SkillRequirement>
                        {
                            new SkillRequirement { skill = SkillDefOf.Medicine, minLevel = 4 }
                        }
                    });
                    bloodDef.comps.Add(new CompProperties_RestrictUsableByRace { allowedRaces = new List<ThingDef> { sourceDef } });
                    bloodDef.comps.Add(new CompProperties_UseEffect { compClass = typeof(CompUseEffect_AdministerBloodTransfusion) });
                    bloodDef.comps.Add(new CompProperties_UseEffect { compClass = typeof(CompUseEffect_DestroySelf) });
                }

                bloodDef.alwaysHaulable = true;
                bloodDef.drawGUIOverlay = true;
                bloodDef.rotatable = false;
                bloodDef.pathCost = 15;

                if (bloodDef.thingCategories == null)
                {
                    bloodDef.thingCategories = new List<ThingCategoryDef>();
                }

                DirectXmlCrossRefLoader.RegisterListWantsCrossRef(bloodDef.thingCategories, "RawBlood", bloodDef);

                bloodDef.uiIconForStackCount = 1;
                bloodDef.soundInteract = SoundDefOf.Standard_Drop;
                bloodDef.soundDrop = SoundDefOf.Standard_Drop;

                bloodDef.techLevel = TechLevel.Industrial;

                bloodDef.socialPropernessMatters = true;
                bloodDef.modContentPack = sourceDef.modContentPack; //does this matter?

                //worth 4x as much as the meat of the same animal if humanlike, half as much for critters
                bloodDef.BaseMarketValue = sourceDef.race.Humanlike
                                                   ? sourceDef.race.meatMarketValue * 4f
                                                   : sourceDef.race.meatMarketValue / 2f;

                bloodDef.graphicData = new GraphicData
                {
                    graphicClass = typeof(Graphic_StackCount),
                    texPath = sourceDef.race.FleshType == FleshTypeDefOf.Insectoid
                                      ? "Things/Item/Resource/InsectBloodPack"
                                      : "Things/Item/Resource/BloodPack",
                    drawSize = Vector2.one * 0.85f
                };

                if (sourceDef.race.Humanlike)
                    bloodDef.description = "BloodHumanlikeDesc".Translate(sourceDef.label);
                else if (sourceDef.race.FleshType == FleshTypeDefOf.Insectoid)
                    bloodDef.description = "BloodInsectDesc".Translate(sourceDef.label);
                else
                    bloodDef.description = "BloodDesc".Translate(sourceDef.label);


                bloodDef.ingestible = new IngestibleProperties
                {
                    parent = bloodDef,
                    foodType = FoodTypeFlags.Fluid | FoodTypeFlags.AnimalProduct,
                    preferability = FoodPreferability.DesperateOnlyForHumanlikes,
                    ingestCommandString = "Drink {0}",
                    ingestReportString = "Drinking {0}.",
                    ingestEffect = EffecterDefOf.EatMeat,
                    ingestSound = SoundDef.Named("Ingest_Drink"),
                    sourceDef = sourceDef,
                    specialThoughtDirect = sourceDef.race.Humanlike
                                                ? ThoughtDef.Named("DrankHumanlikeBlood")
                                                : sourceDef.race.FleshType == FleshTypeDefOf.Insectoid
                                                        ? ThoughtDef.Named("ConsumedInsectHemolymph")
                                                        : null,
                    specialThoughtAsIngredient = sourceDef.race.Humanlike
                                                ? ThoughtDef.Named("ConsumedHumanlikeBloodAsIngredient")
                                                : sourceDef.race.FleshType == FleshTypeDefOf.Insectoid
                                                        ? ThoughtDef.Named("ConsumedInsectHemolymphAsIngredient")
                                                        : null,
                };

                DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(bloodDef.ingestible, "tasteThought", "DrankRawBlood");

                yield return bloodDef;
            }
        }

        private static string GetBloodLabel(ThingDef sourceDef)
        {
            if (sourceDef.race.FleshType == FleshTypeDefOf.Insectoid)
            {
                return "InsectBloodLabel".Translate();
            }
            //HACKY STUFF INCOMING
            //meat defs are created based on 3 exclusive rules. 
            //1. def.race.useMeatFrom -> just take the meat def from this source
            //2. def.race.meatLabel -> use this label instead of generating one. Used for
            //   two reasons: a. overriding names of common meat types (beef, venison, pork)
            //   and b. generating a collective meat name for base types used in rule 1.
            //3. Generate the name of the meat as 'def.label + " meat"'
            //blood defs need to follow a similar principle, but without using the common meat name overrides.
            //also needs to be translation compatible.

            string meatLabel = sourceDef.race.meatLabel;
            //check for collective meat type (2b). These should be of the form '<race> meat'
            //(contains a space) - Yaks are a known exception with 'yak beef', but this should
            //handle that and similar situations as long as the first part is still the race name.
            if (!meatLabel.NullOrEmpty() && meatLabel.Contains(' '))
            {
                //cut out the part before the space, hopefully that's the <race> part. 
                return "BloodLabel".Translate(meatLabel.Substring(0, meatLabel.IndexOf(' ')));
            }

            //no meat label, or meat label is for common meat name. just use the source def label
            return "BloodLabel".Translate(sourceDef.label);
        }
    }
}
