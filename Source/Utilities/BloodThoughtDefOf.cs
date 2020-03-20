// BloodThoughtDefOf.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/12 12:53 AM


using RimWorld;
using Verse;

namespace BloodBank {
    public static class BloodThoughtDefOf
    {
        private static ThoughtDef _giveBloodPositive;
        public static ThoughtDef GiveBloodPositive =>
            _giveBloodPositive ?? (_giveBloodPositive = DefDatabase<ThoughtDef>.GetNamed("GiveBloodPositive"));

        private static ThoughtDef _giveBloodNegative;
        public static ThoughtDef GiveBloodNegative =>
            _giveBloodNegative ?? (_giveBloodNegative = DefDatabase<ThoughtDef>.GetNamed("GiveBloodNegative"));

        private static ThoughtDef _knowStoleBlood;
        public static ThoughtDef KnowStoleBlood =>
            _knowStoleBlood ?? (_knowStoleBlood = DefDatabase<ThoughtDef>.GetNamed("KnowStoleBlood"));


        private static ThoughtDef _killedGuestForBlood;
        public static ThoughtDef KilledGuestForBlood =>
            _killedGuestForBlood ?? (_killedGuestForBlood = DefDatabase<ThoughtDef>.GetNamed("KilledGuestForBlood"));

        private static ThoughtDef _killedColonistForBlood;
        public static ThoughtDef KilledColonistForBlood =>
            _killedColonistForBlood ?? (_killedColonistForBlood = DefDatabase<ThoughtDef>.GetNamed("KilledColonistForBlood"));


        private static ThoughtDef _consumedHumanlikeBloodDirect;
        public static ThoughtDef ConsumedHumanlikeBloodDirect =>
            _consumedHumanlikeBloodDirect ?? (_consumedHumanlikeBloodDirect = ThoughtDef.Named("ConsumedHumanlikeBloodDirect"));

        private static ThoughtDef _consumedHumanlikeBloodDirectBloodlust;
        public static ThoughtDef ConsumedHumanlikeBloodDirectBloodlust =>
            _consumedHumanlikeBloodDirectBloodlust ?? (_consumedHumanlikeBloodDirectBloodlust = ThoughtDef.Named("ConsumedHumanlikeBloodDirectBloodlust"));

        private static ThoughtDef _consumedHumanlikeBloodDirectCannibal;
        public static ThoughtDef ConsumedHumanlikeBloodDirectCannibal =>
            _consumedHumanlikeBloodDirectCannibal ?? (_consumedHumanlikeBloodDirectCannibal = ThoughtDef.Named("ConsumedHumanlikeBloodDirectCannibal"));


        private static ThoughtDef _consumedHumanlikeBloodAsIngredient;
        public static ThoughtDef ConsumedHumanlikeBloodAsIngredient =>
            _consumedHumanlikeBloodAsIngredient ?? (_consumedHumanlikeBloodAsIngredient = ThoughtDef.Named("ConsumedHumanlikeBloodAsIngredient"));

        private static ThoughtDef _consumedHumanlikeBloodAsIngredientCannibal;
        public static ThoughtDef ConsumedHumanlikeBloodAsIngredientCannibal =>
            _consumedHumanlikeBloodAsIngredientCannibal ?? (_consumedHumanlikeBloodAsIngredientCannibal = ThoughtDef.Named("ConsumedHumanlikeBloodAsIngredientCannibal"));


        private static ThoughtDef _consumedInsectHemolymphDirect;
        public static ThoughtDef ConsumedInsectHemolymphDirect =>
            _consumedInsectHemolymphDirect ?? (_consumedInsectHemolymphDirect = ThoughtDef.Named("ConsumedInsectHemolymphDirect"));

        private static ThoughtDef _consumedInsectHemolymphAsIngredient;
        public static ThoughtDef ConsumedInsectHemolymphDirectAsIngredient =>
            _consumedInsectHemolymphAsIngredient ?? (_consumedInsectHemolymphAsIngredient = ThoughtDef.Named("ConsumedInsectHemolymphAsIngredient"));
    }
}
