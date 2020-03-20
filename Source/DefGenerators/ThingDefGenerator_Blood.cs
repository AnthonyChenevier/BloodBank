// ThingDefGenerator_Blood.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/02/06 3:34 PM


using System.Collections.Generic;
using System.Linq;
using BloodBank.ModSettings;
using RimWorld;
using UnityEngine;
using Verse;

namespace BloodBank {
    public static class ThingDefGenerator_Blood {
        public static IEnumerable<ThingDef> ImpliedBloodDefs()
        {
            BloodDefDataBlock dataBlock = BloodBankMod.Instance.Settings.GetBloodDefProperties();
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

                bloodDef.stackLimit = dataBlock.StackLimit;

                bloodDef.SetStatBaseValue(StatDefOf.Beauty, -4f);
                bloodDef.SetStatBaseValue(StatDefOf.Mass, 0.5f);
                bloodDef.SetStatBaseValue(StatDefOf.Flammability, 2f);
                bloodDef.SetStatBaseValue(StatDefOf.MaxHitPoints, dataBlock.HitPoints);
                bloodDef.SetStatBaseValue(StatDefOf.DeteriorationRate, dataBlock.DeteriorationRate);
                bloodDef.SetStatBaseValue(StatDefOf.Nutrition, dataBlock.Nutrition);
                bloodDef.SetStatBaseValue(StatDefOf.FoodPoisonChanceFixedHuman, dataBlock.FoodPoisonChanceFixedHuman);

                bloodDef.tickerType = TickerType.Rare;
                bloodDef.comps.AddRange(GetComps(sourceDef, dataBlock));

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
                bloodDef.modContentPack = BloodBankMod.Instance.Content;

                //worth 4x as much as the meat of the same animal if humanlike, half as much for critters
                bloodDef.BaseMarketValue = sourceDef.race.Humanlike
                                                   ? sourceDef.race.meatMarketValue * dataBlock.HumanBloodCostMeatMultiplier
                                                   : sourceDef.race.meatMarketValue / dataBlock.AnimalBloodCostMeatMultiplier;

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


                IngestibleProperties ingestible = new IngestibleProperties
                {
                    parent = bloodDef,
                    foodType = FoodTypeFlags.Fluid | FoodTypeFlags.AnimalProduct,
                    preferability = FoodPreferability.DesperateOnlyForHumanlikes,
                    ingestCommandString = "Drink {0}",
                    ingestReportString = "Drinking {0}.",
                    ingestEffect = EffecterDefOf.EatMeat,
                    ingestSound = SoundDef.Named("Ingest_Drink"),
                    sourceDef = sourceDef,
                    specialThoughtDirect = sourceDef.race.FleshType == FleshTypeDefOf.Insectoid
                                                   ? BloodThoughtDefOf.ConsumedInsectHemolymphDirect : null,
                    specialThoughtAsIngredient = sourceDef.race.FleshType == FleshTypeDefOf.Insectoid
                                                         ? BloodThoughtDefOf.ConsumedInsectHemolymphDirectAsIngredient : null,
                };
                bloodDef.ingestible = ingestible;

                DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(ingestible, "tasteThought", "DrankRawBlood");

                yield return bloodDef;
            }
        }

        private static IEnumerable<CompProperties> GetComps(ThingDef sourceDef, BloodDefDataBlock dataBlock)
        {
            yield return new CompProperties_Forbiddable();

            yield return new CompProperties_Rottable
            {
                daysToRotStart = dataBlock.DaysToRot,
                rotDestroys = true
            };

            yield return new CompProperties_Blood
            {
                bloodAmount = dataBlock.Amount,
                minSeverityForBadThought = dataBlock.BloodLossReqdForBadThought,
                harvestEfficiencyFactor = dataBlock.HarvestEfficiencyFactor
            };

            if (!sourceDef.race.Humanlike)
                yield break;

            //humanlike blood can be self-administered (if same race)
            yield return new CompProperties_Usable
            {
                    useJob = DefDatabase<JobDef>.GetNamed("UseItem"),
                    useLabel = "GiveSelfTransfusion".Translate(GetBloodLabel(sourceDef))
            };

            yield return new CompProperties_RestrictUsableWithSkill
            {
                skillRequirements = new List<SkillRequirement>
                {
                    new SkillRequirement
                    {
                        skill = SkillDefOf.Medicine, 
                        minLevel = dataBlock.SkillRequirement
                    }
                }
            };

            yield return new CompProperties_RestrictUsableByRace { allowedRaces = new List<ThingDef> { sourceDef } };

            yield return new CompProperties_UseEffect { compClass = typeof(CompUseEffect_AdministerBloodTransfusion) };

            yield return new CompProperties_UseEffect { compClass = typeof(CompUseEffect_DestroySelf) };
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
            //no meat label. just use the source def label
            if (meatLabel.NullOrEmpty())
                return "BloodLabel".Translate(sourceDef.label);

            //check for collective meat type (2b). These should be of the form '<race> meat'
            //(contains a space) - Yaks are a known exception with 'yak beef', but this should
            //handle that and similar situations as long as the first part is still the race name.
            //cut out the part before the space (if there is one), hopefully that's the <race> part. 
            //otherwise, meat label is for common meat name - just use sorceDef label as above
            int indexOfSpace = meatLabel.IndexOf(' ');
            return "BloodLabel".Translate(indexOfSpace >= 0 ? meatLabel.Substring(0, indexOfSpace) : sourceDef.label);
        }
    }
}
