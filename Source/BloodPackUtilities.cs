using RimWorld;
using Verse;

namespace BloodBank
{
    public static class BloodPackUtilities {
        public static void ApplyBloodPack(Pawn pawn, ThingDef bloodPack)
        {
            Hediff bloodLossHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (bloodLossHediff != null)
            {
                bloodLossHediff.Severity -= bloodPack.GetCompProperties<CompProperties_BloodPack>().bloodAmount;
                pawn.health.Notify_HediffChanged(bloodLossHediff);

                //destroy blood pack
            }
        }

        public static bool MakeBloodPack(Pawn pawn, ThingDef bloodPack)
        {
            CompProperties_BloodPack bloodPackCompProps = bloodPack.GetCompProperties<CompProperties_BloodPack>();


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

            //spawn blood pack

            return hediff.Severity >= bloodPackCompProps.minSeverityForBadThought;
        }

        public static void GiveThoughtsForTakeBlood(Pawn donor, bool isViolation, bool isBad)
        {
            if (donor.NonHumanlikeOrWildMan())
                return;

            ThoughtDef goodThought = DefDatabase<ThoughtDef>.GetNamed("donatedBloodGood");
            ThoughtDef badThought = DefDatabase<ThoughtDef>.GetNamed("donatedBloodBad");

            if (isViolation)
            {

                donor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(badThought, 1));

                foreach (Pawn colonistsAndPrisoner in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners)
                {
                    colonistsAndPrisoner.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("stoleBlood"));
                }
            }
            else if (isBad)
            {
                donor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(badThought, 0));
            }
            else
                donor.needs.mood.thoughts.memories.TryGainMemory(goodThought);
        }

        public static void GiveThoughtsForKilledForBlood(Pawn donor)
        {
            if (donor.NonHumanlikeOrWildMan())
                return;

            ThoughtDef killedGuestThought = DefDatabase<ThoughtDef>.GetNamed("killedGuestForBlood");
            ThoughtDef killedColonistThought = DefDatabase<ThoughtDef>.GetNamed("killedColonistForBlood");

            foreach (Pawn colonistsAndPrisoner in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners)
            {
                colonistsAndPrisoner.needs.mood.thoughts.memories.TryGainMemory(donor.IsColonist ? killedColonistThought : killedGuestThought);
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
