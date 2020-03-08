using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BloodBank
{
    public class Recipe_TakeBlood : Recipe_Surgery
    {
        public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
            //taking blood is a minor violation at worst, and only with hostile or neutral factions, or prisoners of any faction alignment.
            //Allies will happily donate blood like your own colonists (if you can get them to a medical bed). Useful when friendlies also
            //took part in a battle to get all of your pawns back up faster (with allied blood).
            return (pawn.Faction != billDoerFaction || pawn.IsQuestLodger()) &&
                   this.recipe.isViolation 
                   && (pawn.IsPrisoner || pawn.Faction.RelationWith(billDoerFaction).kind != FactionRelationKind.Ally);
        }
        
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (!recipe.products[0].thingDef.HasComp(typeof(CompBlood)))
            {
                Log.Error("Blood Bank - Give blood operation failed (products[0] has no CompBlood)");
                return;
            }

            bool isBadIdea = BloodBankUtilities.MakeBloodPack(pawn, recipe.products[0].thingDef);
            bool isViolation = IsViolationOnPawn(pawn, null, billDoer.Faction);

            BloodBankUtilities.GiveThoughtsForTakeBlood(pawn, isViolation, isBadIdea);
            if (pawn.Dead)
                BloodBankUtilities.GiveThoughtsForKilledForBlood(pawn);
            
            if (!isViolation)
                return;

            int goodwillChange = -5;//minor effect for taking blood from non-allied or prisoner pawn
            string reason = "GoodwillChangedReason_RemovedBodyPart".Translate("blood");
            pawn.Faction.TryAffectGoodwillWith(billDoer.Faction, goodwillChange, true, true, reason, pawn);
        }

        public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part) { return recipe.label; }
    }
}
