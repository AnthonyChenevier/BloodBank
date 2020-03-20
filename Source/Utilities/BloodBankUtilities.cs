using RimWorld;
using Verse;

namespace BloodBank
{
    public static class BloodBankUtilities {
        
        private static HediffDef _healBoostHediffDef;
        private static HediffDef _immuneBoostHediffDef;
        private static HediffDef _performanceBoostHediffDef;

        public static HediffDef ImmuneBoostHediffDef =>
            _immuneBoostHediffDef ?? (_immuneBoostHediffDef = HediffDef.Named("ImmuneBooster_Hediff"));

        public static HediffDef HealBoostHediffDef =>
            _healBoostHediffDef ?? (_healBoostHediffDef = HediffDef.Named("HealBooster_Hediff"));

        public static HediffDef PerformanceBoostHediffDef =>
            _performanceBoostHediffDef ?? (_performanceBoostHediffDef = HediffDef.Named("PerformanceBooster_Hediff"));


        public static void AdministerTransfusion(Pawn pawn, CompBlood bloodPack)
        {
            Hediff bloodLossHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);

            if (bloodLossHediff == null || bloodPack == null)
                return;

            bloodLossHediff.Severity -= bloodPack.Props.bloodAmount;
            pawn.health.Notify_HediffChanged(bloodLossHediff);
        }


        public static void AdministerBooster(Pawn pawn, CompBloodProduct bloodProduct)
        {
            if (bloodProduct == null)
                return;

            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(bloodProduct.Props.hediffDef) ??
                            pawn.health.AddHediff(bloodProduct.Props.hediffDef);
            hediff.Severity = bloodProduct.Props.severity;
            pawn.health.Notify_HediffChanged(hediff);
        }


        public static bool MakeBloodPack(Pawn pawn, ThingDef bloodPack)
        {
            CompProperties_Blood bloodPackCompProps = bloodPack.GetCompProperties<CompProperties_Blood>();
            
            float bloodToTake = bloodPackCompProps.bloodAmount * bloodPackCompProps.harvestEfficiencyFactor;

            Hediff hediff;
            if (pawn.health.hediffSet.HasHediff(HediffDefOf.BloodLoss))
            {
                hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
                hediff.Severity += bloodToTake;
            }
            else
            {
                hediff = pawn.health.AddHediff(HediffDefOf.BloodLoss);
                hediff.Severity = bloodToTake;
            }
            pawn.health.Notify_HediffChanged(hediff);

            return hediff.Severity >= bloodPackCompProps.minSeverityForBadThought;
        }


        public static void GiveThoughtsForTakeBlood(Pawn donor, bool isViolation, bool tookWhenLow)
        {
            if (donor.NonHumanlikeOrWildMan())
                return;

            if (!isViolation)
            {
                if (tookWhenLow)
                    donor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BloodThoughtDefOf.GiveBloodNegative, 0));
                else donor.needs.mood.thoughts.memories.TryGainMemory(BloodThoughtDefOf.GiveBloodPositive);
                return;
            }

            //is a violation
            donor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(BloodThoughtDefOf.GiveBloodNegative, 1));
            PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners.ForEach(cap => {
                cap.needs.mood.thoughts.memories.TryGainMemory(BloodThoughtDefOf.KnowStoleBlood);
            });
        }


        public static void GiveThoughtsForKilledForBlood(Pawn donor)
        {
            if (donor.NonHumanlikeOrWildMan())
                return;

            PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners.ForEach(cap => {
                cap.needs.mood.thoughts.memories.TryGainMemory(donor.IsColonist ? BloodThoughtDefOf.KilledColonistForBlood : BloodThoughtDefOf.KilledGuestForBlood);
            });
        }


        /// <summary>
        /// Non-Alien Race Framework dependent Xenophobia check
        /// </summary>
        /// <returns>true if the pawn is Xenophobic</returns>
        public static bool IsXenophobic(this Pawn pawn)
        {
            TraitDef xenophobiaDef = DefDatabase<TraitDef>.GetNamed("Xenophobia");
            return xenophobiaDef != null && pawn.story.traits.HasTrait(xenophobiaDef) && pawn.story.traits.DegreeOfTrait(xenophobiaDef) == 1;
        }

        //cached defs

        /// <summary>
        /// Check if this thing def is humanlike blood
        /// </summary>
        /// <returns>true if this thing is humanlike blood</returns>
        public static bool IsHumanlikeBlood(this ThingDef foodDef)
        {
            bool isHumanlikeBlood = foodDef.ingestible.sourceDef?.race != null &&
                                    foodDef.ingestible.sourceDef.race.Humanlike &&
                                    foodDef.HasComp(typeof(CompBlood));

            //Debug.Log($"{foodDef.defName}.IsHumanlikeBlood called. Result: {isHumanlikeBlood}.");
            return isHumanlikeBlood;
        }

        /// <summary>
        /// Check if this race allows a particular surgery
        /// </summary>
        /// <param name="recipeDef">the recipe to check</param>
        /// <returns>true if the race allows the surgery</returns>
        public static bool RaceAllowsBloodSurgery(this ThingDef alienDef, RecipeDef recipeDef)
        {
            ThingDef ingredientDef = recipeDef.fixedIngredientFilter?.AnyAllowedDef;
            ThingDef productDef = recipeDef.ProducedThingDef;
            ThingDef bloodDef = BloodDefCache.GetBloodDefFor(alienDef);

            //null coalescing is schweet. Screw you spaghetti code
            return ingredientDef == bloodDef || productDef == bloodDef ||
                   (ingredientDef?.GetCompProperties<CompProperties_RestrictUsableByRace>()?.allowedRaces?.Contains(alienDef) ??
                    productDef?.GetCompProperties<CompProperties_RestrictUsableByRace>()?.allowedRaces?.Contains(alienDef) ?? false);
        }

        public static float GetHealingFactor(Pawn pawn)
        {
            return pawn.health.hediffSet.GetFirstHediffOfDef(HealBoostHediffDef)?.TryGetComp<HediffComp_HealFactor>()?.Props.healFactor ?? 1f;
        }
    }
}
