using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BloodBank
{
    public static class BloodBankUtilities {
        //cached defs
        private static HediffDef _healBoostHediffDef;
        private static HediffDef _immuneBoostHediffDef;
        private static HediffDef _performanceBoostHediffDef;

        private static ThoughtDef _giveBloodGoodThoughtDef;
        private static ThoughtDef _giveBloodBadThoughtDef;
        private static ThoughtDef _stealBloodThoughDef;
        private static ThoughtDef _killedGuestThought;
        private static ThoughtDef _killedColonistThought;

        private static readonly Dictionary<ThingDef, ThingDef> BloodDefsCache = new Dictionary<ThingDef, ThingDef>();

        private static ThoughtDef _consumeBloodDirect;
        private static ThoughtDef _consumeBloodDirectBloodlust;
        private static ThoughtDef _consumeBloodDirectCannibal;

        private static ThoughtDef _consumeBloodIngredient;
        private static ThoughtDef _consumeBloodIngredientBloodlust;
        private static ThoughtDef _consumeBloodIngredientCannibal;

        public static HediffDef ImmuneBoostHediffDef =>
                _immuneBoostHediffDef ?? (_immuneBoostHediffDef = HediffDef.Named("ImmuneBooster_Hediff"));

        public static HediffDef HealBoostHediffDef =>
                _healBoostHediffDef ?? (_healBoostHediffDef = HediffDef.Named("HealBooster_Hediff"));

        public static HediffDef PerformanceBoostHediffDef =>
                _performanceBoostHediffDef ??
                (_performanceBoostHediffDef = HediffDef.Named("PerformanceBooster_Hediff"));

        public static ThoughtDef GiveBloodPositiveThoughtDef =>
                _giveBloodGoodThoughtDef ??
                (_giveBloodGoodThoughtDef = DefDatabase<ThoughtDef>.GetNamed("donatedBloodGood"));

        public static ThoughtDef GiveBloodNegativeThoughtDef =>
                _giveBloodBadThoughtDef ??
                (_giveBloodBadThoughtDef = DefDatabase<ThoughtDef>.GetNamed("donatedBloodBad"));

        public static ThoughtDef StealBloodThoughDef =>
                _stealBloodThoughDef ?? (_stealBloodThoughDef = DefDatabase<ThoughtDef>.GetNamed("stoleBlood"));

        public static ThoughtDef KilledGuestThought =>
                _killedGuestThought ?? (_killedGuestThought = DefDatabase<ThoughtDef>.GetNamed("killedGuestForBlood"));

        public static ThoughtDef KilledColonistThought =>
                _killedColonistThought ??
                (_killedColonistThought = DefDatabase<ThoughtDef>.GetNamed("killedColonistForBlood"));

        public static ThoughtDef ConsumeHumanlikeBloodDirectBloodlustThought =>
                _consumeBloodDirectBloodlust ??
                (_consumeBloodDirectBloodlust = ThoughtDef.Named("DrankHumanlikeBloodBloodlust"));

        public static ThoughtDef ConsumeHumanlikeBloodDirectCannibalThought =>
                _consumeBloodDirectCannibal ??
                (_consumeBloodDirectCannibal = ThoughtDef.Named("DrankHumanlikeBloodCannibal"));

        public static ThoughtDef ConsumeHumanlikeBloodDirectThought =>
                _consumeBloodDirect ?? (_consumeBloodDirect = ThoughtDef.Named("DrankHumanlikeBlood"));

        public static ThoughtDef ConsumeHumanlikeBloodIngredientCannibalThought =>
                _consumeBloodIngredientCannibal ??
                (_consumeBloodIngredientCannibal = ThoughtDef.Named("ConsumedHumanlikeBloodAsIngredientCannibal"));

        public static ThoughtDef ConsumeHumanlikeBloodIngredientThought =>
                _consumeBloodIngredient ??
                (_consumeBloodIngredient = ThoughtDef.Named("ConsumedHumanlikeBloodAsIngredient"));


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
                    donor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(GiveBloodNegativeThoughtDef, 0));
                else donor.needs.mood.thoughts.memories.TryGainMemory(GiveBloodPositiveThoughtDef);
                return;
            }

            //is a violation
            donor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(GiveBloodNegativeThoughtDef, 1));
            PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners.ForEach(cap => {
                cap.needs.mood.thoughts.memories.TryGainMemory(StealBloodThoughDef);
            });
        }


        public static void GiveThoughtsForKilledForBlood(Pawn donor)
        {
            if (donor.NonHumanlikeOrWildMan())
                return;

            PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners.ForEach(cap => {
                cap.needs.mood.thoughts.memories.TryGainMemory(donor.IsColonist ? KilledColonistThought : KilledGuestThought);
            });
        }


        public static bool IsXenophobic(this Pawn pawn)
        {
            TraitDef alienDefOfXenophobia = DefDatabase<TraitDef>.GetNamed("Xenophobia");
            return pawn.story.traits.HasTrait(alienDefOfXenophobia) &&
                   pawn.story.traits.DegreeOfTrait(alienDefOfXenophobia) == 1;
        }


        public static ThingDef GetBloodDef(this ThingDef pawn)
        {
            if (BloodDefsCache.ContainsKey(pawn))
                return BloodDefsCache[pawn];

            //not in the cache. Find it, add it to the cache and return it
            string bloodDefName = $"Blood_{(pawn.race.useMeatFrom != null ? pawn.race.useMeatFrom.defName : pawn.defName)}";
            ThingDef bloodDef = DefDatabase<ThingDef>.GetNamed(bloodDefName);

            if (bloodDef == null)
                return null;

            BloodDefsCache.Add(pawn, bloodDef);
            return bloodDef;
        }


        public static bool IsHumanlikeBlood(this ThingDef foodDef)
        {
            bool isHumanlikeBlood = foodDef.ingestible.sourceDef?.race != null && foodDef.ingestible.sourceDef.race.Humanlike && foodDef.HasComp(typeof(CompBlood));

            //Log.Message($"{foodDef.defName}.IsHumanlikeBlood called. Result: {isHumanlikeBlood}.");
            return isHumanlikeBlood;
        }


        public static bool RaceAllowsSurgery(this ThingDef alienDef, RecipeDef recipeDef)
        {
            ThingDef ingredientDef = recipeDef.fixedIngredientFilter?.AnyAllowedDef;
            ThingDef productDef = recipeDef.ProducedThingDef; 
            ThingDef bloodDef = alienDef.GetBloodDef();

            //null coalescing is schweet. Screw you spaghetti code
            return ingredientDef == bloodDef || productDef == bloodDef ||
                   (ingredientDef?.GetCompProperties<CompProperties_RestrictUsableByRace>()?.allowedRaces?.Contains(alienDef) ??
                    productDef?.GetCompProperties<CompProperties_RestrictUsableByRace>()?.allowedRaces?.Contains(alienDef) ??
                    false);
        }
    }
}
