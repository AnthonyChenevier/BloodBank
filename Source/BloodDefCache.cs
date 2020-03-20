// BloodDefCache.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/13 5:05 PM


using System.Collections.Generic;
using System.Linq;
using BloodBank.ModSettingsData;
using RimWorld;
using Verse;

namespace BloodBank
{
    public static class BloodDefCache {
        private static readonly Dictionary<ThingDef, ThingDef> DefCache = new Dictionary<ThingDef, ThingDef>();

        public static IEnumerable<ThingDef> BloodProducts => BloodBankMod.Instance.Content.AllDefs.OfType<ThingDef>().Where(t=> t.GetCompProperties<CompProperties_BloodProduct>() != null);

        /// <summary>
        /// Get the blood definition for this pawn
        /// </summary>
        /// <returns>the pawn's associated blood def</returns>
        public static ThingDef GetBloodDefFor(ThingDef pawn)
        {
            if (DefCache.ContainsKey(pawn))
                return DefCache[pawn];

            //not in the cache. Find it, add it to the cache and return it
            string bloodDefName = $"Blood_{(pawn.race.useMeatFrom != null ? pawn.race.useMeatFrom.defName : pawn.defName)}";
            ThingDef bloodDef = DefDatabase<ThingDef>.GetNamed(bloodDefName);

            if (bloodDef == null)
                return null;

            DefCache.Add(pawn, bloodDef);
            return bloodDef;
        }

        public static void ApplySettings(BloodDefDataBlock bloodDefData)
        {
            //some entries link to the same blood def. Use this list to filter them out
            List<ThingDef> processedDefs = new List<ThingDef>();
            foreach (ThingDef def in DefCache.Values.Where(def => !processedDefs.Contains(def)))
            {
                //Debug.Log($"Applying properties to blood def ({def.defName}) from settings");
                ThingDef sourceDef = def.ingestible.sourceDef;

                CompProperties_Blood compPropertiesBlood = def.GetCompProperties<CompProperties_Blood>();
                compPropertiesBlood.bloodAmount = bloodDefData.Amount;
                compPropertiesBlood.harvestEfficiencyFactor = bloodDefData.HarvestEfficiencyFactor;
                compPropertiesBlood.minSeverityForBadThought = bloodDefData.BloodLossReqdForBadThought;

                def.GetCompProperties<CompProperties_Rottable>().daysToRotStart = bloodDefData.DaysToRot;

                SkillRequirement medicalSkill = def.GetCompProperties<CompProperties_RestrictUsableWithSkill>()? //only humanlike blood will have this
                                                   .skillRequirements
                                                   .FirstOrDefault(s => s.skill == SkillDefOf.Medicine);
                if (medicalSkill != null)
                    medicalSkill.minLevel = bloodDefData.SkillRequirement;

                def.stackLimit = bloodDefData.StackLimit;

                def.SetStatBaseValue(StatDefOf.MaxHitPoints, bloodDefData.HitPoints);
                def.SetStatBaseValue(StatDefOf.DeteriorationRate, bloodDefData.DeteriorationRate);
                def.SetStatBaseValue(StatDefOf.Nutrition, bloodDefData.Nutrition);
                def.SetStatBaseValue(StatDefOf.FoodPoisonChanceFixedHuman, bloodDefData.FoodPoisonChanceFixedHuman);

                def.BaseMarketValue = sourceDef.race.Humanlike
                                          ? sourceDef.race.meatMarketValue * bloodDefData.HumanBloodCostMeatMultiplier
                                          : sourceDef.race.meatMarketValue / bloodDefData.AnimalBloodCostMeatMultiplier;
                processedDefs.Add(def);
            }
        }
    }
}
