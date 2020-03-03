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

        public static HediffDef ImmuneBoostHediffDef => _immuneBoostHediffDef ?? (_immuneBoostHediffDef = HediffDef.Named("ImmuneBooster_Hediff"));
        public static HediffDef HealBoostHediffDef => _healBoostHediffDef ?? (_healBoostHediffDef = HediffDef.Named("HealBooster_Hediff"));
        public static HediffDef PerformanceBoostHediffDef => _performanceBoostHediffDef ?? (_performanceBoostHediffDef = HediffDef.Named("PerformanceBooster_Hediff"));
        public static ThoughtDef GiveBloodPositiveThoughtDef => _giveBloodGoodThoughtDef ?? (_giveBloodGoodThoughtDef = DefDatabase<ThoughtDef>.GetNamed("donatedBloodGood"));
        public static ThoughtDef GiveBloodNegativeThoughtDef => _giveBloodBadThoughtDef ?? (_giveBloodBadThoughtDef = DefDatabase<ThoughtDef>.GetNamed("donatedBloodBad"));
        public static ThoughtDef StealBloodThoughDef => _stealBloodThoughDef ?? (_stealBloodThoughDef = DefDatabase<ThoughtDef>.GetNamed("stoleBlood"));
        public static ThoughtDef KilledGuestThought => _killedGuestThought ?? (_killedGuestThought = DefDatabase<ThoughtDef>.GetNamed("killedGuestForBlood"));
        public static ThoughtDef KilledColonistThought => _killedColonistThought ?? (_killedColonistThought = DefDatabase<ThoughtDef>.GetNamed("killedColonistForBlood"));

        public static void ApplyBloodPack(Pawn pawn, ThingDef bloodPack)
        {
            Hediff bloodLossHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (bloodLossHediff != null)
            {
                bloodLossHediff.Severity -= bloodPack.GetCompProperties<CompProperties_Blood>().bloodAmount;
                pawn.health.Notify_HediffChanged(bloodLossHediff);
            }
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

        public static void GiveThoughtsForTakeBlood(Pawn donor, bool isViolation, bool isBad)
        {
            if (donor.NonHumanlikeOrWildMan())
                return;

            if (isViolation)
            {

                donor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(GiveBloodNegativeThoughtDef, 1));

                foreach (Pawn colonistsAndPrisoner in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners)
                {
                    colonistsAndPrisoner.needs.mood.thoughts.memories.TryGainMemory(StealBloodThoughDef);
                }
            }
            else if (isBad)
            {
                donor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(GiveBloodNegativeThoughtDef, 0));
            }
            else
                donor.needs.mood.thoughts.memories.TryGainMemory(GiveBloodPositiveThoughtDef);
        }

        public static void GiveThoughtsForKilledForBlood(Pawn donor)
        {
            if (donor.NonHumanlikeOrWildMan())
                return;

            foreach (Pawn colonistsAndPrisoner in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners)
            {
                colonistsAndPrisoner.needs.mood.thoughts.memories.TryGainMemory(donor.IsColonist ? KilledColonistThought : KilledGuestThought);
            }
        }

        public static ThingDef GetBloodDefForPawn(ThingDef pawn)
        {
            return DefDatabase<ThingDef>.GetNamed("Blood_" + (pawn.race.useMeatFrom != null
                                          ? pawn.race.useMeatFrom.defName
                                          : pawn.defName));
        }
    }
}
