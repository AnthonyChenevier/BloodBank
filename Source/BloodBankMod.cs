using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BloodBank
{

    //this runs first, and holds some high-level variable stuff
    public class BloodBankMod:Mod
    {
        public static Type AlienDefType;
        public static bool AlienFrameworkExists;

        public BloodBankMod(ModContentPack content) : base(content)
        {
            //alien races require special handling for a couple of things
            Log.Message("BloodBank :: Checking for Humanoid Alien Races...");
            var mod = LoadedModManager.RunningModsListForReading.FirstOrDefault(m => m.PackageId == "erdelf.humanoidalienraces");
            if (mod != null)
            {
                Log.Message("BloodBank :: Humanoid Alien Races found. Looking for Type AlienRace.ThingDef_AlienRace...");

                BloodBankMod.AlienDefType = mod.assemblies.loadedAssemblies.FirstOrDefault(a => a.GetName().Name == "AlienRace")
                                  ?.GetType("AlienRace.ThingDef_AlienRace");

                if (BloodBankMod.AlienDefType != null)
                {
                    Log.Message("BloodBank :: Found Type AlienRace.ThingDef_AlienRace");
                    BloodBankMod.AlienFrameworkExists = true;
                }
                else
                    Log.Warning("BloodBank :: Can't find Type AlienRace.ThingDef_AlienRace");
            }
            else
                Log.Warning("BloodBank :: Humanoid Alien Races not found. ");

            Log.Message("BloodBank :: Processing Harmony Patches");
            //Harmony.DEBUG = true; //Don't leave this uncommented. haha famous last words.
            var harmony = new Harmony("makeitso.bloodbank");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
    //This runs after everything has loaded, just before menu. I have to do this here to ensure
    //Alien Race Framework and BloodBank have both done their Def manipulation.
    [StaticConstructorOnStartup]
    public class AlienRacePostPatch
    {
        static AlienRacePostPatch()
        {
            if (BloodBankMod.AlienFrameworkExists)
            {
                int count = 0;
                Log.Message("BloodBank :: removing BloodBank surgeries added by Alien Race Framework. ");
                DefDatabase<ThingDef>.AllDefsListForReading.ForEach(ar=>
                {
                    //Log.Message($"BloodBank :: {ar} Type-{ar.GetType()}. ");
                    if (ar.GetType() != BloodBankMod.AlienDefType || ar.defName == "Human")
                        return;

                    Log.Message("BloodBank :: Removing surgeries for " + ar.defName);
                    ar.recipes.RemoveAll(r => (r.Worker is Recipe_AdministerBloodTransfusion ||
                                               r.Worker is Recipe_AdministerBloodProduct ||
                                               r.Worker is Recipe_TakeBlood) &&
                                              !ar.RaceAllowsSurgery(r));
                    count++;
                });

                Log.Message($"BloodBank :: {count} {BloodBankMod.AlienDefType}s processed. ");
            }
            else
                Log.Warning("BloodBank :: Didn't find Alien Race Framework. Skipping patch. ");
        }
    }
}
