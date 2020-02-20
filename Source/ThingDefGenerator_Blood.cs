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
                if (sourceDef.category == ThingCategory.Pawn)
                {
                    if (sourceDef.race.useMeatFrom == null)
                    {
                        if (sourceDef.race.IsFlesh)
                        {
                            ThingDef bloodDef = new ThingDef();
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
                                    daysToRotStart = 0.5f, rotDestroys = true
                            });
                            bloodDef.comps.Add(new CompProperties_BloodPack
                            {
                                    bloodAmount = 0.1f,
                                    minSeverityForGive = 0.1f,
                                    minSeverityForBadThought = 0.4f,
                                    harvestEfficiencyFactor = 2f
                            });
                            //humanlike blood can be self-administered (if same race)
                            if (sourceDef.race.Humanlike)
                            {
                                List<CompProperties> comps = new List<CompProperties>
                                {
                                        new CompProperties_Usable
                                        {
                                                useJob = DefDatabase<JobDef>.GetNamed("TransfuseBloodSelf"),
                                                useLabel = "GiveSelfTransfusion".Translate()
                                        },
                                        new CompProperties_UseEffect
                                        {
                                                compClass = typeof(CompUseEffect_GiveBlood)
                                        },
                                        new CompProperties_UseEffect
                                        {
                                                compClass = typeof(CompUseEffect_DestroySelf)
                                        }
                                };

                                bloodDef.comps.AddRange(comps);
                            }

                            bloodDef.alwaysHaulable = true;
                            bloodDef.drawGUIOverlay = true;
                            bloodDef.rotatable = false;
                            bloodDef.pathCost = 15;

                            if (bloodDef.thingCategories == null)
                            {
                                bloodDef.thingCategories = new List<ThingCategoryDef>();
                            }

                            DirectXmlCrossRefLoader.RegisterListWantsCrossRef(bloodDef.thingCategories, "RawBlood",
                                                                              bloodDef);

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
                                    texPath = "Things/Item/Resource/BloodPack",
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
                                    specialThoughtDirect =
                                            sourceDef.race.Humanlike
                                                    ? ThoughtDef.Named("DrankHumanlikeBlood")
                                                    : null,
                                    specialThoughtAsIngredient =
                                            sourceDef.race.Humanlike
                                                    ? ThoughtDef.Named("ConsumedHumanlikeBloodAsIngredient")
                                                    : null
                            };

                            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(bloodDef.ingestible, "tasteThought",
                                                                                "DrankRawBlood");


                            bloodDef.defName = "Blood_" + sourceDef.defName;
                            bloodDef.label = "BloodLabel".Translate(sourceDef.race.meatLabel.NullOrEmpty() ? sourceDef.label : sourceDef.race.meatLabel.Replace("meat", "").Trim());
                            yield return bloodDef;
                        }
                    }
                }
            }
        }
    }
}
